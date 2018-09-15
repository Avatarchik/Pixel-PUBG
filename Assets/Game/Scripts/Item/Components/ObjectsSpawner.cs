using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;


[RequireComponent (typeof(NetworkIdentity))]

public class ObjectsSpawner : NetworkBehaviour
{

	public bool SpawnOnStart = true;
	public float timeSpawn = 120;
	public GameObject[] Obj;
	public int ObjMax = 3;
	public Vector3 Offset = new Vector3 (0, 0.1f, 0);
	public bool PlaceOnGround = true;
	private float timeTemp = 0;
	private List<GameObject> itemList = new List<GameObject> ();

	void Start ()
	{
		if (SpawnOnStart)
			Spawn ();
	}

	void Spawn ()
	{
		if (!isServer)
			return;
		
		ObjectExistCheck ();
		if (ObjectsNumber < ObjMax) {
			if (Obj.Length > 0) {
				GameObject itemPick = Obj [Random.Range (0, Obj.Length)];
				Vector3 spawnPoint = DetectGround (transform.position + new Vector3 (Random.Range (-(int)(this.transform.localScale.x / 2.0f), (int)(this.transform.localScale.x / 2.0f)), 0, Random.Range ((int)(-this.transform.localScale.z / 2.0f), (int)(this.transform.localScale.z / 2.0f))));
				GameObject objitem = UnitZ.gameNetwork.RequestSpawnObject (itemPick.gameObject, spawnPoint, Quaternion.identity);

				if (objitem)
					itemList.Add (objitem);
			}
			timeTemp = Time.time;
		}
	}

	private int ObjectsNumber;

	void ObjectExistCheck ()
	{
		ObjectsNumber = 0;
		foreach (var obj in itemList) {
			if (obj != null)
				ObjectsNumber++;
		}
	}

	void Update ()
	{
		if (!isServer)
			return;
		
		if (Time.time > timeTemp + timeSpawn) {
			Spawn ();
		}
	}

	void OnDrawGizmos ()
	{
		Gizmos.color = Color.blue;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawWireCube (transform.position, this.transform.localScale);
	}

	Vector3 DetectGround (Vector3 position)
	{
		if (PlaceOnGround) {
			RaycastHit hit;
			if (Physics.Raycast (position, -Vector3.up, out hit, 1000.0f)) {
				return hit.point + Offset;
			}
		}
		return position;
	}
}
