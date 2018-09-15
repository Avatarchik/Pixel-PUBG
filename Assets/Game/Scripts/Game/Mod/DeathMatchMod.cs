using UnityEngine;
using System.Collections;

public class DeathMatchMod : MonoBehaviour
{
	public Texture2D TeamA, TeamB;
	public GUISkin skin;

	void Awake ()
	{
		if (UnitZ.playerManager) {
			UnitZ.playerManager.SavePlayer = false;
			UnitZ.playerManager.AskForRespawn = false;
			UnitZ.playerManager.AutoRespawn = false;
		}
	}

	void Start ()
	{
		StyleManager Styles = (StyleManager)GameObject.FindObjectOfType (typeof(StyleManager));
		if (!skin && Styles)
			skin = Styles.GetSkin (0);
	}

	PlayerConnector playerConnector;

	void Update ()
	{
		if (UnitZ.playerManager == null)
			return;
		
		if (UnitZ.playerManager.PlayingCharacter == null || (UnitZ.playerManager.PlayingCharacter && !UnitZ.playerManager.PlayingCharacter.IsAlive)) {
			MouseLock.MouseLocked = false;	
		}

		if (playerConnector == null) {
			playerConnector = (PlayerConnector)GameObject.FindObjectOfType (typeof(PlayerConnector));
		} else {
			playerConnector.AutoSpawn = false;
		}
	}

	void OnGUI ()
	{
		if (UnitZ.playerManager == null)
			return;
		
		if (skin)
			GUI.skin = skin;
		
		if (!UnitZ.playerManager.PlayingCharacter || !UnitZ.playerManager.PlayingCharacter.IsAlive) {
			GUI.BeginGroup (new Rect ((Screen.width / 2) - 400, (Screen.height / 2) - 200, 800, 400));
		
			if (GUI.Button (new Rect (50, 0, 300, 400), TeamA)) {
				if (playerConnector) {
					playerConnector.RequestSpawnWithTeam (1, 0);	
				} else {
					UnitZ.playerManager.Respawn (1, 0);
				}
			}
		
			if (GUI.Button (new Rect (450, 0, 300, 400), TeamB)) {
				if (playerConnector) {
					playerConnector.RequestSpawnWithTeam (2, 1);	
				} else {
					UnitZ.playerManager.Respawn (2, 1);
				}
			}
		
			GUI.EndGroup ();
		}
			
	}
}
