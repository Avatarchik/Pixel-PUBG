using UnityEngine;
using System.Collections;

public class PlacingArea : MonoBehaviour {

	public bool Snap = false;
	public string[] KeyPair = {""};
	
	//只有匹配时才放物体
	public bool KeyPairChecker(string[] keys ){
		
		if(keys.Length<=0 && KeyPair.Length<=0)
			return true;
		
		for(int i=0;i<KeyPair.Length;i++){
			for(int k=0;k<keys.Length;k++){
				if(keys[k] == KeyPair[i]){
					return true;	
				}
			}
		}
		return false;	
	}
	
	//设置默认
	void Start () {
		if(KeyPair.Length<=0)
			KeyPair = new string[]{""};
	}

	void Update () {
	
	}
}
