using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//角色存活
[RequireComponent(typeof(CharacterSystem))]
public class CharacterLiving : NetworkBehaviour
{
	
	[SyncVar]
	public byte Hungry = 100;
	public byte HungryMax = 100;
	[SyncVar]
	public byte Water = 100;
	public byte WaterMax = 100;

	[HideInInspector]
	public CharacterSystem character;
	[HideInInspector]
	
	void Start ()
	{
		character = this.GetComponent<CharacterSystem> ();
		
		InvokeRepeating ("stomachUpdate", 1.0f, 1.0f);
		InvokeRepeating ("hungryUpdate", 1.0f, 15.0f);
		InvokeRepeating ("thirstilyUpdate", 1.0f, 10.0f);
		
	}

	public void Respawn(){
		Hungry = HungryMax;
		Water = WaterMax;
	}

	void Update ()
	{
		if (Hungry < 0)
			Hungry = 0;
			
		if (Hungry > HungryMax)
			Hungry = HungryMax;
		
		if (Water < 0)
			Water = 0;
			
		if (Water > WaterMax)
			Water = WaterMax;
	}
	
	public void stomachUpdate ()
	{
		if (isServer) {
			if (character == null || (character && !character.IsAlive))
				return;
		
			if (Water <= 0) {	
				character.ApplyDamage (2, Vector3.up, character.NetID, 0);	
			}
			if (Hungry <= 0) {
			
				character.ApplyDamage (1, Vector3.up, character.NetID, 0);		
			}
		}
	}
	
	public void hungryUpdate ()
	{
		if (isServer) {
			Hungry -= 1;
		}
	}
	
	public void thirstilyUpdate ()
	{
		if (isServer) {
			Water -= 1;
		}
	}
	
	[Command]
	void CmdEatUpdate (byte num)
	{
		Hungry += num;
	}

	public void Eating (byte num)
	{
		CmdEatUpdate(num);
	}
	
	[Command]
	void CmdDrinkUpdate (byte num)
	{
		Water += num;
	}


	public void Drinking (byte num)
	{
		CmdDrinkUpdate(num);
	}
	
	[Command]
	void CmdHealthUpdate (byte num)
	{
		if (character == null)
			return;
		
		character.HP += num;
	}

	public void Healing (byte num)
	{
		if (character == null)
			return;

		CmdHealthUpdate(num);
	}
}
