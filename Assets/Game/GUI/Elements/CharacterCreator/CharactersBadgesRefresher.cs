using UnityEngine;
using System.Collections;

public class CharactersBadgesRefresher : MonoBehaviour {

	public CharacterCreatorCanvas characterCreator;

	void Start () {
	
	}
	
	// keep update all characters data. 
	// everytime when the character panel is open.
	
	void OnEnable(){
		if(characterCreator)
			StartCoroutine (characterCreator.LoadCharacters());
	}

}
