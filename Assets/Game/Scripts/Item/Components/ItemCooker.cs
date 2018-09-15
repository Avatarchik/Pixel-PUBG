using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class ItemCooker : NetworkBehaviour
{


	public CharacterInventory inventory;
	public ItemCrafter[] ItemMixes;
	public float UpdateInterval = 1;
	float timeTmp;

	void Start ()
	{
		inventory = this.GetComponent<CharacterInventory> ();
	}

	void Update ()
	{
		if (inventory == null)
			return;

		if (isServer) {
			UpdateMixer ();
		}
	}

	void checkCook ()
	{
		for (int c = 0; c < ItemMixes.Length; c++) {
			for (int x = 0; x < ItemMixes[c].ItemNeeds.Length; x++) {
				ItemMixes [c].StartBuild = inventory.CheckItem (ItemMixes [c].ItemNeeds [x].Item, ItemMixes [c].ItemNeeds [x].Num);
			}
		}
	}

	void UpdateMixer ()
	{
		if (inventory == null)
			return;

		if (Time.time > timeTmp + UpdateInterval) {
			checkCook ();
			timeTmp = Time.time;

			for (int c = 0; c < ItemMixes.Length; c++) {
				if (ItemMixes [c].StartBuild) {
					if (Time.time >= ItemMixes [c].CraftTimeTemp + ItemMixes [c].CraftTime) {
						inventory.AddItemByItemDataNoLimit (ItemMixes [c].ItemResult, ItemMixes [c].NumResult, -1, -1);
						for (int v = 0; v < ItemMixes [c].ItemNeeds.Length; v++) {
							inventory.RemoveItem (ItemMixes [c].ItemNeeds [v].Item, ItemMixes [c].ItemNeeds [v].Num);
						}
						ItemMixes [c].CraftTimeTemp = Time.time;
					}
				}
			}
		}
	}
}
