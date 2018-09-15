using UnityEngine;
using System.Collections;

//GUI玩家物品加载
public class GUIPlayerItemLoader : GUIItemLoader {

	public DropStockArea dropArea;
	
	void Start () {
		dropArea = this.GetComponent<DropStockArea>();
	}
	
	void Update () {
		if (UnitZ.playerManager == null || UnitZ.playerManager.PlayingCharacter == null)
			return;
		
		currentInventory = UnitZ.playerManager.PlayingCharacter.inventory;
		UpdateFunction();

	}
}
