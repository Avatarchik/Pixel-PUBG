
using UnityEngine;
using System.Collections;
using System;

[RequireComponent (typeof(AudioSource))]
public class ObjectSpawn : MonoBehaviour
{
	public string ItemID = "";
	public string ItemUID = "";
	public GameObject Item;
	public int Group = 2;

	void Start ()
	{
		if (Item) {
			GameObject item = (GameObject)GameObject.Instantiate (Item, this.transform.position, this.transform.rotation);
			item.SendMessage ("SetItemID", ItemID, SendMessageOptions.DontRequireReceiver);
			item.SendMessage ("SetItemUID", ItemUID, SendMessageOptions.DontRequireReceiver);
				
		}
		Destroy (this.gameObject);
	}

	public void SetItemID (string id)
	{
		ItemID = id;
	}

	public void SetItemUID (string uid)
	{
		ItemUID = uid;
	}

	public void GenItemUID ()
	{
		ItemUID = GetUniqueID ();
	}

	void Update ()
	{
	
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
