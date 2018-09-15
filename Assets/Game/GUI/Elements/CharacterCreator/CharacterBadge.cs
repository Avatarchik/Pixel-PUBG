using UnityEngine;
using UnityEngine.UI;
using System.Collections;



public class CharacterBadge : MonoBehaviour {
	// gui elements, need to assign them to these parameter.
	public RawImage GUIImage;
	public Text GUIName;
	public Text GUIType;
	
	[HideInInspector]
	public CharacterSaveData CharacterData;
	[HideInInspector]
	public int Index;
	[HideInInspector]
	public CharacterCreatorCanvas CharacterCreatorS;

	
	void Start () {

	}
	
	// Remove character funtion
	public void Delete(){
		if(CharacterCreatorS)
			CharacterCreatorS.RemoveCharacter(Index);
	}
	
	// Select character function
	public void PlayThisCharacter(){
		if(CharacterCreatorS && CharacterData.PlayerName!="")
			CharacterCreatorS.SelectCharacter(CharacterData);
	}


}
