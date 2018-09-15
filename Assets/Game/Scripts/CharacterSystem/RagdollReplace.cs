using UnityEngine;
using System.Collections;

public class RagdollReplace : MonoBehaviour {

	public float DisableDelay = 5;
	public GameObject RootRagdoll;
	float timeTemp = 0;
	
	
	void Start () {
		timeTemp = Time.time;
		if(RootRagdoll == null){
			FindRoot (this.gameObject);
		}
	}
	
	void FindRoot(GameObject dst){
		foreach (Transform child in dst.transform) {
			if(child.GetComponent<Rigidbody>()){
				RootRagdoll = child.gameObject;
				return;
			}
			FindRoot (child.gameObject);
		}	
	}
	
	void FindRigidBody(GameObject dst){
		
		if(dst.GetComponent<Rigidbody>()){
			dst.GetComponent<Rigidbody>().isKinematic = true;
		}
		if(dst.GetComponent<Collider>()){
			Destroy(dst.GetComponent<Collider>());
		}
		
		foreach (Transform child in dst.transform) {
			FindRigidBody (child.gameObject);
		}	
	}

	void Update () {
		
		if(Time.time >= timeTemp + DisableDelay && (RootRagdoll == null || (RootRagdoll && RootRagdoll.GetComponent<Rigidbody>().velocity.sqrMagnitude <= 0.01f))){
			
			FindRigidBody(this.gameObject);
			Destroy(this);
		}
	}
	
	void FixedUpdate(){
		
	}
}
