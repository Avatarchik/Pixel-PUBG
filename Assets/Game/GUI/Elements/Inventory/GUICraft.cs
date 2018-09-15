using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUICraft : MonoBehaviour
{
	// gui elements, need to assign them to these parameter.
	public Text Name;
	public Image Icon;
	public ItemCrafter Crafter;
	public int Index;
	
	public GUICraftListLoader CrafterLoader;
	
	void Start ()
	{
		if (Icon)
			Icon.enabled = false;
		if (Name)
			Name.enabled = false;
	}

	void Update ()
	{
		// update GUI elements	
		if (Crafter != null && Crafter.ItemResult != null) {
			if (Icon != null && Crafter.ItemResult.ImageSprite != null) {
				Icon.sprite = Crafter.ItemResult.ImageSprite;
				Icon.enabled = true;
			}
			
			if (Name != null) {
				Name.text = Crafter.ItemResult.ItemName;
				Name.enabled = true;
			}
		}
	}
	
	public	void Craft ()
	{
		if (CrafterLoader) {
			CrafterLoader.SelectCraft (Index, Crafter);
		}
	}
}
