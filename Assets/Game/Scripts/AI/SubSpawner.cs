using System.Collections;
using System.Collections.Generic;
using UnityEngine;
#if UNITY_EDITOR
using UnityEditor;
#endif

//子生成器
public class SubSpawner : MonoBehaviour {

	//初始化
	void Start () {
		
	}

    //在编辑器绘制gizmose
    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.red;
        Gizmos.DrawSphere(transform.position, 2);
        Gizmos.DrawWireCube(transform.position, this.transform.localScale);
        Handles.Label(transform.position, "Sub Spawner");
#endif
    }
}
