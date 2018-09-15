using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
 
class EditorItems : EditorWindow
{
	private GameObject gameManager;
	private ItemManager itemManager;
	
	[MenuItem ("Window/UnitZ/Item Manager")]
	public static void  ShowItemManager ()
	{
		EditorWindow.GetWindow (typeof(EditorItems));
	}
	
	void loadData ()
	{
		gameManager = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/UnitZ/Game/GameManager.prefab", typeof(GameObject));
		if (gameManager) {
			itemManager = gameManager.GetComponent<ItemManager> ();
		}	
	}
	
	void OnEnable () {
		loadData ();
	}


	ItemData itemAdding;
	Vector2 scrollPos;
	int indexRemoving = -1;

	void OnGUI ()
	{
		titleContent.text = "Item Manager";
		
		if (itemManager == null)
			return;
		
		GUI.contentColor = Color.yellow;
		EditorGUILayout.LabelField("Add Item(ItemData) here. and click 'Add Item'"); 
		
		GUI.contentColor = Color.white;
		itemAdding = (ItemData)EditorGUILayout.ObjectField (itemAdding, typeof(ItemData), true);
		if (GUILayout.Button ("Add Item", GUILayout.Width (position.width - 5), GUILayout.Height (30))) {
			if (itemAdding != null) {
				System.Array.Resize (ref itemManager.ItemsList, itemManager.ItemsList.Length + 1);
				itemManager.ItemsList [itemManager.ItemsList.Length - 1] = itemAdding;
				itemAdding = null;
			}
		}
		EditorGUILayout.LabelField("Items"); 
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (position.width), GUILayout.Height (position.height - 90));
					
		for (int i=0; i<itemManager.ItemsList.Length; i++) {
			GUI.contentColor = Color.green;

			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			itemManager.ItemsList [i] = (ItemData)EditorGUILayout.ObjectField (itemManager.ItemsList [i], typeof(ItemData), true);
			GUI.contentColor = Color.white;
			if (GUILayout.Button ("Remove", GUILayout.Width (60.0f))) {
				indexRemoving = i;
			}
			
			if (indexRemoving != -1) {
				if (EditorUtility.DisplayDialog ("Remove Item","Do you want to remove this item ?", "Remove", "Cancel")) {
					RemoveItemAt (indexRemoving);
					indexRemoving = -1;
				}else{
					indexRemoving = -1;	
				}
			}

			EditorGUILayout.EndHorizontal ();
		}
             
		EditorGUILayout.EndScrollView ();
		if (GUI.changed) {
			EditorUtility.SetDirty (gameManager);
		}
             
	}

	void RemoveItemAt (int index)
	{
		ItemData[] itemData = new ItemData[itemManager.ItemsList.Length - 1];
		int count = 0;
		for (int i=0; i<itemManager.ItemsList.Length; i++) {
			if (i != index) {
				itemData [count] = itemManager.ItemsList [i];
				count++;
			}
			
		}
		
		itemManager.ItemsList = (ItemData[])itemData.Clone ();
		
	}
    
}

