using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class DamageBase : NetworkBehaviour {
	[SyncVar]
	public int OwnerID;
	[SyncVar]
	public byte OwnerTeam;

}
