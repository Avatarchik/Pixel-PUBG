using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

public class ChatLog : NetworkBehaviour
{
	public string Log;
	public Color TextColor = Color.white;
	public bool ActiveChat;
	public float ShowTextDuration = 5;
	float timeTemp;

	void Start ()
	{
	}

	public void AddLog (string text)
	{
		Log += "\n" + text;
		timeTemp = Time.time;
		showLog = true;
	}


	bool showLog = false;

	void Update ()
	{
		if (Input.GetKeyDown (KeyCode.Return)) {
			
			ActiveChat = !ActiveChat;
			if (ActiveChat) {
				timeTemp = Time.time;
				showLog = true;
			}
		}
		if (showLog) {
			if (Time.time >= timeTemp + ShowTextDuration) {
				showLog = false;	
			}
		}
	
	}

	public void Clear ()
	{
		Log = string.Empty;
	}

	string chattext = "";

	void OnGUI ()
	{
		if (UnitZ.playerManager.PlayingCharacter == null)
			return;

		GUI.skin.label.fontSize = 17;
		GUI.skin.label.normal.textColor = TextColor;
		GUI.skin.label.alignment = TextAnchor.LowerLeft;
		
		if (showLog)
			GUI.Label (new Rect (10, 10, Screen.width, 200), Log);
		
		if (ActiveChat) {
			timeTemp = Time.time;
			GUI.SetNextControlName ("Chattext");
			chattext = GUI.TextField (new Rect (10, 210, 200, 20), chattext);
			
			if (Event.current != null && Event.current.keyCode == KeyCode.Return) {
				if (chattext != string.Empty) {
					UnitZ.playerManager.PlayingCharacter.CmdSendMessage ("<color=yellow>" + PlayerPrefs.GetString ("user_name") + " : </color>" + chattext);
					ActiveChat = false;
					chattext = string.Empty;
				}
			}
			GUI.FocusControl ("Chattext");
		}
		
	}
}
