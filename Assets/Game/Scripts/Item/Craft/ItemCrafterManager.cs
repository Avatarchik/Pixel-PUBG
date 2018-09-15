using UnityEngine;
using System.Collections;

public class ItemCrafterManager : MonoBehaviour
{

	public ItemCrafter[] ItemCraftList;
	[HideInInspector]
	public ItemCrafter ItemSelected;
	private CharacterInventory characterInventory;
	[HideInInspector]
	public bool crafting;
	private float timeTemp;
	[HideInInspector]
	public float CraftingDuration;
	[HideInInspector]
	public float CraftingDurationNormalize;

	void Start ()
	{
		crafting = false;
		CraftingDurationNormalize = 0;
	}

	void Update ()
	{
		
		if (crafting && ItemSelected != null && characterInventory != null) {
			bool cancomplete = true;
			CraftingDuration = ((timeTemp + ItemSelected.CraftTime) - Time.time);
			CraftingDurationNormalize = ((1.0f / ItemSelected.CraftTime) * CraftingDuration);

			for (int i=0; i<ItemSelected.ItemNeeds.Length; i++) {
				if (ItemSelected.ItemNeeds [i].Item) {
					if (characterInventory.GetItemNum (ItemSelected.ItemNeeds [i].Item) < ItemSelected.ItemNeeds [i].Num) {
						cancomplete = false;
					}
				}
			}
			if (!cancomplete) {
				Debug.Log ("stop crafting");
				CancelCraft ();	
			} else {
				if (Time.time >= timeTemp + ItemSelected.CraftTime) {
					CraftComplete ();
				}
			}
		}
		if (crafting) {
			if (ItemSelected == null)
				CancelCraft ();
		}
	}
	
	public void CraftSelected (ItemCrafter item)
	{
		ItemSelected = item;
	}
	
	public bool Craft (CharacterInventory inventory)
	{
		if (ItemSelected == null || inventory == null)
			return false;
		
		characterInventory = inventory;
		for (int i=0; i<ItemSelected.ItemNeeds.Length; i++) {
			if (ItemSelected.ItemNeeds [i].Item) {
				if (characterInventory.GetItemNum (ItemSelected.ItemNeeds [i].Item) < ItemSelected.ItemNeeds [i].Num) {
					return false;
				}
			}
		}
		crafting = true;
		timeTemp = Time.time;
		return true;
	}
	
	public bool CheckNeeds (ItemCrafter Crafter, CharacterInventory inventory)
	{
		if (Crafter == null || inventory == null)
			return false;
		
		for (int i=0; i<Crafter.ItemNeeds.Length; i++) {
			if (Crafter.ItemNeeds [i].Item) {
				if (inventory.GetItemNum (Crafter.ItemNeeds [i].Item) < Crafter.ItemNeeds [i].Num) {
					return false;
				}
			}
		}
		return true;
	}
	
	public void CraftComplete ()
	{
		if (characterInventory != null && ItemSelected != null) {
			for (int i=0; i<ItemSelected.ItemNeeds.Length; i++) {
				if (ItemSelected.ItemNeeds [i].Item) {
					characterInventory.RemoveItem (ItemSelected.ItemNeeds [i].Item, ItemSelected.ItemNeeds [i].Num);
				}
			}
			characterInventory.AddItemByItemData (ItemSelected.ItemResult, ItemSelected.NumResult, -1, -1);
		}
		Debug.Log ("craft complete");
		CancelCraft ();
	}
	
	public void CancelCraft ()
	{
		CraftingDurationNormalize = 0;
		crafting = false;
		ItemSelected = null;
	}
	
}
