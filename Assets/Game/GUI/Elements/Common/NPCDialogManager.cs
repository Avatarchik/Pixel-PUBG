using UnityEngine;
using System.Collections;

public class NPCDialogManager : MonoBehaviour {

	public GUIPlayerItemLoader PlayerItemLoader;
	public GUIIStockItemLoader SecondItemLoader;
	public PlayerHUDCanvas HUD;
	
	void Start () {
		HUD = (PlayerHUDCanvas)GameObject.FindObjectOfType(typeof(PlayerHUDCanvas));
		if(HUD){
			HUD.OpenPanelByName("InventoryStock");	
		}
	}
	
	public void OpenStock(ItemStocker stock){
		SecondItemLoader.OpenInventory(stock.inventory,"Stock");
		if(HUD)
		HUD.OpenPanelByName("InventoryStock");	
	}
	
	public void CloseStock(){
		if(HUD)
		HUD.ClosePanelByName("InventoryStock");	
	}
	
}
