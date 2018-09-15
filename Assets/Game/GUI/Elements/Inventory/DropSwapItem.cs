using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSwapItem : MonoBehaviour, IDropHandler
{
    public GUIItemCollector GUIItem;

    public void Start()
    {
        if (GUIItem == null)
            GUIItem = this.GetComponent<GUIItemCollector>();
    }

    public void OnDrop(PointerEventData data)
    {
        if (GUIItem == null || GUIItem.Item == null)
            return;

        // if it drag from Item equipped inventory
        GUIItemEquipped itemEquip = GetItemEquipDrop(data);
        if (itemEquip != null && GUIItem.Type == "Inventory")
        {
            if (!UnitZ.playerManager.PlayingCharacter.inventory.EquipItemToStickerByCollector(GUIItem.Item, itemEquip.Tag))
            {
                UnitZ.playerManager.PlayingCharacter.inventory.UnEquipItem(itemEquip.CurrentItemCollector);
            }
            return;
        }

        // if it drag from normal inventory
        GUIItemCollector itemDrop = GetItemCollectorDrop(data);
        if (itemDrop != null && itemDrop.Item != null)
        {
            // if it picking from ground  
            if (itemDrop.Type == "Ground" && GUIItem.Type == "Inventory")
            {
                if (itemDrop.Item != null)
                    UnitZ.playerManager.PlayingCharacter.Interactive(itemDrop.Item.Item.gameObject);
                return;
            }

            // if it move from inventory to ground
            if (itemDrop.Type == "Inventory" && GUIItem.Type == "Ground")
            {
                if (itemDrop.Item != null)
                    UnitZ.playerManager.PlayingCharacter.inventory.DropItemByCollector(itemDrop.Item, itemDrop.Item.Num);
                return;
            }

            // if it moved on inventory
            if (GUIItem.currentInventory)
            {
                ItemCollector tmp = new ItemCollector();
                GUIItem.currentInventory.CopyCollector(tmp, GUIItem.Item);
                if ((GUIItem.Type == "Stock" || GUIItem.Type == "Inventory") && itemDrop.Type != "Shop")
                {

                    if (GUIItem.currentInventory != itemDrop.currentInventory)
                    {
                        // Difference Inventory
                        if (GUIItem.currentInventory.character && GUIItem.currentInventory.character.IsMine)
                        {
                            // swap to me
                            UnitZ.playerManager.PlayingCharacter.inventory.PutCollector(itemDrop.Item, GUIItem.Item.InventoryIndex);
                            UnitZ.playerManager.PlayingCharacter.inventory.PutCollectorSync(tmp, itemDrop.Item.InventoryIndex);
                        }
                        else
                        {
                            // swap to another
                            UnitZ.playerManager.PlayingCharacter.inventory.PutCollectorSync(itemDrop.Item, GUIItem.Item.InventoryIndex);
                            UnitZ.playerManager.PlayingCharacter.inventory.PutCollector(tmp, itemDrop.Item.InventoryIndex);
                        }

                    }
                    else
                    {
                        // Same Inventory

                        if (GUIItem.currentInventory.character && GUIItem.currentInventory.character.IsMine)
                        {
                            // is mine
                            GUIItem.currentInventory.SwarpCollector(GUIItem.Item, itemDrop.Item);
                        }
                        else
                        {
                            // is server
                            UnitZ.playerManager.PlayingCharacter.inventory.PutCollectorSync(itemDrop.Item, GUIItem.Item.InventoryIndex);
                            UnitZ.playerManager.PlayingCharacter.inventory.PutCollectorSync(tmp, itemDrop.Item.InventoryIndex);
                        }
                    }
                }
            }
        }
    }

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

    private GUIItemCollector GetItemCollectorDrop(PointerEventData data)
    {
        var originalObj = data.pointerDrag;

        if (originalObj == null)
        {
            return null;
        }
        if (originalObj.GetComponent<DragShortcut>())
            return null;

        if (originalObj.GetComponent<GUIItemCollector>())
        {
            return originalObj.GetComponent<GUIItemCollector>();
        }
        return null;
    }

}