using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropItemArea : MonoBehaviour, IDropHandler
{
    public GUIItemLoader loader;
    public string Type;

    public void Start()
    {

    }

    public void OnDrop(PointerEventData data)
    {

        GUIItemEquipped itemEquip = GetItemEquipDrop(data);
        if (itemEquip != null)
        {
            // move from equipped to player inventory
            if (Type == "Inventory")
            {
                if (itemEquip.CurrentItemCollector != null)
                    UnitZ.playerManager.PlayingCharacter.inventory.UnEquipItem(itemEquip.CurrentItemCollector);
                return;
            }
            // drop from equipped to ground
            if (Type == "Ground")
            {
                if (itemEquip.CurrentItemCollector != null)
                    UnitZ.playerManager.PlayingCharacter.inventory.DropItemByCollector(itemEquip.CurrentItemCollector, itemEquip.CurrentItemCollector.Num);
            }
        }


        GUIItemCollector itemDrop = GetDropItem(data);
        if (itemDrop != null)
        {
            // Pick from ground to Player inventory;
            if (itemDrop.Type == "Ground" && Type == "Inventory")
            {
                if (itemDrop.Item != null)
                    UnitZ.playerManager.PlayingCharacter.Interactive(itemDrop.Item.Item.gameObject);
            }

            // Drop from player inventory to ground;
            if (itemDrop.Type == "Inventory" && Type == "Ground")
            {
                if (itemDrop.Item != null)
                    itemDrop.currentInventory.DropItemByCollector(itemDrop.Item, itemDrop.Item.Num);
            }
        }
    }

    private GUIItemCollector GetDropItem(PointerEventData data)
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
