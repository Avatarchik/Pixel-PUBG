using UnityEngine;
using UnityEditor;
using System.Collections;
 
class EditorCrafter : EditorWindow
{
	private GameObject gameManager;
	private ItemCrafterManager itemCraftManager;
	
	[MenuItem ("Window/UnitZ/Item Crafting")]
	public static void  ShowItemCraftManager ()
	{
		EditorWindow.GetWindow (typeof(EditorCrafter));
	}
	
	void loadData ()
	{
		gameManager = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/UnitZ/Game/GameManager.prefab", typeof(GameObject));
		if (gameManager) {
			itemCraftManager = gameManager.GetComponent<ItemCrafterManager> ();
		}	
	}
	
	void OnEnable ()
	{
		itemNeedAdding = new ItemNeeded[1];
		itemNeedAdding [0] = new ItemNeeded ();
		itemNeedAdding [0].Item = null;
		loadData ();
	}
	
	ItemNeeded[] itemNeedAdding;
	ItemData itemAdding;
	ItemData needAdding;
	Vector2 scrollPos;
	int indexRemoving = -1;
	int indexNeedRemoving = -1;
	int indexShow = -1;

	void OnGUI ()
	{
		titleContent.text = "Item Crafting";
		
		
		
		if (itemCraftManager == null)
			return;
		
		GUI.contentColor = Color.yellow;
		EditorGUILayout.LabelField ("Add Item(ItemData) here. and click 'Add Item'"); 
		
		GUI.contentColor = Color.white;
		itemAdding = (ItemData)EditorGUILayout.ObjectField (itemAdding, typeof(ItemData), true);
		
		if (GUILayout.Button ("Add Item", GUILayout.Width (position.width - 5), GUILayout.Height (30))) {
			if (itemAdding != null) {
				System.Array.Resize (ref itemCraftManager.ItemCraftList, itemCraftManager.ItemCraftList.Length + 1);
				int addingIndex = itemCraftManager.ItemCraftList.Length - 1;
				itemCraftManager.ItemCraftList [addingIndex] = new ItemCrafter ();
				itemCraftManager.ItemCraftList [addingIndex].ItemResult = itemAdding;
				itemCraftManager.ItemCraftList [addingIndex].ItemNeeds = new ItemNeeded[0];
				itemAdding = null;
			}
		}

		
		
		EditorGUILayout.LabelField ("Items"); 
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (position.width), GUILayout.Height (position.height - 90));
					
