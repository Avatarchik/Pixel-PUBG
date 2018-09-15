using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

[RequireComponent(typeof(UnityEngine.AI.NavMeshAgent))]

//导航
public class NavAgent : MonoBehaviour {

	public GameObject Owner;
	public float DistanceLimit = 1;
	public UnityEngine.AI.NavMeshAgent navMeshAgent;
	public bool Show;
	
	void Start () {
		navMeshAgent = this.GetComponent<UnityEngine.AI.NavMeshAgent>();
		navMeshAgent.avoidancePriority = Random.Range(0,100);

		Renderer render = this.GetComponent<Renderer>();
		if(render)
			render.enabled = Show;
	}
	
	public void SetTarget(Vector3 pos){
		navMeshAgent.SetDestination(pos);
	}
	
	void Update () {
		if(Owner == null || Owner.transform.localScale == Vector3.one * 0.000001f){
			Destroy(this.gameObject);	
		}else{
			if(Vector3.Distance(Owner.transform.position,this.transform.position) >= DistanceLimit){
				this.transform.position = Owner.transform.position;	
			}	
		}
	}

}
