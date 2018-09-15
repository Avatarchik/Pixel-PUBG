// 摄像机摇晃
using UnityEngine;
using System.Collections;

public class CameraSwing : MonoBehaviour {

	public Vector3 Speed = new Vector3(0,0.01f,0);
	
	void Start () {
	
	}
	

	void Update () {
		this.transform.Rotate(Speed * Mathf.Sin(Time.time)); 
	}
}
