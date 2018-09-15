using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//角色控制
public class PlayerManager : MonoBehaviour
{
    [HideInInspector]
    public CharacterSystem PlayingCharacter;
    public float AutoRespawnDelay = 3;
    public bool AutoRespawn = false;
    public bool AskForRespawn = true;
    public float SaveInterval = 5;
    public bool SavePlayer = true;
    public bool SaveToServer = false;
    [HideInInspector]
    public SpectreCamera Spectre;
    private float timeTemp = 0;
    private bool savePlayerTemp;
    private float timeAlivetmp = 0;
    private bool autoRespawntmp = false;
    private bool askForRespawntmp = true;
    [HideInInspector]
    public bool spectreMode = false;
    [HideInInspector]
    public float respawnTimer;

    void Start()
    {
        savePlayerTemp = SavePlayer;
        autoRespawntmp = AutoRespawn;
        askForRespawntmp = AskForRespawn;
    }

    public void Reset()
    {
        SavePlayer = savePlayerTemp;
        AutoRespawn = autoRespawntmp;
        AskForRespawn = askForRespawntmp;
        spectreMode = false;
    }

    void Update()
    {
        if (UnitZ.gameNetwork.IsDedicatedServer)
            return;

        if (UnitZ.gameManager.IsPlaying)
            OnPlaying();
    }


    public void Respawn(int spawner)
    {
        if (PlayingCharacter && !PlayingCharacter.IsAlive)
        {
            PlayingCharacter.ReSpawn(spawner);
        }
    }

    public void Respawn(byte team, int spawner)
    {
        if (PlayingCharacter && !PlayingCharacter.IsAlive)
        {
            PlayingCharacter.ReSpawn(team, spawner);
        }
    }

    public void SpectreMode(bool enable)
    {
        spectreMode = enable;
        if (Spectre)
        {
            Spectre.enabled = !enable;
            FPSFlyCamera flycam = Spectre.GetComponent<FPSFlyCamera>();
            if (flycam)
                flycam.enabled = enable;

            if (UnitZ.Hud.IsPanelOpened("Lose"))
            {
                UnitZ.Hud.ClosePanelByName("Lose");
            }
        }
    }

    //游戏中
    public void OnPlaying()
    {
        if (PlayingCharacter)
        {
            //玩家生成
            if (UnitZ.playerSave && PlayingCharacter.IsAlive)
            {
                //玩家存活
                if (Time.time >= timeTemp + SaveInterval)
                {
                    timeTemp = Time.time;
                    if (SavePlayer)
                    {
                        PlayingCharacter.SaveCharacterData(SaveToServer);
                    }
                }
                //玩家存活，关闭失败面板
                if (UnitZ.Hud.IsPanelOpened("Lose"))
                {
                    UnitZ.Hud.ClosePanelByName("Lose");
                }
                timeAlivetmp = Time.time;
            }

            if (!PlayingCharacter.IsAlive)
            {
                //玩家死亡
                if (AutoRespawn)
                {
                    //自动重新生成
                    respawnTimer = (timeAlivetmp + AutoRespawnDelay) - Time.time;
                    if (Time.time > timeAlivetmp + AutoRespawnDelay)
                    {
                        Respawn(-1);
                        UnitZ.Hud.ClosePanelByName("Lose");
                        Debug.Log("Chaacter respawned (" + Time.timeSinceLevelLoad + ")");
                    }
                }
                else
                {
                    if (!UnitZ.Hud.IsPanelOpened("Lose") && !spectreMode)
                    {
                        UnitZ.Hud.OpenPanelByName("Lose");
                    }
                }
            }
        }
        else
        {
            // 如果玩家仍然不存在，则寻找玩家
            findPlayerCharacter();
            if (PlayingCharacter == null)
            {
                //Debug.LogWarning ("Can't find player (CharacterSystem) object in the scene. this is may drain game performance (" + Time.timeSinceLevelLoad + ")");
            }
        }

        //玩家在游戏，且玩家已经死亡
        if (Spectre != null && PlayingCharacter)
        {
            //玩家已死亡
            if (!PlayingCharacter.IsAlive)
            {
                Spectre.Active(true);
            }
            else
            {
                Spectre.Active(false);
                Spectre.LookingAt(PlayingCharacter.gameObject.transform.position);
                PlayingCharacter.spectreThis = true;
            }
        }
        else
        {
            Spectre = (SpectreCamera)GameObject.FindObjectOfType(typeof(SpectreCamera));
            if (Spectre == null)
            {
                //Debug.LogWarning ("Can't find (SpectreCamera) object in the scene. this is may drain game performance (" + Time.timeSinceLevelLoad + ")");
            }
        }
    }

    //查找玩家角色
    private void findPlayerCharacter()
    {
        if (PlayingCharacter == null)
        {
            CharacterSystem[] go = (CharacterSystem[])GameObject.FindObjectsOfType(typeof(CharacterSystem));
            for (int i = 0; i < go.Length; i++)
            {
                CharacterSystem character = go[i];
                if (character)
                {
                    if (character.IsMine)
                    {
                        spectreMode = false;
                        PlayingCharacter = character;
                        if (SavePlayer)
                        {
                            PlayingCharacter.LoadCharacterData(SaveToServer);
                        }
                    }
                }
            }
        }
    }

    //查找生成点
    public Vector3 FindASpawnPoint(int spawner)
    {
        Vector3 spawnposition = Vector3.zero;
        PlayerSpawner[] spawnPoint = (PlayerSpawner[])GameObject.FindObjectsOfType(typeof(PlayerSpawner));

        if (spawner < 0 || spawner >= spawnPoint.Length)
        {
            spawner = Random.Range(0, spawnPoint.Length);
        }
        if (spawnPoint.Length > 0)
        {
            spawnposition = spawnPoint[spawner].transform.position;
        }

        return spawnposition;
    }

    //初始化玩家
    public GameObject InstantiatePlayer(int playerID, string userID, string userName, string characterKey, int characterIndex, byte team, int spawner)
    {
        if (UnitZ.characterManager == null || UnitZ.characterManager.CharacterPresets.Length <= characterIndex || characterIndex < 0)
            return null;

        CharacterSystem characterspawn = UnitZ.characterManager.CharacterPresets[characterIndex].CharacterPrefab;

        if (characterspawn)
        {

            GameObject player = (GameObject)GameObject.Instantiate(characterspawn.gameObject, FindASpawnPoint(spawner), Quaternion.identity);
            CharacterSystem character = player.GetComponent<CharacterSystem>();
            character.NetID = playerID;
            character.Team = team;
            character.CharacterKey = characterKey;
            character.UserName = userName;
            character.UserID = userID;
            MouseLock.MouseLocked = true;
            return player;
        }
        return null;
    }

}
