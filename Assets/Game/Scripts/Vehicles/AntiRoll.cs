using UnityEngine;
using System.Collections;


public class AntiRoll : MonoBehaviour
{

	public WheelCollider WheelL;
	public WheelCollider WheelR;
	public float antiRoll = 5000.0f;

	void FixedUpdate ()
	{
		WheelHit hit;
		float travelL = 1.0f;
		float travelR = 1.0f;
		bool groundedL = WheelL.GetGroundHit (out hit);
	
		if (groundedL)
			travelL = (-WheelL.transform.InverseTransformPoint (hit.point).y - WheelL.radius) / WheelL.suspensionDistance;
		var groundedR = WheelR.GetGroundHit (out hit);
		if (groundedR)
			travelR = (-WheelR.transform.InverseTransformPoint (hit.point).y - WheelR.radius) / WheelR.suspensionDistance;
		var antiRollForce = (travelL - travelR) * antiRoll;
		if (groundedL)
			GetComponent<Rigidbody>().AddForceAtPosition (WheelL.transform.up * -antiRollForce,
		WheelL.transform.position); 
		if (groundedR)
			GetComponent<Rigidbody>().AddForceAtPosition (WheelR.transform.up * antiRollForce,
		WheelR.transform.position); 
	}
}
