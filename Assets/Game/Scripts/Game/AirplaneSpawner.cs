using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif
using System.Collections;

//生成飞机
public class AirplaneSpawner : MonoBehaviour
{
    public Vector3 Offset = Vector3.up;

    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.blue;
        Gizmos.DrawWireSphere(transform.position, this.transform.localScale.x);
        Gizmos.DrawSphere(transform.position, 2);
        Handles.Label(transform.position, "Air Spawner");
#endif
    }

    public Vector3 SpawnPoint()
    {
        Vector2 pos = RandomOnUnitCircle2(this.transform.localScale.x);
        return this.transform.position + new Vector3(pos.x, 0, pos.y) + Offset;
    }

    public static Vector2 RandomOnUnitCircle2(float radius)
    {
        Vector2 randomPointOnCircle = Random.insideUnitCircle;
        randomPointOnCircle.Normalize();
        randomPointOnCircle *= radius;
        return randomPointOnCircle;
    }

}
