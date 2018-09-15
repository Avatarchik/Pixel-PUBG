using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using System.Reflection;
 
class EditorCharacters : EditorWindow
{
	private GameObject gameManager;
	private CharacterManager characterManager;
	
	[MenuItem ("Window/UnitZ/Character Manager")]
	public static void  ShowItemManager ()
	{
		EditorWindow.GetWindow (typeof(EditorCharacters));
	}
	
	void loadData ()
	{
		gameManager = (GameObject)AssetDatabase.LoadAssetAtPath ("Assets/UnitZ/Game/GameManager.prefab", typeof(GameObject));
		if (gameManager) {
			characterManager = gameManager.GetComponent<CharacterManager> ();
		}	
	}
	
	void OnEnable ()
	{
		loadData ();
	}

	CharacterSystem characterAdding;
	Texture2D Image;
	string Name;
	string Description;
	Vector2 scrollPos;
	int indexRemoving = -1;

	void OnGUI ()
	{
		titleContent.text = "Character";
		
		if (characterManager == null)
			return;
		
		GUI.contentColor = Color.yellow;
		EditorGUILayout.LabelField ("Add (CharacterSystem) here. and click 'Add Character'"); 
		
		
		GUI.contentColor = Color.white;
		
		
		EditorGUILayout.BeginHorizontal ();
		GUILayout.Label ("Name");
		Name = EditorGUILayout.TextField (Name);
		EditorGUILayout.EndHorizontal ();
		
		
		GUILayout.Label ("Description");
		Description = EditorGUILayout.TextArea (Description);
		
		
		
		Image = (Texture2D)EditorGUILayout.ObjectField (Image, typeof(Texture2D), true);
		characterAdding = (CharacterSystem)EditorGUILayout.ObjectField (characterAdding, typeof(CharacterSystem), true);
		if (GUILayout.Button ("Add Character", GUILayout.Width (position.width - 5), GUILayout.Height (30))) {
			if (characterAdding != null) {
				System.Array.Resize (ref characterManager.CharacterPresets, characterManager.CharacterPresets.Length + 1);
				characterManager.CharacterPresets [characterManager.CharacterPresets.Length - 1] = new CharacterPreset ();
				characterManager.CharacterPresets [characterManager.CharacterPresets.Length - 1].CharacterPrefab = characterAdding;
				characterManager.CharacterPresets [characterManager.CharacterPresets.Length - 1].Icon = Image;
				characterManager.CharacterPresets [characterManager.CharacterPresets.Length - 1].Name = Name;
				characterManager.CharacterPresets [characterManager.CharacterPresets.Length - 1].Description = Description;
				characterAdding = null;
			}
		}
		
		
		
		EditorGUILayout.LabelField ("Items"); 
		scrollPos = EditorGUILayout.BeginScrollView (scrollPos, GUILayout.Width (position.width), GUILayout.Height (position.height - 165));
					
		for (int i=0; i<characterManager.CharacterPresets.Length; i++) {
			
			
			GUI.contentColor = Color.green;

			EditorGUILayout.Separator ();
			EditorGUILayout.BeginHorizontal ();
			characterManager.CharacterPresets [i].CharacterPrefab = (CharacterSystem)EditorGUILayout.ObjectField (characterManager.CharacterPresets [i].CharacterPrefab, typeof(CharacterSystem), true);
			GUI.contentColor = Color.white;
			if (GUILayout.Button ("Remove", GUILayout.Width (60.0f))) {
				indexRemoving = i;
			}
			EditorGUILayout.EndHorizontal ();
			
			
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label ("Name");
			characterManager.CharacterPresets [i].Name = EditorGUILayout.TextField (characterManager.CharacterPresets [i].Name);
			EditorGUILayout.EndHorizontal ();
			
			
			
			GUILayout.Label ("Description");
			characterManager.CharacterPresets [i].Description = EditorGUILayout.TextArea (characterManager.CharacterPresets [i].Description);
			
			
			
			characterManager.CharacterPresets [i].Icon = (Texture2D)EditorGUILayout.ObjectField (characterManager.CharacterPresets [i].Icon, typeof(Texture2D), true);
			EditorGUILayout.BeginHorizontal ();
			GUILayout.Label (characterManager.CharacterPresets [i].Icon, GUILayout.Width (300), GUILayout.Height (100));
			EditorGUILayout.EndHorizontal ();
			
			if (indexRemoving != -1) {
				if (EditorUtility.DisplayDialog ("Remove Item", "Do you want to remove this character ?", "Remove", "Cancel")) {
					RemoveCharacterAt (indexRemoving);
					indexRemoving = -1;
				} else {
					indexRemoving = -1;	
				}
			}

		}
             
		EditorGUILayout.EndScrollView ();
		if (GUI.changed) {
			EditorUtility.SetDirty (gameManager);
		}
             
	}

	void RemoveCharacterAt (int index)
	{
		CharacterPreset[] characterPresets = new CharacterPreset[characterManager.CharacterPresets.Length - 1];
		int count = 0;
		for (int i=0; i<characterManager.CharacterPresets.Length; i++) {
			if (i != index) {
				characterPresets [count] = characterManager.CharacterPresets [i];
				count++;
			}
			
		}
		
		characterManager.CharacterPresets = (CharacterPreset[])characterPresets.Clone ();
		
	}
    
}

