using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System;

//玩家存储
public class PlayerSave : MonoBehaviour
{

	public List<PlayerSaveData> LoadedData = new List<PlayerSaveData> ();
	[HideInInspector]
	public CharacterSystem MainCharacter;

	void Start ()
	{

		if (UnitZ.gameManager == null)
			return;
		

		if (UnitZ.gameManager.UserID == "") {
			Debug.Log ("UID is not assigned");
			UnitZ.gameManager.UserID = PlayerPrefs.GetString ("UID");
			if (UnitZ.gameManager.UserID == "") {
				UnitZ.gameManager.UserID = GetUniqueID ();
				PlayerPrefs.SetString ("UID", UnitZ.gameManager.UserID);
				Debug.Log ("UID is generated " + UnitZ.gameManager.UserID);
			} else {
				Debug.Log ("UID is " + UnitZ.gameManager.UserID);
			}
		}
	}

    //删除存储
	public void DeleteSave ()
	{
		if (UnitZ.gameManager == null)
			return;
		
		DeleteSave (UnitZ.gameManager.UserID, UnitZ.gameManager.CharacterKey, UnitZ.gameManager.UserName);
	}

	public void DeleteSave (string userID, string characterKey, string userName)
	{
		if (userID == "")
			return;
		
		PlayersRegister (userID);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave.UID = userID;
		playersave.PlayerName = userName;
		playersave.CharacterKey = characterKey;
		playersave.ItemData = "";
		playersave.EquipData = "";
		playersave.FPSItemIndex = 0;
		playersave.Position = "";
		playersave.LevelName = "";
		playersave.Food = 0;
		playersave.Water = 0;
		playersave.Health = 0;

		WriteData (playersave);
	}

    //存储玩家
	public void SavePlayer (CharacterSystem character)
	{

		if (UnitZ.gameManager == null || character == null)
			return;
		
		PlayersRegister (UnitZ.gameManager.UserID);
		PlayerSaveData playersave = new PlayerSaveData ();
			
		playersave.UID = UnitZ.gameManager.UserID;
		playersave.PlayerName = UnitZ.gameManager.UserName;
		playersave.CharacterKey = character.CharacterKey;
		playersave.ItemData = character.inventory.GetItemDataText ();
		playersave.EquipData = character.inventory.GenStickerTextData ();
		playersave.FPSItemIndex = character.inventory.GetCollectorFPSindex ();
		playersave.Position = character.transform.position.x + "," + character.transform.position.y + "," + character.transform.position.z;
		playersave.LevelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
		playersave.Health = character.HP;
		
		CharacterLiving living = character.GetComponent<CharacterLiving> ();
		if (living) {
			playersave.Food = living.Hungry;
			playersave.Water = living.Water;
		}
		WriteData (playersave);
	}

    //存储到text
	public string GetPlayerSaveToText (CharacterSystem character)
	{

		if (UnitZ.gameManager == null || character == null)
			return "";
		
		PlayersRegister (UnitZ.gameManager.UserID);
		PlayerSaveData playersave = new PlayerSaveData ();
			
		playersave.UID = UnitZ.gameManager.UserID;
		playersave.PlayerName = UnitZ.gameManager.UserName;
		playersave.CharacterKey = character.CharacterKey;
		if (character.inventory != null) {
			playersave.ItemData = character.inventory.GetItemDataText ();
			playersave.EquipData = character.inventory.GenStickerTextData ();
			playersave.FPSItemIndex = character.inventory.GetCollectorFPSindex ();
		}
		playersave.Position = character.transform.position.x + "," + character.transform.position.y + "," + character.transform.position.z;
		playersave.LevelName = UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name;
		playersave.Health = character.HP;
		
		CharacterLiving living = character.GetComponent<CharacterLiving> ();
		if (living) {
			playersave.Food = living.Hungry;
			playersave.Water = living.Water;
		}

		return PlayerSaveDataText (playersave);
	}


    //加载玩家
	public void LoadPlayer (CharacterSystem character)
	{
		if (UnitZ.gameManager == null || character == null)
			return;
		
		MainCharacter = character;
		string hasKey = UnitZ.gameManager.UserID + "_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name + "_" + character.CharacterKey + "_" + UnitZ.gameManager.UserName;
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = ReadData (hasKey);
		ApplyPlayerData (playersave, character);
		
	}

