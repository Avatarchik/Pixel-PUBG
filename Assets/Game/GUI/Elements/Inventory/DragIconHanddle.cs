using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public class DragIconHanddle : MonoBehaviour
{
	public GameObject Owner;
	[HideInInspector]
	public int Type;

	void Start ()
	{

	}

	void LateUpdate ()
	{
		if (MouseLock.MouseLocked || Owner == null || !Owner.activeSelf) {
			UnitZ.Hud.ImageDrag.gameObject.SetActive (false);
		}
	}

}