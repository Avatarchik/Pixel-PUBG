using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class Projectile : DamageBase
{
	
	public float Duration = 3;
	public GameObject Spawn;
	private float timeTemp;


	void PositionUpdate (Vector3 position, Quaternion rotation, int id)
	{
		OwnerID = id;
		this.transform.position = Vector3.Lerp (this.transform.position, position, 1);
		this.transform.rotation = Quaternion.Lerp (this.transform.rotation, rotation, 1);
	}

	private bool isQuitting = false;
	void OnApplicationQuit ()
	{ 
		isQuitting = true; 
	}
	
	void OnDestroy ()
	{
		if (!isQuitting && Spawn) {
			GameObject fx = (GameObject)GameObject.Instantiate (Spawn, this.transform.position, this.transform.rotation);
			DamageBase dm = fx.GetComponent<DamageBase> ();
			if (dm) {
				dm.OwnerID = OwnerID;
				dm.OwnerTeam = OwnerTeam;
			}
		}
	}
	
	void Start ()
	{
		timeTemp = Time.time;
	}
	
	void Update ()
	{
		if (Time.time >= timeTemp + Duration) {
			OnDead ();
		}
	}
	

	void OnDead ()
	{
		NetworkServer.Destroy (this.gameObject);
	}
}
