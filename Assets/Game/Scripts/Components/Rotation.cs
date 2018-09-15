
using UnityEngine;
using System.Collections;

public class Rotation : MonoBehaviour {

	public Vector3 Axis = Vector3.up;
	void Start () {
	
	}

	void Update () {
		this.transform.Rotate(Axis * Time.deltaTime);
	}
}