    //写入数据
	public void WriteData (PlayerSaveData playersave)
	{
		string hasKey = playersave.UID + "_" + UnityEngine.SceneManagement.SceneManager.GetActiveScene ().name + "_" + playersave.CharacterKey + "_" + playersave.PlayerName;
		//Debug.Log("SAVE : HAS KEY "+hasKey);
		PlayerPrefs.SetString ("PLAYER_" + hasKey, playersave.UID);
		PlayerPrefs.SetString ("NAME_" + hasKey, playersave.PlayerName);
		PlayerPrefs.SetString ("CHARACTERKEY_" + hasKey, playersave.CharacterKey);
		PlayerPrefs.SetString ("ITEM_" + hasKey, playersave.ItemData);
		PlayerPrefs.SetString ("EQUIP_" + hasKey, playersave.EquipData);
		PlayerPrefs.SetInt ("FPSINDEX" + hasKey, playersave.FPSItemIndex);
		PlayerPrefs.SetString ("POS" + hasKey, playersave.Position);
		PlayerPrefs.SetString ("LEVELNAME" + hasKey, playersave.LevelName);
		PlayerPrefs.SetInt ("FOOD" + hasKey, playersave.Food);
		PlayerPrefs.SetInt ("WATER" + hasKey, playersave.Water);
		PlayerPrefs.SetInt ("HEALTH" + hasKey, playersave.Health);
		//Debug.Log ("Write Data " + playersave.UID);
		//Debug.Log (PlayerSaveDataText (playersave));
	}

    //读取数据
	public PlayerSaveData ReadData (string hasKey)
	{
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave.UID = PlayerPrefs.GetString ("PLAYER_" + hasKey);
		playersave.PlayerName = PlayerPrefs.GetString ("NAME_" + hasKey);
		playersave.CharacterKey = PlayerPrefs.GetString ("CHARACTERKEY_" + hasKey);
		playersave.ItemData = PlayerPrefs.GetString ("ITEM_" + hasKey);
		playersave.EquipData = PlayerPrefs.GetString ("EQUIP_" + hasKey);
		playersave.FPSItemIndex = PlayerPrefs.GetInt ("FPSINDEX" + hasKey);
		playersave.Position = PlayerPrefs.GetString ("POS" + hasKey);
		playersave.LevelName = PlayerPrefs.GetString ("LEVELNAME" + hasKey);
		playersave.Food = (byte)PlayerPrefs.GetInt ("FOOD" + hasKey);
		playersave.Water = (byte)PlayerPrefs.GetInt ("WATER" + hasKey);
		playersave.Health = (byte)PlayerPrefs.GetInt ("HEALTH" + hasKey);
		return playersave;
	}

    //玩家存储数据
	public string PlayerSaveDataText (PlayerSaveData playersave)
	{
		return playersave.UID +
		"^" + playersave.PlayerName +
		"^" + playersave.ItemData +
		"^" + playersave.EquipData +
		"^" + playersave.FPSItemIndex +
		"^" + playersave.Position +
		"^" + playersave.LevelName +
		"^" + playersave.Food +
		"^" + playersave.Water +
		"^" + playersave.Health +
		"^" + playersave.CharacterKey;
	}

    //从text取得存储数据
	public PlayerSaveData GetSaveDataFromText (string dataText)
	{
		string[] raw = dataText.Split ("^" [0]);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave.UID = raw [0];
		playersave.PlayerName = raw [1];
		playersave.ItemData = raw [2];
		playersave.EquipData = raw [3];
		int.TryParse (raw [4], out playersave.FPSItemIndex);
		playersave.Position = raw [5];
		playersave.LevelName = raw [6];
        byte.TryParse (raw [7], out playersave.Food);
        byte.TryParse (raw [8], out playersave.Water);
        byte.TryParse (raw [9], out playersave.Health);
		playersave.CharacterKey = raw [10];
		
		return playersave;
	}

    //应用玩家数据
	void ApplyPlayerData (PlayerSaveData playersave)
	{
		ApplyPlayerData (playersave, MainCharacter);
	}

