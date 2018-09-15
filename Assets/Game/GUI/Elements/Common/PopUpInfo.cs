using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PopUpInfo : MonoBehaviour {

	public Text ContentText;
	private Popup popup;
	void Start () {
		popup = (Popup)GameObject.FindObjectOfType(typeof(Popup));
	}
	
	public void Close(){
		popup.Option2 ();
		this.gameObject.SetActive (false);
	}
	public void Ok(){
		popup.Option1 ();
		this.gameObject.SetActive (false);
	}

}
