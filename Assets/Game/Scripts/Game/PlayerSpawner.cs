using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

//玩家生成
public class PlayerSpawner : MonoBehaviour
{
	public Vector3 Offset = Vector3.up;
	
	void OnDrawGizmos ()
	{
		#if UNITY_EDITOR
		Gizmos.color = Color.green;
		Gizmos.DrawSphere (transform.position, 0.2f);
		Gizmos.DrawWireCube (transform.position, this.transform.localScale);
		Handles.Label(transform.position, "Player Spawner");
		#endif
	}
	
	public Vector3 SpawnPoint ()
	{
		return DetectGround (this.transform.position + new Vector3 (Random.Range (-(int)(this.transform.localScale.x / 2.0f), (int)(this.transform.localScale.x / 2.0f)), 0, Random.Range (-(int)(this.transform.localScale.z / 2.0f), (int)(this.transform.localScale.z / 2.0f))));	
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
