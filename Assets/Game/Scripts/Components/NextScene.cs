using UnityEngine;
using System.Collections;

public class NextScene : MonoBehaviour {

	public string SceneName = "mainmenu";
	
	void Start () {
		UnityEngine.SceneManagement.SceneManager.LoadScene(SceneName);
	}
	
}
