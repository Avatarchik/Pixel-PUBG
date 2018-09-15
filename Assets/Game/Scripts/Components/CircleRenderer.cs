using UnityEngine;
using System.Collections;

[RequireComponent(typeof(LineRenderer))]
public class CircleRenderer : MonoBehaviour
{
    public int segments = 50;
    public LineRenderer line;

    void Start()
    {
        if(line == null)
        line = gameObject.GetComponent<LineRenderer>();
    }

    public void SetCircle(Vector3 position,float radius)
    {
        this.transform.position = position + Vector3.up * 100;
        line.positionCount = (segments + 1);
        line.useWorldSpace = false;
        CreatePoints(radius);
    }

    void CreatePoints(float radius)
    {
        float x;
        float y;
        float z;

        float angle = 20f;

        for (int i = 0; i < (segments + 1); i++)
        {
            x = Mathf.Sin(Mathf.Deg2Rad * angle) * radius;
            z = Mathf.Cos(Mathf.Deg2Rad * angle) * radius;

            line.SetPosition(i, new Vector3(x, 0, z));

            angle += (360f / segments);
        }
    }
}