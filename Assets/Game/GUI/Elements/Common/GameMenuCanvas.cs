using UnityEngine;
using System.Collections;

public class GameMenuCanvas : MonoBehaviour
{
	void Start ()
	{
		
	}
	
	// Resume funtion
	public void Resume ()
	{
		MouseLock.MouseLocked = true;
		this.gameObject.SetActive (false);
	}
	
	// Quit game function
	public void Disconnect ()
	{
		if (UnitZ.gameManager)
			UnitZ.gameManager.QuitGame ();
		
		MouseLock.MouseLocked = false;
		this.gameObject.SetActive (false);
	}

}
