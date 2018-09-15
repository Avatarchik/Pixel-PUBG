using UnityEngine;
using UnityEngine.EventSystems;

//已装备面板GUI
public class GUIItemEquipped : MonoBehaviour, IDropHandler
{
    public ItemCollector CurrentItemCollector;
    public ItemSticker CurrentSticker;
    public string Tag;
    public EquipType Type;
    [HideInInspector]
    public GUIItemCollector guiItem;

    void Start()
    {
    }

    void Awake()
    {
        guiItem = this.GetComponent<GUIItemCollector>();
    }

    //清除
    public void Clear()
    {
        if (CurrentItemCollector != null)
            CurrentItemCollector.Clear();
        CurrentSticker = null;
    }
    
    //切换
    public void Toggle(){
     if (UnitZ.playerManager != null && UnitZ.playerManager.PlayingCharacter != null && UnitZ.playerManager.PlayingCharacter.inventory != null)
                UnitZ.playerManager.PlayingCharacter.inventory.ToggleUseEquipped(CurrentItemCollector);   
    }

    public void SetupCollector()
    {
        //if (UnitZ.playerManager.PlayingCharacter == null)
            //return;

        if (CurrentSticker != null)
        {
            CurrentItemCollector = UnitZ.playerManager.PlayingCharacter.inventory.GetItemCollectorByEquipIndex(CurrentSticker.Index);
            if (guiItem != null)
                guiItem.Item = CurrentItemCollector;
        }
    }

    //扔出
    public void OnDrop(PointerEventData data)
    {
        if (UnitZ.playerManager.PlayingCharacter == null)
            return;

        GUIItemEquipped itemEquip = GetItemEquipDrop(data);
        if (itemEquip != null)
        {
            //与已装备物品切换
            UnitZ.playerManager.PlayingCharacter.inventory.SwapEquppedSticker(CurrentSticker, itemEquip.CurrentSticker);
            return;
        }

        GUIItemCollector itemDrop = GetItemCollectorDrop(data);
        if (itemDrop != null && itemDrop.Item != null)
        {
            //从面板装备物品
            if (itemDrop.Type == "Inventory")
            {
                if (itemDrop.Item != null)
                {
                    if (UnitZ.playerManager.PlayingCharacter.inventory.EquipItemToStickerByCollector(itemDrop.Item, CurrentSticker))
                        UnitZ.playerManager.PlayingCharacter.inventory.ToggleUseEquipped(itemDrop.Item);
                }
            }

            //从地上装备物品
            if (itemDrop.Type == "Ground")
            {
                if (itemDrop.Item != null)
                {
                    UnitZ.playerManager.PlayingCharacter.Interactive(itemDrop.Item.Item.gameObject);
                    UnitZ.playerManager.PlayingCharacter.inventory.stickerTarget = CurrentSticker;
                }
            }
        }
    }

    //扔回面板
    public void DropBackToInventory()
    {
        if (CurrentItemCollector != null)
            UnitZ.playerManager.PlayingCharacter.inventory.UnEquipItem(CurrentItemCollector);
    }

    //扔回地上
    public void DropToGround()
    {
        if (CurrentItemCollector != null)
            UnitZ.playerManager.PlayingCharacter.inventory.DropItemByCollector(CurrentItemCollector, CurrentItemCollector.Num);
    }

    //GUi控制
    private GUIItemCollector GetItemCollectorDrop(PointerEventData data)
    {
        var originalObj = data.pointerDrag;

        if (originalObj == null)
        {
            return null;
        }
        if (originalObj.GetComponent<GUIItemCollector>())
        {
            return originalObj.GetComponent<GUIItemCollector>();
        }
        return null;
    }

    //GUI物品装备
    private GUIItemEquipped GetItemEquipDrop(PointerEventData data)
    {
        var originalObj = data.pointerDrag;

        if (originalObj == null)
        {
            return null;
        }
        if (originalObj.GetComponent<GUIItemEquipped>())
        {
            return originalObj.GetComponent<GUIItemEquipped>();
        }
        return null;
    }
}
