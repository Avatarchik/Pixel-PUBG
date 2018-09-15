using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUILevelPresetSelector : MonoBehaviour
{
	public LevelPreset Level;
	public RawImage Icon;
	public Text Name;
	public Text Detail;
	
	void Start ()
	{
		// just update a GUI elements
		if (Level != null) {
			if (Level.Icon)
				Icon.texture = Level.Icon;
			if (Name)
				Name.text = Level.LevelName;
			if (Detail)
				Detail.text = Level.Detail;
		}
	}
	
	public void Select ()
	{
		// if this preset was selected, so setup a starting level on menu.
		MainMenuManager menu = (MainMenuManager)GameObject.FindObjectOfType (typeof(MainMenuManager));
		if (menu != null && Level != null) {
			menu.LevelSelected(Level.SceneName);
			menu.OpenPreviousPanel ();
		}
	}

}

[System.Serializable]
public class LevelPreset
{
	public string LevelName;
	public string Detail;
	public string SceneName;
	public Texture2D Icon;
}

