using UnityEngine;
using System.Collections;

//装备物品
public class FPSItemEquipment : MonoBehaviour {
	
	public string ItemID = "";
    public int ItemIndex;
    public string Info = "";
	public ItemCollector CollectorSlot;
    [HideInInspector]
	public bool OnFire1,OnFire2;
	public virtual void Trigger() {
		OnFire1 = true;
	}
	public virtual void Trigger2() {
		OnFire2 = true;
	}
	public virtual void OnTriggerRelease() {
		OnFire1 = false;
	}
	public virtual void OnTrigger2Release() {
		OnFire2 = false;
	}
	public virtual void Reload() {
		
	}
	public virtual void ReloadComplete() {
		
	}
	public virtual void OnAction(){
		
	}
	public void Hide(bool visible){
		Renderer[] render = GetComponentsInChildren<Renderer>();
		foreach(var ob in render)
			ob.enabled = visible;
	}
	
	public void SetItemID(string id){
		ItemID = id;
	}
    public bool IsVisible = true;
	

}
