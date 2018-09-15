using UnityEngine;
using System.Collections;

public class Ladder : MonoBehaviour {

	void Start () {
	
	}

	void OnTriggerStay(Collider player) {
		
		FPSController fpsController = player.GetComponent<FPSController>();
		
        if(fpsController){
			fpsController.Climb(fpsController.inputDirection.z);
		}
		
    }
		
		
}
