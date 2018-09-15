
using System.Collections;
using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;


public class Popup : MonoBehaviour
{
    public static Popup Pop;
    public PopUpInfo PopupObject;
	public PopUpPassword PopupPasswordObject;

    private void Awake()
    {
        Pop = this;
    }
    void Start ()
	{
        if (PopupObject)
			PopupObject.gameObject.SetActive (false);
		
		if (PopupPasswordObject)
			PopupPasswordObject.gameObject.SetActive (false);
		
	}

	public void Option1 ()
	{
		if (callBack1Tmp != null)
			callBack1Tmp ();

		callBack1Tmp = null;
		callBack2Tmp = null;
	}

	public void Option2 ()
	{
		if (callBack2Tmp != null)
			callBack2Tmp ();

		callBack1Tmp = null;
		callBack2Tmp = null;
	}

	public void ShowPopup (string text)
	{
		if (PopupObject) {
			PopupObject.gameObject.SetActive (true);
			PopupObject.ContentText.text = text;
		}
	}

	public void ShowPopupPassword ()
	{
		Debug.Log ("Showpassword");
		if (PopupPasswordObject) {
			PopupPasswordObject.gameObject.SetActive (true);
		}
	}

	Action callBack1Tmp;
	Action callBack2Tmp;


	public void Asking (string header, Action callback1 = null, Action callback2 = null)
	{
		ShowPopup (header);

		callBack1Tmp = callback1;
		callBack2Tmp = callback2;
	}

	public void AskingPassword (string header, Action callback1 = null, Action callback2 = null)
	{
		ShowPopupPassword ();
		callBack1Tmp = callback1;
		callBack2Tmp = callback2;
	}

}
