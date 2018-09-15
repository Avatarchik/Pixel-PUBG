
using UnityEngine;
using UnityEngine.Networking;

//网络游戏管理
[System.Serializable]
public class DeadArea
{
    public float Time;
    public float EscapeTime;
    public float Radius;
    public byte Damage;
    public bool AirDrop;
}

public class NetworkGameplayManager : NetworkBehaviour
{
    public PlayersManager playersManager;
    public ScoreManager scoreManager;
    public ChatLog chatLog;

    private float timeTmp;
    public float CountdownToPlane = 10;
    public GameObject AirplaneObject;
    public DeadArea[] DeadAreas;
    private int currentArea;
    [SyncVar]
    [HideInInspector]
    public int PlayersAlive = 0;
    [SyncVar]
    [HideInInspector]
    public int PlayersAliveMax = 0;
    [SyncVar]
    [HideInInspector]
    public Vector3 CentreArea;
    [SyncVar]
    [HideInInspector]
    public float Radius;
    [SyncVar]
    [HideInInspector]
    public float LerpValue = 0;
    [SyncVar]
    [HideInInspector]
    public bool IsBattleStart;
    [HideInInspector]
    public bool IsBattleEnded;

    public GameObject SafeArea;
    public GameObject DeadArea;

    private float timeAreaTmp;
    [HideInInspector]
    public GameObject safeArea;
    [HideInInspector]
    public GameObject lastDeadArea;
    private Vector3 scaleTmp;
    private Vector3 positionTmp;
    private float timeEscapeTmp;
    private bool isEscape;


    void Awake()
    {
        UnitZ.NetworkGameplay = this;

        if (playersManager == null)
            playersManager = this.GetComponent<PlayersManager>();
        if (scoreManager == null)
            scoreManager = this.GetComponent<ScoreManager>();
        if (chatLog == null)
            chatLog = this.GetComponent<ChatLog>();
    }

    private void Start()
    {
        IsBattleEnded = false;
        IsBattleStart = false;
        UnitZ.gameManager.IsBattleStart = false;
        UnitZ.gameManager.IsPlaying = true;
        timeTmp = Time.timeSinceLevelLoad;
    }

    public void SpawnPlane(bool drop, Vector3 passingTo)
    {
        if (AirplaneObject != null)
        {
            AirplaneSpawner spawner = GameObject.FindObjectOfType<AirplaneSpawner>();
            if (spawner != null)
            {
                GameObject planeobj = UnitZ.gameNetwork.RequestSpawnObject(AirplaneObject, spawner.SpawnPoint(), Quaternion.identity);
                if (planeobj != null)
                {
                    Debug.Log("Spawn plane");
                    planeobj.transform.LookAt(passingTo + (Vector3.up * planeobj.transform.position.y));
                    Airplane plane = planeobj.GetComponent<Airplane>();
                    if (plane != null)
                    {
                        plane.DropSupply = drop;
                        plane.SetDrop(passingTo);
                    }
                }
            }
        }
    }

    [Command(channel = 0)]
    void CmdStartNewArea(Vector3 position, Vector3 scale, int index)
    {
        RpcStartNewArea(position, scale, index);
    }

    [ClientRpc(channel = 0)]
    void RpcStartNewArea(Vector3 position, Vector3 scale, int index)
    {
        positionTmp = position;
        scaleTmp = scale;
        currentArea = index;
        if (lastDeadArea)
        {
            lastDeadArea.transform.position = positionTmp;
            lastDeadArea.transform.localScale = scaleTmp;
        }
    }

    [HideInInspector]
    public int EndgameRanked = 0;
    public void PlayerKilled(NetworkConnection target)
    {
        if (isServer)
            TargetPlayerKilled(target, PlayersAlive + 1);
    }

    [TargetRpc(channel = 0)]
    private void TargetPlayerKilled(NetworkConnection target, int currentRank)
    {
        Debug.Log("*** You got kill at rank " + currentRank);
        EndgameRanked = currentRank;
        GameOver(false);
    }

