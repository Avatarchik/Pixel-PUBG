using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//玩家管理
public class PlayersManager : NetworkBehaviour
{
	public float VersionCheckDelay = 5;
	[SyncVar (hook = "OnDataChanged")]
	public string PlayersData = "";
	public List<string> TeamsList = new List<string> ();
	public List<PlayerData> PlayerList = new List<PlayerData> ();

	void Awake ()
	{
		ClearPlayers ();
	}

	public void ClearPlayers ()
	{
		PlayerList.Clear ();
		TeamsList.Clear ();
	}

	public PlayerData GetPlayerData (int ID)
	{
		foreach (PlayerData player in PlayerList) {
			if (player.ID == ID) {
				return player;
			}
		}
		return new PlayerData ();
	}

	public void AddPlayer (int id)
	{
		PlayerData play = new PlayerData ();
		play.Dead = 0;
		play.ID = id;
		play.Name = "";
		play.Team = "";
		PlayerList.Add (play);
	}

	private void AddTeam (string team)
	{
		if (TeamsList.Contains (team)) {
			return;
		}
		TeamsList.Add (team);
	}

	public void RemovePlayer (int id)
	{
		Debug.Log (id + " Removed");
		for (int i = 0; i < PlayerList.Count; i++) {
			if (id == PlayerList [i].ID) {
				PlayerList.RemoveAt (i);
				rewritePlayersData ();
				return;
			}
		}
	}

	public void UpdatePlayerInfo (int id, int score, int dead, string name, string team, string gameKey, bool isconnected)
	{
		bool have = false;
		foreach (PlayerData pp in PlayerList) {
			if (pp.ID == id) {
				have = true;
			}
		}
		if (!have) {
			AddPlayer (id);
		}

		PlayerData player = new PlayerData ();
		player.Dead = dead;
		player.ID = id;
		player.Name = name;
		player.Team = team;
		AddTeam (team);

		for (int i = 0; i < PlayerList.Count; i++) {
			if (id == PlayerList [i].ID) {
				PlayerList [i] = player;
				rewritePlayersData ();
				return;
			}
		}

	}

	public void AddScore (int id, int score, int dead)
	{

		for (int i = 0; i < PlayerList.Count; i++) {
			if (PlayerList [i].ID == id) {

				PlayerData player = new PlayerData ();
				player.Dead = PlayerList [i].Dead;
				player.ID = PlayerList [i].ID;
				player.Name = PlayerList [i].Name;
				player.Team = PlayerList [i].Team;
				player.Score = PlayerList [i].Score + score;
				player.Dead = PlayerList [i].Dead + dead;
				PlayerList [i] = player;

				rewritePlayersData ();
				return;
			}
		}
	}

	void rewritePlayersData ()
	{
		string idlist = "";
		string teamlist = "";
		string deadlist = "";
		string namelist = "";
		string scorelist = "";
		string allteamlist = "";

		foreach (PlayerData pp in PlayerList) {
			if (pp.ID != -1) {
				idlist += pp.ID + ",";
				teamlist += pp.Team + ",";
				scorelist += pp.Score + ",";
				deadlist += pp.Dead + ",";
				namelist += pp.Name + ",";
			}
		}
		foreach (string tt in TeamsList) {
			allteamlist += tt + ",";
		}

		PlayersData = idlist + "|" + teamlist + "|" + scorelist + "|" + deadlist + "|" + namelist + "|" + allteamlist;
	}

	public void ReadData (string data)
	{
		PlayerList.Clear ();
		TeamsList.Clear ();

		string[] dataget = data.Split ("|" [0]);
		if (dataget.Length >= 6) {
			string[] idget = dataget [0].Split ("," [0]);
			string[] teamget = dataget [1].Split ("," [0]);
			string[] scoreget = dataget [2].Split ("," [0]);
			string[] deadget = dataget [3].Split ("," [0]);
			string[] nameget = dataget [4].Split ("," [0]);
			string[] allteamget = dataget [5].Split ("," [0]);

			for (int i = 0; i < idget.Length; i++) {
				if (idget [i] != "") {
					PlayerData playerdata = new PlayerData ();
                    int.TryParse(idget[i], out playerdata.ID);
                    int.TryParse (scoreget [i], out playerdata.Score);
					int.TryParse (deadget [i], out playerdata.Dead);
					playerdata.Name = nameget [i];
					playerdata.Team = teamget [i];
					PlayerList.Add (playerdata);
				}
			}

			for (int i = 0; i < allteamget.Length; i++) {
				AddTeam (allteamget [i]);
			}
		}
	}

	public void OnDataChanged (string data)
	{
		PlayersData = data;
		ReadData (data);
	}


}



public struct PlayerData
{
	public int ID;
	public int Score;
	public string Team;
	public int Dead;
	public string Name;

}

