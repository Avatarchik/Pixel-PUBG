using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//独立管理
public class DedicatedManager : MonoBehaviour
{
    public PlayerLobby CurrentPlayerLobby;
    public PlayerConnector CurrentPlayer;
    public bool Online = true;
    public string ServerName = "Battle for Survival";
    public string ServerPassword = "";
    public string SceneStart = "battleground";
    public int MinimumPlayer = 1;
    public int BotNumber = 1;
    public float UpdateTime = 1;
    public float RestartTime = 5;
    public float DisconnectTime = 5;
    public bool AutoRestart = true;

    private float timeTmp;
    private bool isServerRestarting = false;
    private bool isGameEnded = false;
    private bool isStarted = false;
    private GameObject lobbyInstance;

    void Start()
    {
        StartDedicated();
        AudioListener.pause = true;
    }

    public void StartDedicated()
    {
        if (lobbyInstance)
            Destroy(lobbyInstance);
        lobbyInstance = new GameObject("LobbyInstance");

        //设置所有参数
        UnitZ.gameNetwork.IsDedicatedServer = true;
        UnitZ.gameNetwork.matchName = ServerName;
        UnitZ.gameNetwork.HostPassword = ServerPassword;
        UnitZ.gameManager.BotNumber = BotNumber;
        isServerRestarting = false;
        isGameEnded = false;
        isStarted = false;

        if (Online)
        {
            //通过unity开启网络中继
            UnitZ.gameNetwork.StartMatchMaker();
            UnitZ.gameNetwork.HostGame(SceneStart, true);
            if (ServerLog.instance != null)
                ServerLog.instance.Log("Starting new lobby using Unity Relay Server");
        }
        else
        {
            //通过局域网开启
            UnitZ.gameNetwork.HostGame(SceneStart, false);
            if (ServerLog.instance != null)
                ServerLog.instance.Log("Starting new lobby on Local network : " + UnitZ.gameNetwork.networkAddress);
        }

    }

    //断开连接
    public void OnDisconnect()
    {
        if (!isActiveAndEnabled)
            return;
        UnitZ.gameManager.IsPlaying = false;
        Debug.Log("Server disconnected, Restarting..");
        isServerRestarting = true;
        timeTmp = Time.time;
        if (ServerLog.instance != null)
            ServerLog.instance.Log("Stopped Host.");
    }


    void Update()
    {
        if (UnitZ.gameManager.IsPlaying)
        {
            if (CurrentPlayer != null && CurrentPlayer.isServer)
            {
                if (UnitZ.NetworkGameplay.IsBattleStart && UnitZ.NetworkGameplay.IsBattleEnded)
                {
                    if (!isGameEnded)
                    {
                        //游戏结束
                        Debug.Log("Game is over");
                        if (ServerLog.instance != null)
                            ServerLog.instance.Log("Match is over");
                        isGameEnded = true;
                        timeTmp = Time.time;
                    }
                    else
                    {
                        //游戏已经结束
                        if (!isServerRestarting)
                        {
                            if (Time.time >= timeTmp + DisconnectTime)
                            {
                                //断开连接
                                UnitZ.gameNetwork.Disconnect();
                                Debug.Log("Stop host!");
                                isServerRestarting = true;
                                timeTmp = Time.time;
                            }
                        }
                    }
                }
            }
            if (UnitZ.playerManager.Spectre != null)
            {
                GameObject.Destroy(UnitZ.playerManager.Spectre.gameObject);
            }
        }
        else
        {
            if (isServerRestarting)
            {
                if (Time.time >= timeTmp + RestartTime)
                {
                    if (AutoRestart)
                    {
                        //重新开始
                        Debug.Log("Restart!");
                        StartDedicated();
                        isServerRestarting = false;
                        if (ServerLog.instance != null)
                            ServerLog.instance.Log("Restart hosting..");
                    }
                    else
                    {
                        if (ServerLog.instance != null)
                            ServerLog.instance.Log("No restart");
                    }
                    timeTmp = Time.time;
                }
            }
            else
            {
                if (Time.time > timeTmp + UpdateTime)
                {
                    if (CurrentPlayerLobby != null && CurrentPlayerLobby.isServer)
                    {
                        int playercount = UnitZ.gameNetwork.GetLobbyPlayerCount();
                        if (playercount - 1 >= MinimumPlayer)
                        {
                            if (UnitZ.gameNetwork.IsOnLobby() && !isStarted)
                            {
                                Debug.Log("Start Server");
                                isStarted = UnitZ.gameNetwork.StartServerGame();
                            }
                        }
                    }
                    timeTmp = Time.time;
                }
            }
        }
    }

    public void ForceStartServer()
    {
        if (ServerLog.instance != null)
            ServerLog.instance.Log("Force start match");
        StartCoroutine(UnitZ.gameNetwork.ServerCountdownCoroutine());
    }
}