	void ApplyPlayerData (PlayerSaveData playersave, CharacterSystem character)
	{
		if (character && character.inventory) {

			string[] positionraw = playersave.Position.Split ("," [0]);
			if (positionraw.Length > 2) {
				Vector3 position = Vector3.zero;
				float.TryParse (positionraw [0], out position.x);
				float.TryParse (positionraw [1], out position.y);
				float.TryParse (positionraw [2], out position.z);
				character.transform.position = position;
			}

			character.HP = playersave.Health;
			CharacterLiving living = character.GetComponent<CharacterLiving> ();
			if (living) {
				living.Hungry = playersave.Food;
				living.Water = playersave.Water;
			}
			//如果有新存储，参数都使用默认
			if (playersave.Food <= 0 && playersave.Water <= 0 && playersave.Health <= 0) {
				character.HP = character.HPmax;
				if (living) {
					living.Hungry = living.HungryMax;
					living.Water = living.WaterMax;	
				}
			}

			//Debug.Log ("Applying "+playersave.ItemData+" |  "+playersave.EquipData+" | "+playersave.FPSItemIndex);
			//Debug.Log ("with " + PlayerSaveDataText (playersave));
			character.inventory.SetupStarterItem ();
			character.inventory.SetItemsFromText (playersave.ItemData);
			character.inventory.UpdateSticker (playersave.EquipData);

			if (character.inventory.Items.Count > playersave.FPSItemIndex)
				character.inventory.EquipItemByCollector (character.inventory.Items [playersave.FPSItemIndex]);
		}
	}

    //存到服务器
	public void SaveToServer (string dataText)
	{
		//Debug.Log ("Server received : " + dataText);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = GetSaveDataFromText (dataText);
		WriteData (playersave);
	}

    //存到本地
	public void SaveToLocal (string dataText)
	{
		//Debug.Log ("Server received : " + dataText);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = GetSaveDataFromText (dataText);
		WriteData (playersave);
	}

    //接受数据并应用
	public void ReceiveDataAndApply (string datatext, CharacterSystem character)
	{
		//Debug.Log ("Received From Server " + datatext);
		
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = GetSaveDataFromText (datatext);

		ApplyPlayerData (playersave, character);
		
	}

    //从服务器取到数据
	public string GetDataFromServer (string hasKey)
	{
		//Debug.Log ("Request Loading " + hasKey);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = ReadData (hasKey);
		return PlayerSaveDataText (playersave);
	}
    //从本地取到数据
	public string GetDataFromLocal (string hasKey)
	{
		//Debug.Log ("Request Loading " + hasKey);
		PlayerSaveData playersave = new PlayerSaveData ();
		playersave = ReadData (hasKey);
		return PlayerSaveDataText (playersave);
	}

    //玩家注册
	public bool PlayersRegister (string uid)
	{
		string allplayer = PlayerPrefs.GetString ("PLAYERS");
		//Debug.Log ("Load all UID " + allplayer);
		string[] playerlist = allplayer.Split ("," [0]);
		for (int i = 0; i < playerlist.Length; i++) {
			if (playerlist [i] != "" && uid == playerlist [i]) {
				//Debug.Log (uid + ": is alrady exists");
				return false;	
			}
		}
		allplayer += uid + ",";
		PlayerPrefs.SetString ("PLAYERS", allplayer);
		//Debug.Log ("Registered " + allplayer);
		return true;
	}

	void Update ()
	{
	
	}

    //取得ID
	public string GetUniqueID ()
	{
		var random = new System.Random ();   
		DateTime epochStart = new System.DateTime (1970, 1, 1, 8, 0, 0, System.DateTimeKind.Utc);
		double timestamp = (System.DateTime.UtcNow - epochStart).TotalSeconds;
		string uniqueID = String.Format ("{0:X}", Convert.ToInt32 (timestamp))
		                  + "-" + String.Format ("{0:X}", Convert.ToInt32 (Time.time * 1000000))
		                  + "-" + String.Format ("{0:X}", random.Next (1000000000));
         
		//Debug.Log ("Generated Unique ID: " + uniqueID);
		return uniqueID;
	}

    //存储角色
	public bool SaveCharacter (CharacterSaveData character)
	{
		string hasKey = character.CharacterKey;
		//Debug.Log ("Save Character " + hasKey + " " + PlayerPrefs.HasKey ("CHARACTER_" + hasKey));
		if (!PlayerPrefs.HasKey ("CHARACTER_" + hasKey)) {
			PlayerPrefs.SetString ("CHARACTER_" + hasKey, character.PlayerName);
			PlayerPrefs.SetInt ("CharacterIndex_" + hasKey, character.CharacterIndex);
			return true;
		}
		return false;
	}

