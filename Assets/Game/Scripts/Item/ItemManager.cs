//物品管理
using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Networking;

public class ItemManager : MonoBehaviour
{
    public ItemData[] ItemsList;
    public Dictionary<int, FPSItemEquipment> ItemEquipments = new Dictionary<int, FPSItemEquipment>();
    public string Suffix = "UZ";

    void Start()
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            UnitZ.gameNetwork.spawnPrefabs.Add(ItemsList[i].gameObject);
            if (ItemsList[i].ItemFPS)
            {
                FPSItemPlacing fpsItemPlacer = ItemsList[i].ItemFPS.GetComponent<FPSItemPlacing>();
                if (fpsItemPlacer)
                {
                    if (fpsItemPlacer.Item != null)
                    {
                        NetworkIdentity objPlace = fpsItemPlacer.Item.GetComponent<NetworkIdentity>();
                        if (objPlace)
                        {
                            UnitZ.gameNetwork.spawnPrefabs.Add(fpsItemPlacer.Item.gameObject);
                        }
                    }
                }
                //扔出物品
                FPSItemThrow fpsItemThrow = ItemsList[i].ItemFPS.GetComponent<FPSItemThrow>();
                if (fpsItemThrow)
                {
                    if (fpsItemThrow.Item != null)
                    {
                        NetworkIdentity objPlace = fpsItemThrow.Item.GetComponent<NetworkIdentity>();
                        if (objPlace)
                        {
                            UnitZ.gameNetwork.spawnPrefabs.Add(fpsItemThrow.Item.gameObject);
                        }
                    }
                }
            }
        }
    }

    void Awake()
    {
        ItemEquipments = new Dictionary<int, FPSItemEquipment>(ItemsList.Length);
        for (int i = 0; i < ItemsList.Length; i++)
        {
            ItemsList[i].ItemID = Suffix + i;
            ItemsList[i].ItemIndex = i;
            //每个物体分配ID
            if (ItemsList[i].ItemFPS)
            {
                FPSItemEquipment fpsItemEquipment = ItemsList[i].ItemFPS.GetComponent<FPSItemEquipment>();
                if (fpsItemEquipment)
                {
                    fpsItemEquipment.ItemID = ItemsList[i].ItemID;
                    fpsItemEquipment.ItemIndex = i;
                    ItemEquipments[i] = fpsItemEquipment;
                }
                FPSWeaponEquipment weapon = ItemsList[i].ItemFPS.GetComponent<FPSWeaponEquipment>();
                if (weapon)
                {
                    if (ItemsList[i].ItemEquip)
                    {
                        ItemsList[i].ItemEquip.UsingType = weapon.UsingType;
                    }
                }
                //物品放置
                FPSItemPlacing fpsItemPlacer = ItemsList[i].ItemFPS.GetComponent<FPSItemPlacing>();
                if (fpsItemPlacer)
                {
                    if (fpsItemPlacer.Item != null)
                    {
                        ObjectSpawn objSpawn = fpsItemPlacer.Item.GetComponent<ObjectSpawn>();
                        if (objSpawn)
                        {
                            objSpawn.ItemID = ItemsList[i].ItemID;
                            if (objSpawn.Item)
                            {
                                ObjectPlacing objPlace = objSpawn.Item.GetComponent<ObjectPlacing>();
                                if (objPlace)
                                {
                                    objPlace.ItemID = ItemsList[i].ItemID;
                                }
                            }
                        }
                    }
                }
            }
        }
    }

    //物品装备
    public FPSItemEquipment GetFPSitem(int index)
    {
        return ItemEquipments[index];
    }

    //取得物品
    public ItemData GetItem(int index)
    {
        if (index < ItemsList.Length && index >= 0)
        {
            return ItemsList[index];
        }
        else
        {
            return null;
        }
    }

    //通过ID取得索引
    public int GetIndexByID(string itemid)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (itemid == ItemsList[i].ItemID)
            {
                return i;
            }
        }
        return -1;
    }

    //通过名字取得索引
    public int GetIndexByName(string itemname)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (itemname == ItemsList[i].ItemName)
            {
                return i;
            }
        }
        return -1;
    }

    //复制物品数据
    public ItemData CloneItemData(ItemData item)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (item.ItemID == ItemsList[i].ItemID)
            {
                return ItemsList[i];
            }
        }
        return null;
    }

    //通过物品数据取得索引
    public int GetItemIndexByItemData(ItemData item)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (item.ItemID == ItemsList[i].ItemID)
            {
                return i;
            }
        }
        return -1;
    }

    //通过索引复制物品数据
    public ItemData CloneItemDataByIndex(string itemID)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (ItemsList[i].ItemID == itemID)
            {
                return ItemsList[i];
            }
        }
        return null;
    }

    //通过ID获取物品数据
    public ItemData GetItemDataByID(string itemid)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (itemid == ItemsList[i].ItemID)
            {
                return ItemsList[i];
            }
        }
        return null;
    }

    //通过名字获取物品数据
    public ItemData GetItemDataByName(string itemname)
    {
        for (int i = 0; i < ItemsList.Length; i++)
        {
            if (itemname == ItemsList[i].ItemName)
            {
                return ItemsList[i];
            }
        }
        return null;
    }

    //放置物品
    public void PlacingObject(string itemid, Vector3 position, Vector3 normal)
    {
        ItemData itemdata = GetItemDataByID(itemid);
        if (itemdata.ItemFPS)
        {
            FPSItemPlacing fpsplacing = itemdata.ItemFPS.GetComponent<FPSItemPlacing>();
            if (fpsplacing)
            {
                if (fpsplacing.Item)
                {

                    GameObject obj = UnitZ.gameNetwork.RequestSpawnObject(fpsplacing.Item, position, Quaternion.identity);
                    if (obj)
                    {
                        ObjectPlacing objplaced = obj.GetComponent<ObjectPlacing>();
                        objplaced.transform.forward = normal;
                        objplaced.SetItemID(itemid);

                    }
                }
            }
        }
    }

    //直接放置物品
    public void DirectPlacingObject(string itemid, string itemuid, Vector3 position, Quaternion rotation)
    {
        //Debug.Log("Direct place "+itemid);
        ItemData itemdata = GetItemDataByID(itemid);
        if (itemdata.ItemFPS)
        {
            FPSItemPlacing fpsplacing = itemdata.ItemFPS.GetComponent<FPSItemPlacing>();
            if (fpsplacing)
            {
                if (fpsplacing.Item)
                {

                    GameObject obj = UnitZ.gameNetwork.RequestSpawnObject(fpsplacing.Item, position, rotation);
                    if (obj)
                    {
                        ObjectPlacing objplaced = obj.GetComponent<ObjectPlacing>();
                        objplaced.SetItemID(itemid);
                        objplaced.SetItemUID(itemuid);
                    }
                }
            }
        }
    }
}

