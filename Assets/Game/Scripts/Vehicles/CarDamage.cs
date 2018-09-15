using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//汽车伤害
public class CarDamage : MonoBehaviour {

	public AudioClip ClashSound;
	public byte ClashDamage;
	public Car Root;
	public float VelocityThreshold = 3;


	void Start () {
		
	}
	
	void OnTriggerEnter (Collider collider)
	{
		Rigidbody rig = Root.GetComponent<Rigidbody> ();
		if (!rig)
			return;
		
		if (Root.Audiosource && ClashSound && rig.velocity.magnitude > VelocityThreshold) {
			Root.Audiosource.PlayOneShot (ClashSound);
		}

		DamagePackage dm = new DamagePackage ();
		dm.Damage = (byte)(ClashDamage * rig.velocity.magnitude);
		dm.Normal = rig.velocity.normalized;
		dm.Direction = rig.velocity * 2;
		dm.Position = this.transform.position;


		dm.ID = Root.Seats [0].PassengerID;
		dm.Team = 3;
		dm.DamageType = 0;
		//Debug.Log ("Hit " + collider.name);
		collider.gameObject.transform.root.SendMessage ("DirectDamage", dm, SendMessageOptions.DontRequireReceiver);

	}
}
