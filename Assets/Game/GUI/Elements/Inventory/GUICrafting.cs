using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUICrafting : GUICraft
{

	public ValueBar Process;
	public Button CraftButton;
	
	void Start ()
	{
		if (Icon)
			Icon.enabled = false;
		if (Name)
			Name.enabled = false;
		if (Process)
			Process.gameObject.SetActive (false);
		if (CraftButton)
			CraftButton.gameObject.SetActive (false);
	}

	void Update ()
	{
		if (UnitZ.itemCraftManager) {
			if (Process) {
				// Just Update ValueBar GUI related to crafting time in Sec.
				if (UnitZ.itemCraftManager.crafting && UnitZ.itemCraftManager.ItemSelected == Crafter) {
					Process.gameObject.SetActive (true);
					Process.ValueMax = 1;
					Process.Value = 1 - UnitZ.itemCraftManager.CraftingDurationNormalize;
					Process.CustomText = Mathf.Floor (UnitZ.itemCraftManager.CraftingDuration).ToString () + " SEC.";
				} else {
					Process.gameObject.SetActive (false);
				}
			}
			
			if (CraftButton) {
				// Craft button will hide when crafting.
				if (UnitZ.itemCraftManager.crafting) {
					CraftButton.gameObject.SetActive (false);	
				} else {
					if (UnitZ.playerManager && UnitZ.playerManager.PlayingCharacter) {
						// Craft button will showing only when enough the resources.
						if (UnitZ.itemCraftManager.CheckNeeds (Crafter, UnitZ.playerManager.PlayingCharacter.inventory)) {
							CraftButton.gameObject.SetActive (true);	
						} else {
							CraftButton.gameObject.SetActive (false);
						}
					}	
				}
			}
		
			if (Crafter != null && Crafter.ItemResult != null) {
				// Just update GUI elements
				if (Icon != null && Crafter.ItemResult.ImageSprite != null) {
					Icon.sprite = Crafter.ItemResult.ImageSprite;
					Icon.enabled = true;
				}
			
				if (Name != null) {
					Name.text = Crafter.ItemResult.ItemName;
					Name.enabled = true;
				}
			}
		}
	}

	public void CancelCraft ()
	{
		// function for cancle
		if (UnitZ.itemCraftManager && UnitZ.itemCraftManager.ItemSelected == Crafter)
			UnitZ.itemCraftManager.CancelCraft ();
		
		if (CrafterLoader)
			CrafterLoader.SelectCraft (Index, null);
	}
	
	public void ConfirmCraft ()
	{
		// function of craft.
		if (UnitZ.itemCraftManager) {
			if (UnitZ.playerManager && UnitZ.playerManager.PlayingCharacter) {
				UnitZ.itemCraftManager.CraftSelected (Crafter);
				UnitZ.itemCraftManager.Craft (UnitZ.playerManager.PlayingCharacter.inventory);
			}
		}
	}

}
