using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ItemDropAfterDead : MonoBehaviour
{
	public float DropRate = 1;
	public ItemData[] Item;
	public int[] DropNum;

	public Vector3 Offset = new Vector3 (0, 0.1f, 0);

	void Start ()
	{
		if (DropNum.Length < Item.Length) {
			DropNum = new int[Item.Length];
			for (int i = 0; i < DropNum.Length; i++) {
				if (DropNum [i] <= 0)
					DropNum [i] = 1;
			}
		}
	}


	public void DropItem ()
	{
		if (Random.Range (0, 100) >= 99 * (1 - DropRate)) {
			int index = Random.Range (0, Item.Length);
			ItemData itemPick = Item [index];
			Vector3 spawnPoint = DetectGround (transform.position + new Vector3 (Random.Range (-(int)(this.transform.localScale.x / 2.0f), (int)(this.transform.localScale.x / 2.0f)), 0, Random.Range ((int)(-this.transform.localScale.z / 2.0f), (int)(this.transform.localScale.z / 2.0f))));
			UnitZ.gameNetwork.RequestSpawnItem (itemPick.gameObject, itemPick.NumTag, DropNum [index], spawnPoint, Quaternion.identity);
		}
	}

	Vector3 DetectGround (Vector3 position)
	{
		RaycastHit hit;
		if (Physics.Raycast (position, -Vector3.up, out hit, 1000.0f)) {
			return hit.point + Offset;
		}
		return position;
	}
}
