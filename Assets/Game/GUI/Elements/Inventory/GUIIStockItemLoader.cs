using UnityEngine;
using System.Collections;

public class GUIIStockItemLoader : GUIItemLoader {

	public DropStockArea dropArea;
	
	void Start () {
		dropArea = this.GetComponent<DropStockArea>();
	}
	
	
	public void OpenInventory(CharacterInventory inventory,string type){
		currentInventory = inventory;
		Type = type;
		UpdateGUIInventory ();
	}

	void Update ()
	{
		if (currentInventory == null)
			return;

		UpdateFunction();
	}
}