    public void PlayerWin(NetworkConnection target)
    {
        if (isServer)
            TargetPlayerWin(target);
    }

    [TargetRpc(channel = 0)]
    private void TargetPlayerWin(NetworkConnection target)
    {
        EndgameRanked = 1;
        GameOver(true);
    }

    public void GameOver(bool isWin)
    {
        if (isWin)
        {
            UnitZ.Hud.OpenPanelByName("Win");
        }
        else
        {
            UnitZ.Hud.OpenPanelByName("Lose");
        }
    }

    public GameObject[] allPlayer;
    private void Update()
    {
        if (isServer)
        {
            allPlayer = GameObject.FindGameObjectsWithTag("Player");
            int alive = 0;
            int lastplayer = -1;

            for (int i = 0; i < allPlayer.Length; i++)
            {
                if (allPlayer[i] != null)
                {
                    CharacterSystem player = allPlayer[i].GetComponent<CharacterSystem>();
                    if (player != null && player.IsAlive)
                    {
                        lastplayer = i;
                        alive += 1;
                    }
                }
            }
            PlayersAlive = alive;

            if (IsBattleStart && !IsBattleEnded)
            {
                if (alive <= 1)
                {
                    if (lastplayer != -1)
                    {
                        if (allPlayer[lastplayer] != null)
                        {
                            CharacterSystem player = allPlayer[lastplayer].GetComponent<CharacterSystem>();
                            if (player != null && player.NetID != -1)
                                PlayerWin(player.connectionToClient);
                        }
                    }
                    IsBattleEnded = true;
                }
            }

        }

        if (!IsBattleStart)
        {
            if (isServer)
            {
                if (Time.timeSinceLevelLoad > timeTmp + CountdownToPlane)
                {
                    SpawnPlane(false, Vector3.zero);
                    isEscape = false;
                    timeAreaTmp = Time.time;
                    IsBattleStart = true;
                    UnitZ.gameManager.IsBattleStart = true;
                    IsBattleEnded = false;
                    getInThePlane();
                }
            }
        }
        else
        {
            if (safeArea == null && SafeArea)
                safeArea = (GameObject)GameObject.Instantiate(SafeArea.gameObject, Vector3.zero, Quaternion.identity);

            if (lastDeadArea == null && DeadArea)
                lastDeadArea = (GameObject)GameObject.Instantiate(DeadArea.gameObject, Vector3.zero, Quaternion.identity);

            if (lastDeadArea)
                lastDeadArea.SetActive(currentArea > 0);

            if (safeArea)
                safeArea.SetActive(currentArea > 0);

            if (isServer)
            {
                if (DeadAreas.Length > currentArea)
                {
                    LerpValue = 0;
                    DeadArea NextArea = DeadAreas[currentArea];
                    currentAreaDamage = NextArea.Damage;
                    Radius = NextArea.Radius;

                    if (Time.time >= timeAreaTmp + NextArea.Time)
                    {
                        if (!isEscape)
                        {
                            timeEscapeTmp = Time.time;
                            isEscape = true;
                        }

                        float timing = 1 - (((NextArea.EscapeTime + timeEscapeTmp) - Time.time) / NextArea.EscapeTime);
                        if (currentArea > 0)
                            LerpValue = timing;

                        if (Time.time > NextArea.EscapeTime + timeEscapeTmp)
                        {
                            if (lastDeadArea)
                            {
                                positionTmp = CentreArea;
                                scaleTmp = new Vector3(Radius, SafeArea.transform.localScale.y, Radius) * 2;
                            }

                            if (currentArea >= DeadAreas.Length - 1)
                                return;

                            currentArea += 1;
                            isEscape = false;
                            timeAreaTmp = Time.time;
                            CentreArea = GetArea(CentreArea, Radius - (DeadAreas[currentArea].Radius));
                            CmdStartNewArea(positionTmp, scaleTmp, currentArea);

                            if (DeadAreas[currentArea].AirDrop)
                                SpawnPlane(true, CentreArea);
                        }
                    }
                }
                damageUpdate();
            }

            if (safeArea && lastDeadArea)
            {
                safeArea.transform.position = CentreArea;
                safeArea.transform.localScale = new Vector3(Radius, SafeArea.transform.localScale.y, Radius) * 2;
                lastDeadArea.transform.position = Vector3.Lerp(positionTmp, safeArea.transform.position, LerpValue);
                lastDeadArea.transform.localScale = Vector3.Lerp(scaleTmp, safeArea.transform.localScale, LerpValue);
            }
        }
    }

