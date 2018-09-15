using UnityEngine;
using System.Collections;

public class GUIItemDataNeed : GUIItemData
{
	public Color Ready = Color.green;
	public Color NotReady = Color.red;
	public CharacterInventory Inventory;
	public int Need;
	
	void Update ()
	{
		// just update a GUI elements
		if (Item) {
			if (Icon) {
				Icon.enabled = true;
				Icon.sprite = Item.ImageSprite;
			}
			if (Name) {
				Name.enabled = true;
				Name.text = Item.ItemName; 
			}
			if (Num) {
				Num.enabled = true;
				if (Inventory) {
					if (Inventory.CheckItem (Item, Need)) {
						Num.text = "X "+Need+" Ready";
						Num.color = Ready;
					} else {
						Num.text = "X "+Need;
						Num.color = NotReady;
					}
				}
			}
		}
	}
}
