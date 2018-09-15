using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class ConnectingEvent : MonoBehaviour
{

	public Text TextInfo;
	public float Timeout = 10;
	public GameObject Preloading;
	private float timeTemp;
	private bool conneted;
	
	void OnEnable ()
	{
		if(Preloading)
			Preloading.SetActive(true);
		conneted = false;
		timeTemp = Time.time;	
		if (TextInfo)
			TextInfo.text = "Connecting to server";
	}
	
	void Start ()
	{
	}
	
	void Update ()
	{
		if(!conneted){
			if (Time.time >= Timeout + timeTemp) {
				if (TextInfo)
					TextInfo.text = "Connecting Time out";
				
				if(Preloading)
					Preloading.SetActive(false);
			}
		}
	}
}