    private byte currentAreaDamage;
    private float dmTimeTmp;
    private void damageUpdate()
    {
        //在毒圈中受到伤害
        if (Time.time >= dmTimeTmp + 1)
        {
            for (int i = 0; i < allPlayer.Length; i++)
            {
                if (allPlayer[i] != null)
                {
                    CharacterSystem player = allPlayer[i].GetComponent<CharacterSystem>();
                    if (player != null)
                    {
                        if (lastDeadArea != null && lastDeadArea.activeSelf)
                        {
                            Vector3 playerPos = player.transform.position;
                            playerPos.y = 0;
                            Vector3 deadAreaPos = lastDeadArea.transform.position;
                            deadAreaPos.y = 0;

                            float distance = Vector3.Distance(player.transform.position, lastDeadArea.transform.position);
                            if (distance > (lastDeadArea.transform.localScale.x / 2.0f))
                            {
                                //造成伤害
                                player.ApplyDamage(currentAreaDamage, Vector3.up, -1, 0);
                            }
                        }
                    }
                }
            }
            dmTimeTmp = Time.time;
        }
    }

    Vector3 GetArea(Vector3 pos, float radius)
    {
        Vector2 newpos = Random.insideUnitCircle * radius;
        return pos + new Vector3(newpos.x, 0, newpos.y);
    }

    void getInThePlane()
    {
        Airplane plane = (Airplane)GameObject.FindObjectOfType(typeof(Airplane));
        SpawnAI();

        if (plane != null)
        {
            GameObject[] allPlayer = GameObject.FindGameObjectsWithTag("Player");
            PlayersAliveMax = allPlayer.Length;
            for (int i = 0; i < allPlayer.Length; i++)
            {
                if (allPlayer[i].GetComponent<CharacterDriver>())
                    allPlayer[i].GetComponent<CharacterDriver>().PickupCarCallback(plane);
            }
        }

    }

    GameObject[] AIs;
    public void SpawnAI()
    {
        if (isServer)
        {
            EnemySpawner spawner = (EnemySpawner)GameObject.FindObjectOfType<EnemySpawner>();
            if (spawner)
            {
                spawner.MaxObject = UnitZ.gameManager.BotNumber;
                AIs = spawner.Spawn(UnitZ.gameManager.BotNumber);
                for (int i = 0; i < AIs.Length; i++)
                {
                    if (AIs[i] != null)
                    {
                        AICharacterShooterNav ai = AIs[i].GetComponent<AICharacterShooterNav>();
                        if (ai != null)
                        {
                            ai.Fighting = false;
                        }
                    }
                }
            }
        }
    }

    bool alreadyEquip;
    public void EquipItemAI()
    {
        if (isServer)
        {
            if (!alreadyEquip)
            {
                for (int i = 0; i < AIs.Length; i++)
                {
                    if (AIs[i] != null)
                    {
                        CharacterSystem character = AIs[i].GetComponent<CharacterSystem>();
                        if (character != null)
                        {
                            character.inventory.ApplyStarterItem();
                        }

                        AICharacterShooterNav ai = AIs[i].GetComponent<AICharacterShooterNav>();
                        if (ai != null)
                        {
                            ai.Fighting = true;
                        }
                    }
                }
                alreadyEquip = true;
            }
        }
    }
}
