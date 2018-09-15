using UnityEngine;
using System.Collections;

public class LinkActive : ItemData
{

	public string Link = "http://www.hardworkerstudio.com";

	public override void Pickup (CharacterSystem character)
	{
		Application.OpenURL (Link);
		base.Pickup (character);
	}
	
}
