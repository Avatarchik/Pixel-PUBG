using UnityEngine;
using System.Collections;

//角色管理
public class CharacterManager : MonoBehaviour
{
	//角色预制索引
	public int CharacterIndex = 0;
	//角色预制
	public CharacterPreset[] CharacterPresets;
	private CharacterSaveData characterCreate;
	public CharacterSaveData SelectedCharacter;
	public GameObject CharacterSelected;

	void Start ()
	{
		for (int i = 0; i < CharacterPresets.Length; i++) {
			UnitZ.gameNetwork.spawnPrefabs.Add (CharacterPresets [i].CharacterPrefab.gameObject);
		}
	}

	public bool CreateCharacter (string characterName)
	{
		//创建新角色
		if (UnitZ.gameManager) {
			CreateResult res = new CreateResult ();
			if (characterName != "") {
				//取得新的角色Key
				res = SaveNewCharacter (characterName);
				if (res.IsSuccess) {
					//如果通过
					UnitZ.gameManager.UserName = characterName;
					UnitZ.gameManager.CharacterKey = res.CharacterKey;
					return true;
				}
			}
		}
		return false;
	}

	public void SetCharacter ()
	{
		//从列表中选择一个角色后
		//取得角色预制索引
		CharacterIndex = SelectedCharacter.CharacterIndex;
		
		if (CharacterPresets.Length > 0) {
			//检查角色索引是否正确
			if (CharacterIndex >= CharacterPresets.Length)
				CharacterIndex = CharacterPresets.Length - 1;
			
			if (CharacterIndex < 0)
				CharacterIndex = 0;
			
			//设置玩家名字和角色Key
			if (UnitZ.gameManager) {
				UnitZ.gameManager.UserName = SelectedCharacter.PlayerName;
				UnitZ.gameManager.CharacterKey = SelectedCharacter.CharacterKey;
				CharacterSelected = UnitZ.characterManager.CharacterPresets [CharacterIndex].CharacterPrefab.gameObject;
			}
		}
	}

	public void SetupCharacter (CharacterSaveData character)
	{
		//从列表选择一个角色后
		//取得角色预制索引
		SelectedCharacter = character;
		CharacterIndex = SelectedCharacter.CharacterIndex;
		
		if (CharacterPresets.Length > 0) {
			//检查角色索引是否正确
			if (CharacterIndex >= CharacterPresets.Length)
				CharacterIndex = CharacterPresets.Length - 1;
			
			if (CharacterIndex < 0)
				CharacterIndex = 0;
			
			//设置玩家名字和角色Key
			if (UnitZ.gameManager) {
				UnitZ.gameManager.UserName = SelectedCharacter.PlayerName;
				UnitZ.gameManager.CharacterKey = SelectedCharacter.CharacterKey;
				CharacterSelected = UnitZ.characterManager.CharacterPresets [CharacterIndex].CharacterPrefab.gameObject;
			}
		}
	}

	public void SelectCreateCharacter (int index)
	{
		characterCreate = new CharacterSaveData ();
		characterCreate.CharacterIndex = index;
	}

	public CreateResult SaveNewCharacter (string characterName)
	{
		//创建新角色后存储
		CreateResult res = new CreateResult ();
		res.IsSuccess = false;
		if (characterName != "" && UnitZ.gameManager && UnitZ.playerSave) {
			characterCreate.PlayerName = characterName;
			res = UnitZ.playerSave.CreateCharacter (characterCreate);
		}
		return res;
	}

	public void RemoveCharacter (CharacterSaveData character)
	{
		if (UnitZ.playerSave) {
			UnitZ.playerSave.RemoveCharacter (character);
		}
	}
}
