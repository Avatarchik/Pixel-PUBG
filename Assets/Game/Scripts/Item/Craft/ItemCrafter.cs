using UnityEngine;
using System.Collections;

[System.Serializable]
public class ItemCrafter {

	public ItemData ItemResult;
	public int NumResult = 1;
	public ItemNeeded[] ItemNeeds;
	public float CraftTime = 2;
	public float CraftTimeTemp;
	public bool StartBuild;
	public int ItemType;
	
}

[System.Serializable]
public class ItemNeeded{
	public ItemData Item;
	public int Num;
}