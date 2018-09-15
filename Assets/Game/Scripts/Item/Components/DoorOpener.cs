using UnityEngine;
using System.Collections;


public class DoorOpener : ObjectTrigger
{
	public DoorFrame Door;

	void Start ()
	{
	}
	
	public override void Pickup (CharacterSystem character)
	{
		if(Door){
			Door.Access(character);	
		}
		base.Pickup (character);
	}

}
