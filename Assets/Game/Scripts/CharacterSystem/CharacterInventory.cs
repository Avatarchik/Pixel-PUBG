
using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

public class CharacterInventory : NetworkBehaviour
{
    [HideInInspector]
    public CharacterSystem character;
    [Header("Graphics")]
    public FPSItemEquipment DefaultHand;
    public ItemSticker[] itemStickers;
    [HideInInspector]
    public List<ItemCollector> Items = new List<ItemCollector>();
    [HideInInspector]
    public PlayerView playerView;
    [HideInInspector]
    public FPSItemEquipment FPSEquipment;
    [HideInInspector]
    public ItemEquipment TDEquipment;
    [HideInInspector]
    [SyncVar(hook = "OnInventoryChanged")]
    public string StickerTextData;
    [HideInInspector]
    public bool Toggle = false;
    [Header("Items")]
    public ItemDataPackage[] StarterItems;
    [HideInInspector]
    [SyncVar]
    public int UpdateCount = 0;
    public bool Limited = false;
    public int LimitedSlot = 30;
    [HideInInspector]
    public bool IsReady;
    [HideInInspector]
    public ItemSticker FPSObject;
    public Transform Pointer;
    public bool EquipStarter = false;

    private void Awake()
    {
        character = this.gameObject.GetComponent<CharacterSystem>();
        playerView = this.gameObject.GetComponent<PlayerView>();

        //取得该角色所有物品
        if (this.transform.GetComponentsInChildren(typeof(ItemSticker)).Length > 0)
        {
            var items = this.transform.GetComponentsInChildren(typeof(ItemSticker));
            itemStickers = new ItemSticker[items.Length];
            for (int i = 0; i < items.Length; i++)
            {
                itemStickers[i] = items[i].GetComponent<ItemSticker>();
                itemStickers[i].ItemIndex = -1;
                itemStickers[i].Index = i;
                if (itemStickers[i].equipType == EquipType.FPSItemView)
                {
                    FPSObject = itemStickers[i];
                }
            }
        }

    }

