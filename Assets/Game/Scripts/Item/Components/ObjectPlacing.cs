
using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System;

[RequireComponent(typeof(NetworkIdentity))]
//[RequireComponent(typeof(NetworkTransform))]

public class ObjectPlacing : NetworkBehaviour {
	[SyncVar]
	public string ItemID = "";
	[SyncVar]
	public string ItemData = "";
	[SyncVar]
	public string ItemUID = "";
	
	public void Start ()
	{

	}

	public override void OnStartClient ()
	{
		if(isServer){
			RpcTellAllItemData(ItemID,ItemData,ItemUID);
		}
		base.OnStartClient ();
	}

	[ClientRpc (channel=2)]
	void RpcTellAllItemData (string itemID,string itemData,string itemUID){
		ItemID = itemID;
		ItemData = itemData;
		ItemUID = itemUID;
		this.SendMessage("OnPlacingReady",SendMessageOptions.DontRequireReceiver);
	}
	
	public void SetItemID(string id){
		ItemID = id;
	}
	public void SetItemUID(string uid){
		ItemUID = uid;
	}
	public void SetItemData(string data){
		ItemData = data;	
	}

	
	public string GetUniqueID ()
	{
		var random = new System.Random ();   
		DateTime epochStart = new System.DateTime (1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		string uniqueID = String.Format ("{0:X}", Convert.ToInt32 (timestamp))
                 + "-" + String.Format ("{0:X}", Convert.ToInt32 (Time.time * 1000000))
                 + "-" + String.Format ("{0:X}", random.Next (1000000000));
		return uniqueID;
	}

}
