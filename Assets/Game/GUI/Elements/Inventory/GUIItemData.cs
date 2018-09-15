using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class GUIItemData : MonoBehaviour {

	public Image Icon;
	public Text Name;
	public Text Num;
	public ItemData Item;
	
	void Start () {
		if(Icon)
			Icon.enabled = false;
		if(Name)
			Name.enabled = false;
		if(Num)
			Num.enabled = false;
	}
	
	void Update () {
		// just update a GUI elements
		if(Item){
			if(Icon){
				Icon.enabled = true;
				Icon.sprite = Item.ImageSprite;
			}
			if(Name){
				Name.enabled = true;
				Name.text = Item.ItemName; 
			}
			if(Num){
				Num.enabled = true;
				
			}
			
		}
	}
}
