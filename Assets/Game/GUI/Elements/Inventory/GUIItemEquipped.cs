using UnityEngine;
using UnityEngine.EventSystems;

//��װ�����GUI
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

    //���
    public void Clear()
    {
        if (CurrentItemCollector != null)
            CurrentItemCollector.Clear();
        CurrentSticker = null;
    }
    
    //�л�
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

    //�ӳ�
    public void OnDrop(PointerEventData data)
    {
        if (UnitZ.playerManager.PlayingCharacter == null)
            return;

        GUIItemEquipped itemEquip = GetItemEquipDrop(data);
        if (itemEquip != null)
        {
            //����װ����Ʒ�л�
            UnitZ.playerManager.PlayingCharacter.inventory.SwapEquppedSticker(CurrentSticker, itemEquip.CurrentSticker);
            return;
        }

        GUIItemCollector itemDrop = GetItemCollectorDrop(data);
        if (itemDrop != null && itemDrop.Item != null)
        {
            //�����װ����Ʒ
            if (itemDrop.Type == "Inventory")
            {
                if (itemDrop.Item != null)
                {
                    if (UnitZ.playerManager.PlayingCharacter.inventory.EquipItemToStickerByCollector(itemDrop.Item, CurrentSticker))
                        UnitZ.playerManager.PlayingCharacter.inventory.ToggleUseEquipped(itemDrop.Item);
                }
            }

            //�ӵ���װ����Ʒ
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

    //�ӻ����
    public void DropBackToInventory()
    {
        if (CurrentItemCollector != null)
            UnitZ.playerManager.PlayingCharacter.inventory.UnEquipItem(CurrentItemCollector);
    }

    //�ӻص���
    public void DropToGround()
    {
        if (CurrentItemCollector != null)
            UnitZ.playerManager.PlayingCharacter.inventory.DropItemByCollector(CurrentItemCollector, CurrentItemCollector.Num);
    }

    //GUi����
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

    //GUI��Ʒװ��
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
