using UnityEngine.EventSystems;
using UnityEngine;

public class GUIItemUse : MonoBehaviour, IPointerClickHandler
{

    public GUIItemCollector guiitem;
    public GameObject DropBT;
    void Start()
    {
        if (guiitem == null)
            guiitem = this.GetComponent<GUIItemCollector>();

        if (DropBT)
            DropBT.SetActive(guiitem.Type != "Ground");
    }

    public void Use()
    {
        if (guiitem == null)
            return;

        if (guiitem.Type == "Ground")
        {
            if (UnitZ.playerManager != null && UnitZ.playerManager.PlayingCharacter != null && guiitem.Item != null && guiitem.Item.Item != null) 
                UnitZ.playerManager.PlayingCharacter.Interactive(guiitem.Item.Item.gameObject);
        }

        if (guiitem.Type == "Inventory")
        {
            if (guiitem.currentInventory != null && guiitem.Item != null)
            {
                guiitem.currentInventory.EquipItemByCollector(guiitem.Item);
            }
        }
    }

    public void DropToGround()
    {
        if (guiitem.Type == "Inventory")
        {
            if (guiitem.Item != null)
                guiitem.currentInventory.DropItemByCollector(guiitem.Item, guiitem.Item.Num);
        }
    }

    public void OnPointerClick(PointerEventData eventData)
    {
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            Use();
        }
    }


}
