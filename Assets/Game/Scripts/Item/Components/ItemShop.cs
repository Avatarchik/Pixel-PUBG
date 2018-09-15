using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemShop : MonoBehaviour
{

	public ItemData[] Items;

	void Start ()
	{
		
	}

	public void Buy (ItemData item)
	{
		if (UnitZ.playerManager.PlayingCharacter == null)
			return;
		
		//if (UnitZ.playerManager.PlayingCharacter.inventory.Money > item.Price) { 
		//UnitZ.playerManager.PlayingCharacter.inventory.Money -= item.Price;

			UnitZ.playerManager.PlayingCharacter.inventory.AddItemByItemData (item, 1, -1, -1);

		//}

	}


	void OnGUI ()
	{
		for (int i = 0; i < Items.Length; i++) {
			GUI.DrawTexture (new Rect (10, 80 * i, 50, 50), Items [i].ImageSprite.texture);
			GUI.Label (new Rect (80, 80 * i, 200, 50), Items [i].ItemName);
			if (GUI.Button (new Rect (400, 80 * i, 100, 50), "Buy")) {
				Buy (Items [i]);
			}
		}

	}


}