    public void SetupStarterItem()
    {
        //从开始物品队列清楚物品
        RemoveAllItem();
        int shortkey = 0;
        for (int i = 0; i < StarterItems.Length; i++)
        {
            if (StarterItems[i].item != null)
            {
                AddItemByItemData(StarterItems[i].item, StarterItems[i].Num, -1, shortkey);
                shortkey++;
            }
        }
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null && Items[i].Item != null && Items[i].Item.ItemEquip != null &&
                (Items[i].Item.ItemEquip.itemType == EquipType.PrimaryGun || Items[i].Item.ItemEquip.itemType == EquipType.SecondaryGun || Items[i].Item.ItemEquip.itemType == EquipType.MeleeWeapon))
            {
                if (Items[i].Item.ItemEquip.AutoEquip || Items[i].Item.ItemEquip.AutoToggle)
                {
                    EquipItemByCollector(Items[i]);
                    ToggleUseEquipped(Items[i]);
                }
                break;
            }
        }
    }

    private void Start()
    {
        if (playerView != null && playerView.FPSCamera != null)
        {
            Pointer = playerView.FPSCamera.RayPointer;
        }

        if (EquipStarter)
        {
            //如果可用，生成准星
            ApplyStarterItem();
        }
        IsReady = true;
    }

    public string GenStickerTextData()
    {
        //从所有已装备物品生成保存数据 
        string res = "";
        for (int i = 0; i < itemStickers.Length; i++)
        {
            string index = itemStickers[i].ItemIndex.ToString();
            if (itemStickers[i].transform.childCount <= 0 || !itemStickers[i].IsVisible)
            {
                index = "-1";
            }
            res += index + ",";
        }
        return res;
    }

    public string GetItemDataText()
    {
        //从所有物品生成保存数据
        string itemdata = "";
        string indexdata = "";
        string numdata = "";
        string numtagdata = "";
        string shortcut = "";

        foreach (var item in Items)
        {
            if (item.Item != null)
            {
                indexdata += item.Item.ItemID + ",";
                numdata += item.Num + ",";
                numtagdata += item.NumTag + ",";
                shortcut += item.Shortcut + ",";
            }
        }
        itemdata = indexdata + "|" + numdata + "|" + numtagdata + "|" + shortcut;

        return itemdata;
    }

    public void SetItemsFromText(string itemdatatext)
    {
        //将保存数据转换为库存
        UpdateCount += 1;
        ItemManager item = (ItemManager)GameObject.FindObjectOfType(typeof(ItemManager));
        if (item)
        {
            string[] data = itemdatatext.Split("|"[0]);
            if (data.Length >= 4)
            {
                RemoveAllItem();
                string[] indexList = data[0].Split(","[0]);
                string[] numList = data[1].Split(","[0]);
                string[] numtagList = data[2].Split(","[0]);
                string[] shortcutList = data[3].Split(","[0]);

                for (int i = 0; i < indexList.Length; i++)
                {
                    if (indexList[i] != "")
                    {
                        ItemCollector itemCol = new ItemCollector();
                        itemCol.Item = item.GetItemDataByID(indexList[i]);
                        if (itemCol.Item != null)
                        {
                            int.TryParse(numList[i], out itemCol.Num);
                            int.TryParse(numtagList[i], out itemCol.NumTag);
                            int.TryParse(shortcutList[i], out itemCol.Shortcut);

                            itemCol.Active = true;
                            AddItemByItemDataNoLimit(itemCol.Item, itemCol.Num, itemCol.NumTag, itemCol.Shortcut);
                        }
                    }
                }
            }
        }
    }

    public void AutoEquiping(ItemData itemdata)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID)
            {
                if (Items[i].Item.ItemEquip != null && Items[i].Item.ItemEquip.AutoEquip)
                {
                    EquipItemByCollector(Items[i]);
                }
            }
        }
    }

    public bool DropItemBySameEquipType(ItemData itemdata)
    {
        //检查背包空槽
        int haveSlot = 0;
        for (int i = 0; i < itemStickers.Length; i++)
        {
            if (itemStickers[i] != null && itemdata.ItemEquip != null)
            {
                if (itemStickers[i].equipType == itemdata.ItemEquip.itemType)
                {
                    haveSlot += 1;
                }
            }
        }

        //先从背包扔掉未装备物品
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null && Items[i].Item != null)
            {
                if (Items[i].Item.UniqueItem)
                {
                    if (Items[i].Item.ItemEquip != null && itemdata.ItemEquip != null)
                    {
                        if (Items[i].Item.ItemEquip.itemType == itemdata.ItemEquip.itemType)
                        {
                            if (Items[i].EquipIndex == -1)
                            {
                                Debug.Log("Drop from inventory" + Items[i].Item.ItemName);
                                DropItemByCollector(Items[i], Items[i].Num);
                                return true;
                            }
                        }
                    }
                }
            }
        }

        //扔掉已装备的物品
        int itemcount = 0;
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null && Items[i].Item != null)
            {
                if (Items[i].Item.UniqueItem)
                {
                    if (Items[i].Item.ItemEquip != null && itemdata.ItemEquip != null)
                    {
                        if (Items[i].Item.ItemEquip.itemType == itemdata.ItemEquip.itemType)
                        {
                            itemcount += 1;
                            if (itemcount >= haveSlot)
                            {
                                if (Items[i].EquipIndex != -1)
                                {
                                    Debug.Log("Drop from slot" + Items[i].Item.ItemName);
                                    DropItemByCollector(Items[i], Items[i].Num);
                                    return true;
                                }
                            }
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool DropItemBySticker(ItemSticker sticker)
    {
        //扔掉已装备的物品
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null && Items[i].Item != null)
            {
                if (Items[i].EquipIndex == sticker.Index)
                {
                    DropItemByCollector(Items[i], Items[i].Num);
                    return true;
                }
            }
        }
        return false;
    }

    public void AddItemFromText(string itemdatatext)
    {
        //将保存数据转换为库存项目
        UpdateCount += 1;
        ItemManager item = (ItemManager)GameObject.FindObjectOfType(typeof(ItemManager));
        if (item)
        {
            //Debug.Log ("Get itemlist : " + itemdatatext);
            string[] data = itemdatatext.Split("|"[0]);
            string[] indexList = data[0].Split(","[0]);
            string[] numList = data[1].Split(","[0]);
            string[] numtagList = data[2].Split(","[0]);


            for (int i = 0; i < indexList.Length; i++)
            {
                if (indexList[i] != "")
                {
                    ItemCollector itemCol = new ItemCollector();
                    itemCol.Item = item.GetItemDataByID(indexList[i]);

                    int.TryParse(numList[i], out itemCol.Num);
                    int.TryParse(numtagList[i], out itemCol.NumTag);

                    itemCol.Active = true;
                    AddItemByItemDataNoLimit(itemCol.Item, itemCol.Num, itemCol.NumTag, -1);
                }
            }
        }
    }

    public bool AddItemTest(ItemCollector item, int num)
    {
        if (UnitZ.itemManager != null)
        {
            if (item.Item != null && num > 0)
            {
                ItemData itemdata = UnitZ.itemManager.CloneItemData(item.Item);
                if (itemdata != null)
                {
                    return AddItemTest(itemdata, num);
                }
            }
        }
        return false;
    }

    public bool AddItemTest(ItemData itemdata, int num)
    {
        //如果有足够的空间或更多的条件，添加测试
        if (UnitZ.itemManager != null)
        {
            if (itemdata != null && num > 0)
            {

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID && itemdata.Stack)
                    {
                        return true;
                    }
                }

                int itemsCount = 0;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null && Items[i].Item != null)
                    {
                        itemsCount++;
                    }
                }
                if (Limited && itemsCount >= LimitedSlot)
                {
                    return false;
                }
                return true;

            }
        }
        return false;
    }

    public ItemCollector AddItemByIndex(int index, int num, int numtag)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            var itemdata = UnitZ.itemManager.GetItem(index);
            if (itemdata != null && num > 0)
            {

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].ItemIndex == index && itemdata.Stack)
                    {
                        Items[i].Num += num;
                        return Items[i];
                    }
                }

                int itemsCount = 0;
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null && Items[i].Item != null)
                    {
                        itemsCount++;
                    }
                }
                if (Limited && itemsCount >= LimitedSlot)
                {
                    return null;
                }

                if (itemdata.Stack)
                {
                    ItemCollector itemc = new ItemCollector();
                    itemc.ItemIndex = index;
                    itemc.Item = itemdata;
                    itemc.NumTag = numtag;
                    itemc.Num += num;
                    Items.Add(itemc);
                    if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                        EquipItemByCollector(itemc);
                    return itemc;
                }
                else
                {
                    for (int i = 0; i < num; i++)
                    {
                        ItemCollector itemc = new ItemCollector();
                        itemc.ItemIndex = index;
                        itemc.Item = itemdata;
                        itemc.NumTag = numtag;
                        itemc.Num = 1;
                        Items.Add(itemc);
                        if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                            EquipItemByCollector(itemc);
                        return itemc;
                    }
                }
            }
        }
        return null;
    }

    public ItemCollector AddItemByItemDataNoLimit(ItemData item, int num, int numtag, int shortcut)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            if (item != null && num > 0)
            {
                ItemData itemdata = UnitZ.itemManager.CloneItemData(item);
                if (itemdata != null)
                {

                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID && itemdata.Stack)
                        {
                            Items[i].Num += num;
                            return Items[i];
                        }
                    }

                    if (itemdata.Stack)
                    {
                        ItemCollector itemc = new ItemCollector();
                        itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                        itemc.Item = itemdata;
                        itemc.NumTag = numtag;
                        itemc.Shortcut = shortcut;
                        itemc.Num += num;
                        Items.Add(itemc);
                        return itemc;
                    }
                    else
                    {
                        for (int i = 0; i < num; i++)
                        {
                            ItemCollector itemc = new ItemCollector();
                            itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                            itemc.Item = itemdata;
                            itemc.NumTag = numtag;
                            itemc.Shortcut = shortcut;
                            itemc.Num = 1;
                            Items.Add(itemc);
                            return itemc;
                        }
                    }

                }
            }
        }
        return null;
    }

    public ItemCollector AddItemByItemData(ItemData item, int num, int numtag, int shortcut)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            if (item != null && num > 0)
            {
                ItemData itemdata = UnitZ.itemManager.CloneItemData(item);
                if (itemdata != null)
                {

                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID && itemdata.Stack)
                        {
                            Items[i].Num += num;
                            return Items[i];
                        }
                    }

                    int itemsCount = 0;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i] != null && Items[i].Item != null)
                        {
                            itemsCount++;
                        }
                    }
                    if (Limited && itemsCount >= LimitedSlot)
                    {
                        return null;
                    }

                    if (itemdata.Stack)
                    {
                        ItemCollector itemc = new ItemCollector();
                        itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                        itemc.Item = itemdata;
                        itemc.NumTag = numtag;
                        itemc.Shortcut = shortcut;
                        itemc.EquipIndex = -1;
                        itemc.Num += num;
                        Items.Add(itemc);
                        if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip))
                            EquipItemByCollector(itemc);
                        return itemc;
                    }
                    else
                    {
                        for (int i = 0; i < num; i++)
                        {
                            ItemCollector itemc = new ItemCollector();
                            itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                            itemc.Item = itemdata;
                            itemc.NumTag = numtag;
                            itemc.Shortcut = shortcut;
                            itemc.Num = 1;
                            itemc.EquipIndex = -1;
                            Items.Add(itemc);
                            if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip))
                                EquipItemByCollector(itemc);
                            return itemc;
                        }
                    }
                }
            }
        }
        return null;
    }

    public ItemCollector AddItemByCollector(ItemCollector item)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            if (item.Item != null && item.Num > 0)
            {
                ItemData itemdata = UnitZ.itemManager.CloneItemData(item.Item);
                if (itemdata != null)
                {

                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID && itemdata.Stack)
                        {
                            Items[i].Num += item.Num;
                            return Items[i];
                        }
                    }

                    int itemsCount = 0;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i] != null && Items[i].Item != null)
                        {
                            itemsCount++;
                        }
                    }
                    if (Limited && itemsCount >= LimitedSlot)
                    {
                        return null;
                    }

                    if (itemdata.Stack)
                    {
                        ItemCollector itemc = new ItemCollector();
                        itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                        itemc.Item = itemdata;
                        itemc.NumTag = item.NumTag;
                        itemc.Shortcut = item.Shortcut;
                        itemc.Num += item.Num;
                        Items.Add(itemc);
                        if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                            EquipItemByCollector(itemc);
                        return itemc;
                    }
                    else
                    {
                        for (int i = 0; i < item.Num; i++)
                        {
                            ItemCollector itemc = new ItemCollector();
                            itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                            itemc.Item = itemdata;
                            itemc.NumTag = item.NumTag;
                            itemc.Shortcut = item.Shortcut;
                            itemc.Num = 1;
                            Items.Add(itemc);
                            if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                                EquipItemByCollector(itemc);
                            return itemc;
                        }
                    }
                }
            }
        }
        return null;
    }

    public ItemCollector AddItemByCollector(ItemCollector item, int num, int shortcut)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            if (item.Item != null && num > 0)
            {
                ItemData itemdata = UnitZ.itemManager.CloneItemData(item.Item);
                if (itemdata != null)
                {

                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID && itemdata.Stack)
                        {
                            Items[i].Num += num;
                            return Items[i];
                        }
                    }

                    int itemsCount = 0;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i] != null && Items[i].Item != null)
                        {
                            itemsCount++;
                        }
                    }
                    if (Limited && itemsCount >= LimitedSlot)
                    {
                        return null;
                    }

                    if (itemdata.Stack)
                    {
                        ItemCollector itemc = new ItemCollector();
                        itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                        itemc.Item = itemdata;
                        itemc.NumTag = item.NumTag;
                        itemc.Shortcut = shortcut;
                        itemc.Num += num;
                        Items.Add(itemc);
                        if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                            EquipItemByCollector(itemc);
                        return itemc;
                    }
                    else
                    {

                        for (int i = 0; i < num; i++)
                        {
                            ItemCollector itemc = new ItemCollector();
                            itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                            itemc.Item = itemdata;
                            itemc.NumTag = item.NumTag;
                            itemc.Shortcut = shortcut;
                            itemc.Num = 1;
                            Items.Add(itemc);
                            if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                                EquipItemByCollector(itemc);
                            return itemc;
                        }
                    }

                }
            }
        }
        return null;
    }

    public ItemCollector AddItemByCollector(ItemCollector item, int num)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            if (item.Item != null && num > 0)
            {
                ItemData itemdata = UnitZ.itemManager.CloneItemData(item.Item);
                if (itemdata != null)
                {

                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i].Item != null && Items[i].Item.ItemID == itemdata.ItemID && itemdata.Stack)
                        {
                            Items[i].Num += num;
                            return Items[i];
                        }
                    }

                    int itemsCount = 0;
                    for (int i = 0; i < Items.Count; i++)
                    {
                        if (Items[i] != null && Items[i].Item != null)
                        {
                            itemsCount++;
                        }
                    }
                    if (Limited && itemsCount >= LimitedSlot)
                    {
                        return null;
                    }

                    if (itemdata.Stack)
                    {
                        ItemCollector itemc = new ItemCollector();
                        itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                        itemc.Item = itemdata;
                        itemc.NumTag = item.NumTag;
                        itemc.Shortcut = item.Shortcut;
                        itemc.Num += num;
                        Items.Add(itemc);
                        if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                            EquipItemByCollector(itemc);
                        return itemc;
                    }
                    else
                    {
                        for (int i = 0; i < num; i++)
                        {
                            ItemCollector itemc = new ItemCollector();
                            itemc.ItemIndex = UnitZ.itemManager.GetIndexByID(itemdata.ItemID);
                            itemc.Item = itemdata;
                            itemc.NumTag = item.NumTag;
                            itemc.Shortcut = item.Shortcut;
                            itemc.Num = 1;
                            Items.Add(itemc);
                            if (itemc.Item != null && itemc.Item.ItemEquip != null && (itemc.Item.ItemEquip.AutoEquip || itemc.Item.ItemEquip.AutoToggle))
                                EquipItemByCollector(itemc);
                            return itemc;
                        }
                    }
                }
            }
        }
        return null;
    }

    public CharacterInventory PeerTrade;
    [Command(channel = 0)]
    void CmdAddItemSync(string itemid, int num, int numtag, int shortcut)
    {
        Debug.Log("***Reciev Trad from " + itemid);
        ItemData itemdata = UnitZ.itemManager.CloneItemDataByIndex(itemid);
        if (itemdata)
        {
            PeerTrade.AddItemByItemData(itemdata, num, numtag, shortcut);
        }
    }

    [Command(channel = 0)]
    void CmdRemoveItemSync(int index, int num)
    {
        Debug.Log("Remove " + PeerTrade.name + " index " + index);
        PeerTrade.RemoveItemByCollectorIndex(index, num);
    }

    public void AddItemByCollectorSync(ItemCollector item)
    {
        CmdAddItemSync(item.Item.ItemID, item.Num, item.NumTag, item.Shortcut);
    }

    public void AddItemByCollectorSync(ItemCollector item, int num, int shortcut)
    {
        CmdAddItemSync(item.Item.ItemID, num, item.NumTag, shortcut);
    }

    public void RemoveItemByCollectorSync(ItemCollector itemcollector, int num)
    {
        int index = -1;
        for (int i = 0; i < PeerTrade.Items.Count; i++)
        {
            if (PeerTrade.Items[i] != null && PeerTrade.Items[i] == itemcollector)
            {
                index = i;
                break;
            }
        }
        CmdRemoveItemSync(index, num);

    }

    public bool RemoveItemByCollectorIndex(int index, int num)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {

            if (num > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null && i == index)
                    {

                        if (Items[i].Num <= 0)
                        {
                            Debug.Log(Items[i].Item.ItemName + " Is no more");
                            return false;
                        }
                        if (Items[i].Num < num)
                        {
                            if (Items[i].Num > 0)
                            {
                                Items[i].Num -= Items[i].Num;
                            }
                        }
                        else
                        {
                            Items[i].Num -= num;
                        }

                        if (Items[i].Num <= 0)
                        {
                            RemoveEquipItemByCollector(Items[i]);
                            Items.RemoveAt(index);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool RemoveItem(ItemData itemdata, int num)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            if (itemdata != null && num > 0)
            {

                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null && Items[i].Item.ItemID == itemdata.ItemID)
                    {

                        if (Items[i].Num <= 0)
                        {
                            Debug.Log(Items[i].Item.ItemName + " Is no more");
                            return false;
                        }
                        if (Items[i].Num < num)
                        {
                            if (Items[i].Num > 0)
                            {
                                Items[i].Num -= Items[i].Num;
                            }
                        }
                        else
                        {
                            Items[i].Num -= num;
                        }

                        if (Items[i].Num <= 0)
                        {

                            RemoveEquipItemByCollector(Items[i]);
                            Items.RemoveAt(i);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool RemoveItemByCollector(ItemCollector itemcollector, int num)
    {
        UpdateCount += 1;
        ItemData itemdata = itemcollector.Item;
        if (UnitZ.itemManager != null)
        {

            if (itemdata != null && num > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null && Items[i] == itemcollector)
                    {

                        if (Items[i].Num <= 0)
                        {
                            Debug.Log(Items[i].Item.ItemName + " Is no more");
                            return false;
                        }
                        if (Items[i].Num < num)
                        {
                            if (Items[i].Num > 0)
                            {
                                Items[i].Num -= Items[i].Num;
                            }
                        }
                        else
                        {
                            Items[i].Num -= num;
                        }

                        if (Items[i].Num <= 0)
                        {
                            RemoveEquipItemByCollector(Items[i]);
                            Items.RemoveAt(i);
                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public bool RemoveItemByIndex(int index, int num)
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            var itemdata = UnitZ.itemManager.GetItem(index);
            if (itemdata != null && num > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i] != null && Items[i].ItemIndex == index)
                    {
                        if (Items[i].Num <= 0)
                        {
                            return false;
                        }
                        if (Items[i].Num < num)
                        {
                            if (Items[i].Num > 0)
                            {
                                Items[i].Num -= Items[i].Num;
                            }
                        }
                        else
                        {
                            Items[i].Num -= num;
                        }
                        if (Items[i].Num <= 0)
                        {
                            RemoveEquipItemByCollector(Items[i]);
                            Items.RemoveAt(i);

                        }
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public void RemoveAllItem()
    {
        UpdateCount += 1;
        if (UnitZ.itemManager != null)
        {
            for (int i = 0; i < Items.Count; i++)
            {
                if (Items[i] != null)
                {
                    Items[i].Num = 0;
                    RemoveEquipItemByCollector(Items[i]);
                    Items.RemoveAt(i);
                }
            }
        }
        Items.Clear();
    }

    public int GetItemNum(ItemData itemdata)
    {
        if (UnitZ.itemManager != null)
        {
            if (itemdata != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Item.ItemID == itemdata.ItemID)
                    {
                        return Items[i].Num;
                    }
                }
            }
        }
        return 0;
    }

    public int GetItemNumByIndex(int index)
    {
        if (UnitZ.itemManager != null)
        {
            var itemdata = UnitZ.itemManager.GetItem(index);
            if (itemdata != null)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].ItemIndex == index)
                    {
                        return Items[i].Num;
                    }
                }
            }
        }
        return 0;
    }

    public bool CheckItem(ItemData itemdata, int num)
    {
        if (UnitZ.itemManager != null)
        {
            if (itemdata != null && num > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].Item.ItemID == itemdata.ItemID)
                    {
                        if (Items[i].Num >= num)
                        {
                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    public bool CheckItemByIndex(int index, int num)
    {
        if (UnitZ.itemManager != null)
        {
            var itemdata = UnitZ.itemManager.GetItem(index);
            if (itemdata != null && num > 0)
            {
                for (int i = 0; i < Items.Count; i++)
                {
                    if (Items[i].ItemIndex == index)
                    {
                        if (Items[i].Num >= num)
                        {

                            return true;
                        }
                    }
                }
            }
        }
        return false;
    }

    [Command(channel = 0)]
    void CmdDropitem(string itemid, int numtag, int num, Vector3 position, Quaternion rotation)
    {
        GameObject itemobj = UnitZ.itemManager.GetItemDataByID(itemid).gameObject;
        if (itemobj != null)
        {
            UnitZ.gameNetwork.RequestSpawnItem(itemobj.gameObject, numtag, num, position, rotation);
        }
    }

    public void DropItem(ItemData itemdata, int num, int numtag)
    {
        UpdateCount += 1;
        if (RemoveItem(itemdata, num))
        {
            CmdDropitem(itemdata.ItemID, numtag, num, this.transform.position, itemdata.gameObject.transform.rotation);
        }
    }

    public void DropItemByCollector(ItemCollector item, int num)
    {
        UpdateCount += 1;
        if (RemoveItemByCollector(item, num))
        {
            CmdDropitem(item.Item.ItemID, item.NumTag, num, this.transform.position, item.Item.gameObject.transform.rotation);
        }
    }

    public void OnViewChanged()
    {
        RemoveEquippedItem(FPSObject);
        ToggleInUseEquipped();
    }

    public void ToggleInUseEquipped()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null)
            {
                if (Items[i].InUse)
                {
                    ToggleUseEquipped(Items[i]);
                }
            }
        }
    }

    public void ToggleUseEquipped(ItemCollector collector)
    {
        if (collector != null)
        {
            if (UnitZ.itemManager != null)
            {
                var itemdata = UnitZ.itemManager.GetItem(collector.ItemIndex);
                if (itemdata != null)
                {
                    if (itemdata.ItemFPS)
                    {
                        EquipPrimaryItem(collector);
                        AttachFPSItemViewAndCollector(itemdata.ItemFPS, collector);
                        UpdateCount += 1;
                    }
                }
            }
        }
    }

    [HideInInspector]
    public ItemSticker stickerTarget;
    public bool EquipItemToStickerByCollector(ItemCollector itemCollector, string Tag)
    {
        if (itemCollector.ItemIndex != -1 && itemCollector.Num > 0)
        {
            return EquipItem(itemCollector, Tag);
        }
        return false;
    }

    public bool EquipItemToStickerByCollector(ItemCollector itemCollector, ItemSticker sticker)
    {
        if (itemCollector != null && itemCollector.Item != null && itemCollector.Item.ItemEquip)
        {
            //Debug.Log("Equip to sticker " + itemCollector.Item.ItemName + " > " + sticker.name);
            ItemEquipment itemEquip = itemCollector.Item.ItemEquip;
            if (sticker.equipType == itemEquip.itemType)
            {
                UpdateCount += 1;
                clearEquippedIndex(sticker.Index);
                itemCollector.EquipIndex = sticker.Index;

                AttachSticker(sticker, itemEquip);
                sticker.ItemIndex = itemCollector.ItemIndex;
                sticker.itemCollector = itemCollector;
                return true;
            }
        }
        return false;
    }

    public void EquipItemByItemIndex(int index)
    {
        if (UnitZ.itemManager != null)
        {
            ItemCollector item = GetItemCollectorByItemIndex(index);
            if (item != null)
                EquipItem(item);
        }
    }

    public void EquipItemByCollector(ItemCollector itemCollector)
    {
        if (itemCollector.ItemIndex != -1 && itemCollector.Num > 0)
        {
            EquipItem(itemCollector);
        }
    }

    private ItemCollector previousPrimaryItem;
    public void EquipPrimaryItem(ItemCollector itemCollector)
    {
        if (itemCollector.Item == null || itemCollector.Item.ItemEquip == null)
            return;

        ItemEquipment itemEquip = itemCollector.Item.ItemEquip;
        for (int i = 0; i < itemStickers.Length; i++)
        {
            if (itemStickers[i] != null && itemEquip != null)
            {
                if (itemStickers[i].equipType == EquipType.PrimaryUse)
                {
                    if (itemCollector.Item.SavePreviousUse)
                        previousPrimaryItem = itemCollector;
                    UpdateCount += 1;
                    clearUsedEquipped();
                    itemCollector.InUse = true;
                    AttachSticker(itemStickers[i], itemEquip);
                    itemStickers[i].ItemIndex = itemCollector.ItemIndex;
                    itemStickers[i].itemCollector = itemCollector;
                    UpdatePrimaryInUse();
                    return;
                }
            }
        }
    }

    public void UpdatePrimaryInUse()
    {
        for (int i = 0; i < itemStickers.Length; i++)
        {
            if (itemStickers[i].equipType != EquipType.PrimaryUse && itemStickers[i].equipType != EquipType.FPSItemView)
            {
                if (itemStickers[i].itemCollector != null)
                {
                    itemStickers[i].Visible(!itemStickers[i].itemCollector.InUse);
                }
            }
        }
    }

    public void EquipPreviousPrimary()
    {
        if (previousPrimaryItem != null)
        {
            ToggleUseEquipped(previousPrimaryItem);
        }
    }

    public void EquipItem(ItemCollector itemCollector)
    {
        if (itemCollector != null && itemCollector.Item != null && itemCollector.Item.ItemEquip)
        {
            ItemEquipment itemEquip = itemCollector.Item.ItemEquip;

            int openSlot = -1;
            for (int i = 0; i < itemStickers.Length; i++)
            {
                if (itemStickers[i] != null && itemEquip != null)
                {
                    if (itemStickers[i].equipType == itemEquip.itemType)
                    {
                        if (itemStickers[i].IsEmpty())
                        {
                            openSlot = i;
                            break;
                        }
                    }
                }
            }
            if (openSlot == -1)
            {
                for (int i = 0; i < itemStickers.Length; i++)
                {
                    if (itemStickers[i] != null && itemEquip != null)
                    {
                        if (itemStickers[i].equipType == itemEquip.itemType)
                        {
                            openSlot = i;
                        }
                    }
                }
            }

            if (openSlot != -1)
            {
                UpdateCount += 1;
                clearEquippedIndex(openSlot);
                itemCollector.EquipIndex = openSlot;
                AttachSticker(itemStickers[openSlot], itemEquip);
                itemStickers[openSlot].ItemIndex = itemCollector.ItemIndex;
                itemStickers[openSlot].itemCollector = itemCollector;
                if (itemEquip.AutoToggle)
                {
                    ToggleUseEquipped(itemCollector);
                }
            }
        }
    }

    public void UnEquipItem(ItemCollector itemCollector)
    {
        if (itemCollector != null)
        {
            RemoveEquipItemByCollector(itemCollector);
        }
    }

    public bool EquipItem(ItemCollector itemCollector, string tag)
    {
        if (itemCollector != null && itemCollector.Item != null && itemCollector.Item.ItemEquip)
        {
            ItemEquipment itemEquip = itemCollector.Item.ItemEquip;
            for (int i = 0; i < itemStickers.Length; i++)
            {
                if (itemStickers[i] != null && itemEquip != null && itemStickers[i].Tag == tag)
                {
                    if (itemStickers[i].equipType == itemEquip.itemType)
                    {
                        UpdateCount += 1;
                        clearEquippedIndex(i);
                        itemCollector.EquipIndex = i;

                        AttachSticker(itemStickers[i], itemEquip);
                        itemStickers[i].ItemIndex = itemCollector.ItemIndex;
                        itemStickers[i].itemCollector = itemCollector;
                        return true;
                    }
                }
            }
        }
        return false;
    }

    public ItemSticker GetItemSticker(EquipType Type, string tag)
    {
        for (int i = 0; i < itemStickers.Length; i++)
        {
            if (itemStickers[i] != null && itemStickers[i].equipType == Type && itemStickers[i].Tag == tag)
            {
                return itemStickers[i];
            }
        }
        return null;
    }

    public void SwapEquppedSticker(ItemSticker sticker1, ItemSticker sticker2)
    {
        ItemCollector item1 = GetItemCollectorByEquipIndex(sticker1.Index);
        ItemCollector item2 = GetItemCollectorByEquipIndex(sticker2.Index);

        int itemEquipTmp = sticker1.Index;
        if (item1 != null)
        {
            itemEquipTmp = item1.EquipIndex;
        }

        if (item1 != null)
        {
            if (item1.Item.ItemEquip.itemType == sticker2.equipType)
            {
                if (item2 != null)
                    item1.EquipIndex = item2.EquipIndex;

                AttachSticker(sticker2, item1.Item.ItemEquip);
                sticker2.ItemIndex = item1.ItemIndex;
                sticker2.itemCollector = item1;
            }
        }
        if (item2 != null)
        {

            if (item2.Item.ItemEquip.itemType == sticker1.equipType)
            {
                item2.EquipIndex = itemEquipTmp;
                AttachSticker(sticker1, item2.Item.ItemEquip);
                sticker1.ItemIndex = item2.ItemIndex;
                sticker1.itemCollector = item2;
            }
        }
        RemoveEquippedItem(FPSObject);
        ToggleInUseEquipped();
    }

    public ItemSticker GetStickerByItemCollector(ItemCollector collector)
    {
        for (int i = 0; i < itemStickers.Length; i++)
        {
            if (i == collector.EquipIndex)
            {
                return itemStickers[i];
            }
        }
        return null;
    }

    private void clearEquippedIndex(int slot)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null)
            {
                if (Items[i].EquipIndex == slot)
                {
                    Items[i].EquipIndex = -1;
                }
            }
        }
    }

    private void clearUsedEquipped()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null)
            {
                Items[i].InUse = false;
            }
        }
    }

    public void AttachSticker(ItemSticker sticker, ItemEquipment equip)
    {

        if (sticker != null && equip != null)
        {
            RemoveSticker(sticker);
            Quaternion rotationTemp = sticker.transform.rotation;
            rotationTemp.eulerAngles += sticker.RotationOffset;
            GameObject newitem = (GameObject)GameObject.Instantiate(equip.gameObject, sticker.transform.position, rotationTemp);
            newitem.transform.parent = sticker.gameObject.transform;
            ItemEquipment itemequip = newitem.GetComponent<ItemEquipment>();
            if (sticker.equipType == EquipType.PrimaryUse)
            {
                TDEquipment = itemequip;
            }
            sticker.SendMessage("OnAttached", newitem, SendMessageOptions.DontRequireReceiver);
            this.gameObject.SendMessage("OnEquipChanged", equip.UsingType, SendMessageOptions.DontRequireReceiver);
            UpdateCount += 1;
        }
    }

    public void AttachFPSItemView(FPSItemEquipment item)
    {
        if (item != null && FPSObject != null)
        {
            UpdateCount += 1;
            Quaternion rotationTemp = FPSObject.transform.rotation;
            rotationTemp.eulerAngles += FPSObject.RotationOffset;
            RemoveEquippedItem(FPSObject);

            GameObject newitem = (GameObject)GameObject.Instantiate(item.gameObject, FPSObject.transform.position, rotationTemp);
            newitem.transform.parent = FPSObject.gameObject.transform;
            FPSEquipment = newitem.GetComponent<FPSItemEquipment>();
            FPSObject.SendMessage("OnAttached", newitem, SendMessageOptions.DontRequireReceiver);
        }
    }

    public void AttachFPSItemViewAndCollector(FPSItemEquipment item, ItemCollector itemcollector)
    {
        UpdateCount += 1;
        if (item != null && FPSObject != null)
        {

            Quaternion rotationTemp = FPSObject.transform.rotation;
            rotationTemp.eulerAngles += FPSObject.RotationOffset;
            RemoveEquippedItem(FPSObject);
            GameObject newitem = (GameObject)GameObject.Instantiate(item.gameObject, FPSObject.transform.position, rotationTemp);
            newitem.transform.parent = FPSObject.gameObject.transform;
            FPSEquipment = newitem.GetComponent<FPSItemEquipment>();
            collectorAttachedTemp = itemcollector;
            FPSObject.SendMessage("OnAttached", newitem, SendMessageOptions.DontRequireReceiver);
            FPSObject.GetComponent<ItemSticker>().itemCollector = itemcollector;
            SaveDataToItemCollector((FPSWeaponEquipment)FPSEquipment.GetComponent<FPSWeaponEquipment>(), itemcollector);
        }
    }

    public void IsPrimaryCollectorStillExist()
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null)
            {
                if (Items[i].InUse && Items[i].Num > 0)
                {
                    return;
                }
            }
        }
        RemoveEquippedItem(FPSObject);
    }

    public void RemoveEquipItemByCollector(ItemCollector item)
    {
        ItemSticker sticker = GetStickerByItemCollector(item);
        if (sticker != null)
        {
            //Debug.Log("Remove equip " + sticker.name);
            RemovePrimaryEquip(item);
            UnEquippedItem(sticker);
            clearEquippedIndex(sticker.Index);
            UpdateCount += 1;
        }
    }

    public void RemoveSticker(ItemSticker sticker)
    {
        UpdateCount += 1;
        if (sticker != null)
        {
            sticker.ItemIndex = -1;
            var items = sticker.transform.GetComponentsInChildren(typeof(ItemEquipment));
            for (int i = 0; i < items.Length; i++)
                Destroy(items[i].gameObject);
        }
    }

    public void RemovePrimaryEquip(ItemCollector item)
    {
        for (int i = 0; i < itemStickers.Length; i++)
        {
            if (itemStickers[i].equipType == EquipType.PrimaryUse)
            {
                if (itemStickers[i].itemCollector == item)
                {
                    itemStickers[i].ItemIndex = -1;
                    var items = itemStickers[i].transform.GetComponentsInChildren(typeof(ItemEquipment));
                    for (int c = 0; c < items.Length; c++)
                        Destroy(items[c].gameObject);
                }
            }
        }
    }

    public void UnEquippedItem(ItemSticker sticker)
    {
        UpdateCount += 1;
        if (sticker != null)
        {
            if (FPSObject != null)
            {
                ItemSticker fpsSticker = FPSObject.GetComponent<ItemSticker>();
                if (fpsSticker != null)
                {
                    if (fpsSticker.itemCollector != null && fpsSticker.itemCollector.EquipIndex == sticker.Index)
                    {
                        var fpsitems = FPSObject.transform.GetComponentsInChildren(typeof(FPSItemEquipment));
                        for (int v = 0; v < fpsitems.Length; v++)
                        {
                            fpsitems[v].transform.SetParent(null);
                            GameObject.Destroy(fpsitems[v].gameObject);
                        }
                    }
                }
            }

            var items = sticker.transform.GetComponentsInChildren(typeof(ItemEquipment));
            for (int i = 0; i < items.Length; i++)
            {
                items[i].transform.SetParent(null);
                GameObject.Destroy(items[i].gameObject);
            }
            sticker.ItemIndex = -1;
        }
    }

    public void RemoveEquippedItem(ItemSticker sticker)
    {
        UpdateCount += 1;
        if (sticker != null)
        {
            sticker.ItemIndex = -1;
            var items = sticker.transform.GetComponentsInChildren(typeof(ItemEquipment));
            for (int i = 0; i < items.Length; i++)
            {
                items[i].transform.SetParent(null);
                GameObject.Destroy(items[i].gameObject);
            }

            if (FPSObject != null)
            {
                var fpsitems = FPSObject.transform.GetComponentsInChildren(typeof(FPSItemEquipment));
                for (int v = 0; v < fpsitems.Length; v++)
                {
                    fpsitems[v].transform.SetParent(null);
                    GameObject.Destroy(fpsitems[v].gameObject);
                }
            }
        }
    }

    [HideInInspector]
    public ItemCollector collectorAttachedTemp;

    public int GetCollectorFPSindex()
    {

        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i] != null)
            {
                if (Items[i] == collectorAttachedTemp)
                {
                    return i;
                }
            }
        }
        return 0;
    }

    public ItemCollector GetItemCollectorByItemIndex(int index)
    {
        for (int i = 0; i < Items.Count; i++)
        {
            if (Items[i].ItemIndex == index)
                return Items[i];
        }
        return null;
    }

    public void SaveDataToItemCollector(FPSWeaponEquipment fpsprefab, ItemCollector item)
    {
        if (fpsprefab != null)
        {
            fpsprefab.SetCollectorSlot(item);
        }

    }

    public void SwarpShortcut(ItemCollector item1, ItemCollector item2)
    {
        ItemCollector tmp = new ItemCollector();
        CopyShortcut(tmp, item1);
        CopyShortcut(item1, item2);
        CopyShortcut(item2, tmp);
        UpdateCount += 1;
    }

    public void CopyShortcut(ItemCollector item, ItemCollector source)
    {
        if (item != null && source != null)
        {
            item.Shortcut = source.Shortcut;
        }
    }

    public void PutCollector(ItemCollector item, int invindex)
    {
        putCollector(item.Item.ItemID, invindex, item.Num, item.NumTag);
        UpdateCount += 1;
    }

    public void PutCollectorSync(ItemCollector item, int invindex)
    {
        CmdPutCollector(item.Item.ItemID, invindex, item.Num, item.NumTag);
    }

    [Command(channel = 0)]
    void CmdPutCollector(string itemid, int invid, int num, int numtag)
    {
        if (PeerTrade)
            PeerTrade.putCollector(itemid, invid, num, numtag);
    }

    void putCollector(string itemid, int invid, int num, int numtag)
    {
        Items[invid].Item = UnitZ.itemManager.CloneItemDataByIndex(itemid);
        Items[invid].Num = num;
        Items[invid].NumTag = numtag;
        Items[invid].Shortcut = -1;
        UpdateCount += 1;
    }

    public void SwarpCollector(ItemCollector item1, ItemCollector item2)
    {
        ItemCollector tmp = new ItemCollector();
        CopyCollector(tmp, item1);
        CopyCollector(item1, item2);
        CopyCollector(item2, tmp);
        UpdateCount += 1;
    }

    public void CopyCollector(ItemCollector item, ItemCollector source)
    {

        item.Active = source.Active;
        item.ItemIndex = source.ItemIndex;
        item.Item = source.Item;
        item.Num = source.Num;
        item.NumTag = source.NumTag;
        item.Shortcut = source.Shortcut;
        item.EquipIndex = source.EquipIndex;
    }

    public ItemCollector GetItemCollectorByShortCutIndex(int index)
    {
        if (UnitZ.itemManager != null)
        {
            foreach (ItemCollector item in Items)
            {
                if (item != null)
                {
                    if (item.Shortcut == index)
                    {
                        return item;
                    }
                }
            }
        }
        return null;
    }

    public ItemCollector GetItemCollectorByEquipIndex(int index)
    {
        if (UnitZ.itemManager != null)
        {
            foreach (ItemCollector item in Items)
            {
                if (item != null)
                {
                    if (item.EquipIndex == index)
                    {
                        return item;
                    }
                }
            }
        }
        return null;
    }

    public ItemCollector GetLatestItemCollector()
    {
        for (int i = Items.Count - 1; i >= 0; i--)
        {
            if (Items[i] != null)
            {
                return Items[i];
            }
        }
        return null;
    }

    public void DeleteShortcut(ItemCollector itemCollector, int shortcut)
    {
        if (UnitZ.itemManager != null)
        {
            foreach (ItemCollector item in Items)
            {
                if (item != null)
                {
                    if (itemCollector != item)
                    {
                        if (item.Shortcut == shortcut)
                        {
                            item.Shortcut = -1;
                        }
                    }
                    if (itemCollector == item)
                    {
                        item.Shortcut = shortcut;
                    }
                }
            }
        }
    }

    private int LateUpdateCount;
    private void Update()
    {
        int i = 0;
        foreach (ItemCollector item in Items)
        {
            item.InventoryIndex = i;
            i++;
        }
        if (isLocalPlayer && IsReady)
        {
            // 向服务器更新背包
            if (LateUpdateCount != UpdateCount)
            {
                string stickertext = GenStickerTextData();
                CmdUpdateOtherInventory(stickertext);
                LateUpdateCount = UpdateCount;
            }
        }

        if (!IsReady)
        {
            if (StickerTextData != GenStickerTextData())
            {
                onReceivedStickers(StickerTextData);
            }
        }
    }

    [Command(channel = 1)]
    public void CmdUpdateOtherInventory(string text)
    {
        if (text != StickerTextData)
        {
            StickerTextData = text;
        }
    }

    public void UpdateSticker(string text)
    {
        StickerTextData = text;
        CmdUpdateOtherInventory(StickerTextData);
        IsReady = true;
    }

    public void ApplyStarterItem()
    {
        UpdateCount += 1;
        IsReady = true;
        SetupStarterItem();
        UpdateInventoryToAll();
    }

    public void UpdateInventoryToAll()
    {
        IsReady = true;
        string stickertext = GenStickerTextData();
        CmdUpdateOtherInventory(stickertext);
    }

    public void OnInventoryChanged(string text)
    {
        StickerTextData = text;
        if (!isLocalPlayer)
            onReceivedStickers(text);
    }

    private void onReceivedStickers(string text)
    {
        if (text == "")
            return;

        IsReady = true;

        string[] stickers = text.Split(","[0]);
        for (int i = 0; i < stickers.Length; i++)
        {
            if (stickers[i] != "")
            {
                int indexget = 2;
                if (int.TryParse(stickers[i], out indexget))
                {
                    if (UnitZ.itemManager != null && i < itemStickers.Length && i >= 0)
                    {
                        var itemdata = UnitZ.itemManager.GetItem(indexget);

                        if (itemdata != null)
                        {
                            AttachSticker(itemStickers[i], itemdata.ItemEquip);
                            itemStickers[i].ItemIndex = indexget;
                        }

                        if (indexget == -1)
                        {
                            RemoveSticker(itemStickers[i]);
                        }
                    }
                }
            }
        }
    }

    private void LateUpdate()
    {
        FreeHandsChecker();
    }

    public bool IsFreeHanded()
    {
        if ((FPSObject != null && FPSObject != null) &&
            (FPSObject.gameObject.transform.childCount <= 0))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void FreeHandsChecker()
    {
        if (IsFreeHanded())
        {
            MeleeMode();
        }
    }

    public void MeleeMode()
    {
        if (DefaultHand == null)
            return;

        AttachFPSItemView(DefaultHand);
    }

    public void EquipmentOnAction(Vector3 direction, byte num, byte spread, byte seed)
    {
        if (TDEquipment)
        {
            TDEquipment.Action(direction, num, spread, seed);
        }
    }

    public ItemCollector[] GetAllItemAround(float radius)
    {
        List<ItemCollector> items = new List<ItemCollector>();
        GameObject[] itemAround = (GameObject[])GameObject.FindGameObjectsWithTag("Item");
        for (int i = 0; i < itemAround.Length; i++)
        {
            if (Vector3.Distance(this.transform.position, itemAround[i].transform.position) <= radius)
            {
                ItemCollector item = new ItemCollector();
                ItemData itemdata = itemAround[i].GetComponent<ItemData>();
                item.Item = itemdata;
                item.ItemIndex = itemdata.ItemIndex;
                item.Num = itemdata.Quantity;
                item.NumTag = itemdata.NumTag;
                items.Add(item);
            }
        }
        return items.ToArray();
    }
}

[System.Serializable]
public class ItemDataPackage
{
    public ItemData item;
    public int Num = 1;
}
