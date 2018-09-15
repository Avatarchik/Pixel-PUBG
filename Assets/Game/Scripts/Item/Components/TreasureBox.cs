using UnityEngine;
using System.Collections;

public class TreasureBox : DamageManager
{
	
	public float DropRate = 1;
	public ItemData[] Item;
	public Vector3 Offset = new Vector3 (0, 0.1f, 0);

	public override void OnThisThingDead ()
	{
		Spawn ();
		base.OnThisThingDead ();
	}

	void Spawn ()
	{
		if (!isServer)
			return;

		if (Random.Range (0, 100) >= 99 * (1 - DropRate)) {

			if (Item.Length > 0) {
				ItemData itemPick = Item [Random.Range (0, Item.Length)];
				Vector3 spawnPoint = DetectGround (transform.position + Vector3.up);
				UnitZ.gameNetwork.RequestSpawnObject (itemPick.gameObject, spawnPoint, Quaternion.identity);
			}
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
