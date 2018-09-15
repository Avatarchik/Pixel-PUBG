using UnityEngine;
using System.Collections;

public class FuelApplying : MonoBehaviour {

	public float FuelPlus = 100;
	public float MinDistance = 2;

	void Start () {
		CarControl[] carAround = (CarControl[])GameObject.FindObjectsOfType(typeof(CarControl));
		float mindistance = MinDistance;
		int carselect = -1;
		for(int i=0;i<carAround.Length;i++){
			if(carAround[i]!=null){
				float distance = Vector3.Distance(this.transform.position,carAround[i].transform.localPosition);
				if(distance < mindistance){
					mindistance = distance;
					carselect = i;
				}
			}
		}
		if(carAround.Length > carselect && carAround[carselect]!=null){
			carAround[carselect].SendMessage("ApplyFuel",FuelPlus,SendMessageOptions.DontRequireReceiver);
		}

		Destroy(this.gameObject);
	}

}
