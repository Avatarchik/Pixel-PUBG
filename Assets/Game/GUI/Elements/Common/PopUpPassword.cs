using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PopUpPassword : MonoBehaviour {

	public InputField ServerNameText;
	[System.NonSerialized]
	public string Password;
	private Popup popup;

	void Start () {
		popup = (Popup)GameObject.FindObjectOfType(typeof(Popup));
	}

	void OnEnable(){
		Password = "";
		if (ServerNameText)
			ServerNameText.text = Password;
		
	}

	public void SetPassword (InputField num)
	{
		Password = num.text;
	}

	public void Close(){
		popup.Option2 ();
	}

	public void Ok(){
		popup.Option1 ();
	}
}