    //更新角色
    public bool UpdateCharacter(CharacterSaveData character)
    {
        string hasKey = character.CharacterKey;
        //Debug.Log("Update Character " + hasKey + " " + PlayerPrefs.HasKey("CHARACTER_" + hasKey));
        if (PlayerPrefs.HasKey("CHARACTER_" + hasKey))
        {
            PlayerPrefs.SetString("CHARACTER_" + hasKey, character.PlayerName);
            PlayerPrefs.SetInt("CharacterIndex_" + hasKey, character.CharacterIndex);
            return true;
        }
        return false;
    }

    //创建角色
    public CreateResult CreateCharacter (CharacterSaveData character)
	{
		CreateResult res = new CreateResult ();
		string charactersList = PlayerPrefs.GetString ("CHARACTERS");
		character.CharacterKey = GetUniqueID ();
		res.CharacterKey = character.CharacterKey;
		res.IsSuccess = false;
		
		if (SaveCharacter (character)) {
			charactersList += "," + character.CharacterKey;
			PlayerPrefs.SetString ("CHARACTERS", charactersList);
			res.IsSuccess = true;
			//Debug.Log (" character list " + charactersList);
		}
		return res;
	}

    //移除角色
	public void RemoveCharacter (CharacterSaveData character)
	{
		string[] charactersList = PlayerPrefs.GetString ("CHARACTERS").Split ("," [0]);
		string characters = "";
		string hasKey = character.CharacterKey;
		
		
		if (charactersList.Length > 0) {
			for (int i = 0; i < charactersList.Length; i++) {
				if (charactersList [i] != "" && charactersList [i] != hasKey) {
					characters += "," + charactersList [i];
				}
			}
			PlayerPrefs.SetString ("CHARACTERS", characters);
			PlayerPrefs.DeleteKey ("CHARACTER_" + hasKey);
			PlayerPrefs.DeleteKey ("CharacterIndex_" + hasKey);
			
			if (UnitZ.gameManager)
				DeleteSave (UnitZ.gameManager.UserID, hasKey, character.PlayerName);
		}
	}

    //角色存储数据
	public CharacterSaveData LoadCharacter (string key)
	{
		//Debug.Log ("Load " + name);
		CharacterSaveData character = new CharacterSaveData ();
		string hasKey = key;
		
		if (PlayerPrefs.HasKey ("CHARACTER_" + hasKey)) {
			character.PlayerName = PlayerPrefs.GetString ("CHARACTER_" + hasKey);
			character.CharacterIndex = PlayerPrefs.GetInt ("CharacterIndex_" + hasKey);
			character.CharacterKey = hasKey;
		}
		return character;
	}

	public CharacterSaveData[] LoadAllCharacters ()
	{
		string[] charactersList = PlayerPrefs.GetString ("CHARACTERS").Split ("," [0]);
		List<CharacterSaveData> chars = new List<CharacterSaveData> ();
		
		if (charactersList.Length > 0) {
			for (int i = 0; i < charactersList.Length; i++) {
				if (charactersList [i] != "") {
					CharacterSaveData ch = LoadCharacter (charactersList [i]);
					if (ch.PlayerName != "") {
						chars.Add (ch);
					}
				}
			}
			
			CharacterSaveData[] allCharacters = chars.ToArray ();
			
			return allCharacters;
		}
		return null;
	}
	
}

public struct PlayerSaveData
{
	public string UID;
	public string ItemData;
	public string EquipData;
	public int FPSItemIndex;
	public string PlayerName;
	public string CharacterKey;
	public string Position;
	public string LevelName;
	public byte Food;
	public byte Water;
	public byte Health;
}

public struct CreateResult
{
	public bool IsSuccess;
	public string CharacterKey;
}
[Serializable]
public struct CharacterSaveData
{
	public string PlayerName;
	public string CharacterKey;
	public int CharacterIndex;
}

[System.Serializable]
public class CharacterPreset
{
	public CharacterSystem CharacterPrefab;
	public Texture2D Icon;
	public string Name;
	public string Description;
}