		for (int i=0; i<itemCraftManager.ItemCraftList.Length; i++) {
			GUI.contentColor = Color.green;

			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			itemCraftManager.ItemCraftList [i].ItemResult = (ItemData)EditorGUILayout.ObjectField (itemCraftManager.ItemCraftList [i].ItemResult, typeof(ItemData), true);
			GUI.contentColor = Color.white;
			
			
			if (indexShow == i) {
				GUI.contentColor = Color.yellow;
				if (GUILayout.Button ("Done", GUILayout.Width (60.0f))) {
					indexShow = -1;
				}
			} else {
				GUI.contentColor = Color.white;
				if (GUILayout.Button ("Edit", GUILayout.Width (60.0f))) {
					indexShow = i;
				}	
			}
			
			if (indexNeedRemoving != -1 && indexShow != -1) {
				if (EditorUtility.DisplayDialog ("Remove Item", "Do you want to remove this item ?", "Remove", "Cancel")) {
					RemoveNeedAt (indexShow, indexNeedRemoving);
					indexNeedRemoving = -1;
				} else {
					indexNeedRemoving = -1;	
				}
			}
			
			if (indexRemoving != -1) {
				if (EditorUtility.DisplayDialog ("Remove Item", "Do you want to remove this item ?", "Remove", "Cancel")) {
					RemoveCraftAt (indexRemoving);
					indexRemoving = -1;
				} else {
					indexRemoving = -1;	
				}
			}

			EditorGUILayout.EndHorizontal ();
			
			
			if (indexShow == i) {
				GUI.contentColor = Color.yellow;
				EditorGUILayout.LabelField ("Add more requirement here"); 
				EditorGUILayout.BeginVertical ();
				
				GUI.contentColor = Color.white;
				EditorGUILayout.BeginHorizontal ();
				needAdding = (ItemData)EditorGUILayout.ObjectField (needAdding, typeof(ItemData), true);
				
				
				if (GUILayout.Button ("Add", GUILayout.Width (80.0f))) {
					if (needAdding != null) {
						System.Array.Resize (ref itemCraftManager.ItemCraftList [indexShow].ItemNeeds, itemCraftManager.ItemCraftList [indexShow].ItemNeeds.Length + 1);
						itemCraftManager.ItemCraftList [indexShow].ItemNeeds [itemCraftManager.ItemCraftList [indexShow].ItemNeeds.Length - 1] = new ItemNeeded ();
						itemCraftManager.ItemCraftList [indexShow].ItemNeeds [itemCraftManager.ItemCraftList [indexShow].ItemNeeds.Length - 1].Item = needAdding;
						needAdding = null;
					}
				}
				
	
				EditorGUILayout.EndHorizontal ();
				EditorGUILayout.LabelField ("Requirement"); 
				
			
				for (int v=0; v<itemCraftManager.ItemCraftList [i].ItemNeeds.Length; v++) {
					EditorGUILayout.BeginHorizontal ();
					itemCraftManager.ItemCraftList [i].ItemNeeds [v].Item = (ItemData)EditorGUILayout.ObjectField (itemCraftManager.ItemCraftList [i].ItemNeeds [v].Item, typeof(ItemData), true);
					GUILayout.FlexibleSpace ();
					EditorGUILayout.LabelField ("Num", GUILayout.Width (30)); 
					int.TryParse (EditorGUILayout.TextField (itemCraftManager.ItemCraftList [i].ItemNeeds [v].Num.ToString (), GUILayout.Width (60)), out itemCraftManager.ItemCraftList [i].ItemNeeds [v].Num);
				
					if (GUILayout.Button ("X", GUILayout.Width (30.0f))) {
						indexNeedRemoving = v;
					}
				
					EditorGUILayout.EndHorizontal ();
				}
				EditorGUILayout.EndVertical ();
				
				
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Crafting Time (second)");
				GUILayout.FlexibleSpace ();
				float.TryParse ((string)EditorGUILayout.TextField (itemCraftManager.ItemCraftList [i].CraftTime.ToString ()), out itemCraftManager.ItemCraftList [i].CraftTime);
				
				EditorGUILayout.EndHorizontal ();
				
				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Result Num");
				GUILayout.FlexibleSpace ();
				int.TryParse ((string)EditorGUILayout.TextField (itemCraftManager.ItemCraftList [i].NumResult.ToString ()), out itemCraftManager.ItemCraftList [i].NumResult);
				
				EditorGUILayout.EndHorizontal ();

				EditorGUILayout.BeginHorizontal ();
				GUILayout.Label ("Craft Type");
				GUILayout.FlexibleSpace ();
				int.TryParse ((string)EditorGUILayout.TextField (itemCraftManager.ItemCraftList [i].ItemType.ToString ()), out itemCraftManager.ItemCraftList [i].ItemType);

				EditorGUILayout.EndHorizontal ();
				
				EditorGUILayout.Separator ();
				if (GUILayout.Button ("Remove Item", GUILayout.Width (100.0f))) {
					indexRemoving = i;
				}
			
				EditorGUILayout.Separator ();
			
			}
		}
             
		EditorGUILayout.EndScrollView ();
		if (GUI.changed) {
			EditorUtility.SetDirty (gameManager);
		}     
	}

	void RemoveNeedAt (int indexshow, int indexneed)
	{
		
		ItemNeeded[] itemNeed = new ItemNeeded[itemCraftManager.ItemCraftList [indexshow].ItemNeeds.Length - 1];
		
		int count = 0;
		for (int i=0; i<itemCraftManager.ItemCraftList [indexshow].ItemNeeds.Length; i++) {
			if (i != indexneed) {
				itemNeed [count] = itemCraftManager.ItemCraftList [indexshow].ItemNeeds [i];
				count++;
			}
		}
		itemCraftManager.ItemCraftList [indexshow].ItemNeeds = (ItemNeeded[])itemNeed.Clone ();
		EditorUtility.SetDirty (gameManager);
	}
	
	void RemoveCraftAt (int indexshow)
	{
		
		ItemCrafter[] itemCraft = new ItemCrafter[itemCraftManager.ItemCraftList.Length - 1];
		
		int count = 0;
		for (int i=0; i<itemCraftManager.ItemCraftList.Length; i++) {
			if (i != indexshow) {
				itemCraft [count] = itemCraftManager.ItemCraftList [i];
				count++;
			}
		}
		
		itemCraftManager.ItemCraftList = (ItemCrafter[])itemCraft.Clone ();
		EditorUtility.SetDirty (gameManager);
	}
    
}
