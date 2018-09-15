using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropStockArea : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	[HideInInspector]
	public CharacterInventory inventory;
	public GUIItemLoader loader;
	public bool IsMine;
	public string Type;
	private GUIItemCollector itemDrop;
	public ItemData Currency;

	public void Start ()
	{
		
	}

	public void Trade (int number)
	{
		
		if (loader == null || itemDrop == null)
			return;
		
		
			
		if (itemDrop != null && inventory != null && itemDrop.Item != null) {
				
			if (number <= 0) {
				number = 1;	
			}

			if (inventory != itemDrop.currentInventory) {
				if (itemDrop.currentInventory.character && itemDrop.currentInventory.character.IsMine) {
					// ItemDrop from my inventory to another inventory.
					switch (Type) {
					case "Stock":
								// Move to stock.
						Debug.Log ("Move to Stock " + inventory.name);
						if (number > itemDrop.Item.Num) {
							number = itemDrop.Item.Num;	
						}
						if (inventory != null && inventory.PeerTrade != null) {
							inventory.PeerTrade.AddItemByCollectorSync (itemDrop.Item, number, -1);
							itemDrop.currentInventory.RemoveItemByCollector (itemDrop.Item, number);
						}
								
						break;
					}
				} else {
					// ItemDrop from another to my inventory.
					switch (itemDrop.Type) {
					case "Stock":
							// Move to my inventory.
						if (number > itemDrop.Item.Num) {
							number = itemDrop.Item.Num;	
						}
						if (inventory != null && inventory.PeerTrade != null) {
							if (inventory.AddItemTest (itemDrop.Item, number)) {
								inventory.AddItemByCollector (itemDrop.Item, number, -1);
								inventory.RemoveItemByCollectorSync (itemDrop.Item, number);
							} else {
								Debug.Log ("Inventory is full");	
							}
						}
						break;
					}
				}
	
			}
		}
		
	}

	public void OnDrop (PointerEventData data)
	{
		itemDrop = GetDropSprite (data);
		
		
		if (loader == null || itemDrop == null)
			return;
		
		
		inventory = loader.currentInventory;
		
		if (inventory != itemDrop.currentInventory) {
			if (TooltipTrade.Instance != null) {
				TooltipTrade.Instance.StartTrade (itemDrop.Item, this);
			}
		}
	}

	public void OnPointerEnter (PointerEventData data)
	{

	}

	public void OnPointerExit (PointerEventData data)
	{

	}

	private GUIItemCollector GetDropSprite (PointerEventData data)
	{
		var originalObj = data.pointerDrag;
		
		if (originalObj == null) {
			return null;
		}
		
		if (originalObj.GetComponent<GUIItemCollector> ()) {
			return originalObj.GetComponent<GUIItemCollector> ();
		}
		return null;
	}
}
