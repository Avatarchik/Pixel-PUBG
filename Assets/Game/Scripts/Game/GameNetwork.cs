//游戏网络管理
using UnityEngine;
using UnityEngine.SceneManagement;
using System.Collections.Generic;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using System.Collections;
using UnityEngine.Networking.Types;


public class GameNetwork : NetworkLobbyManager
{
    [HideInInspector]
    public bool IsDedicatedServer;                      //是否启用服务器
    [HideInInspector]
    public List<MatchInfoSnapshot> MatchListResponse;   //匹配队列查询
    [HideInInspector]
    public MatchInfoSnapshot MatchSelected;             //匹配选择
    public NetworkGameplayManager NetworkGamePlay;      //网络游戏管理
    public string HostPassword = "";
    public string HostNameFillter = "";
    public bool AutoStart = false;
    static short MsgKicked = MsgType.Highest + 1;
    static short MsgBegin = MsgType.Highest + 2;
    protected ulong currentMatchID;
    private bool isMatchMaking;

    void Start()
    {
    }

    //游戏大厅
    public bool IsOnLobby(string scenename = "")
    {
        if (scenename == "")
            scenename = SceneManager.GetActiveScene().name;
        return (scenename == this.lobbyScene);
    }

    //加入游戏成员
    public override void OnServerAddPlayer(NetworkConnection conn, short playerControllerId)
    {
        if (!IsOnLobby())
        {
            var player = (GameObject)GameObject.Instantiate(playerPrefab, new Vector2(0, 0), Quaternion.identity);
            NetworkServer.AddPlayerForConnection(conn, player, playerControllerId);
            return;
        }
        base.OnServerAddPlayer(conn, playerControllerId);
    }

    //客户端改变
    public override void OnClientSceneChanged(NetworkConnection conn)
    {
        base.OnClientSceneChanged(conn);
    }

    //服务端场景改变
    public override void OnServerSceneChanged(string sceneName)
    {
        if (!IsOnLobby(sceneName))
        {
            if (NetworkServer.active)
            {
                if (NetworkGamePlay != null && UnitZ.NetworkGameplay == null)
                {
                    GameObject networkobject = (GameObject)GameObject.Instantiate(NetworkGamePlay.gameObject, Vector3.zero, Quaternion.identity);
                    NetworkServer.Spawn(networkobject);
                }
            }
        }
        base.OnServerSceneChanged(sceneName);
    }

    //游戏匹配
    public void FindInternetMatch()
    {
        MatchListResponse = null;
        singleton.StartMatchMaker();
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(true);
        singleton.matchMaker.ListMatches(0, 50, HostNameFillter, false, 0, 0, OnMatchList);
    }

    //匹配队列
    public override void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matchList)
    {
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);
        MatchListResponse = matchList;
        if (MatchListResponse != null && success)
        {
            if (matchList.Count != 0)
            {
                Debug.Log("Server lists ");
                for (int i = 0; i < MatchListResponse.Count; i++)
                {
                    Debug.Log("Game " + MatchListResponse[i].name + " " + MatchListResponse[i].currentSize + "/" + MatchListResponse[i].maxSize + " (Private)" + MatchListResponse[i].isPrivate);
                }
            }
            else
            {
                Debug.Log("No matches in requested room!");
            }
        }
        else
        {
            Debug.LogError("Couldn't connect to match maker");
        }
    }

    //主机控制
    public void HostGame(string levelname, bool online)
    {
        playScene = levelname;
        isMatchMaking = online;
        startingServer = false;
        if (online)
        {
            if (UnitZ.Hud != null)
                UnitZ.Hud.ProcessPopup.SetActive(true);
            StartMatchMaker();
            singleton.matchMaker.CreateMatch(matchName, (uint)maxConnections, true, HostPassword, "", "", 0, 0, OnMatchCreate);
        }
        else
        {
            singleton.StartHost();
        }

        Debug.Log("Host game Max" + maxConnections);
    }

    //加入游戏
    public void JoinGame()
    {
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.SetPreviousPanel(MainMenuManager.menu.currentPanel);
        if (MatchSelected != null)
        {
            if (MatchSelected.isPrivate)
            {
                Debug.Log("Need password");
                if (Popup.Pop != null)
                {
                    Popup.Pop.AskingPassword("need password ", delegate
                    {
                        Popup.Pop.PopupPasswordObject.gameObject.SetActive(false);
                        Debug.Log("Access with Password " + Popup.Pop.PopupPasswordObject.Password);
                        singleton.matchMaker.JoinMatch(MatchSelected.networkId, Popup.Pop.PopupPasswordObject.Password, "", "", 0, 0, OnMatchJoined);
                    },
                        delegate
                        {
                            LeaveMatch();
                            Popup.Pop.PopupPasswordObject.gameObject.SetActive(false);
                        });
                }
            }
            else
            {
                if (UnitZ.Hud != null)
                    UnitZ.Hud.ProcessPopup.SetActive(true);
                singleton.matchMaker.JoinMatch(MatchSelected.networkId, "", "", "", 0, 0, OnMatchJoined);

            }
            Debug.Log("Connecting to matchMaker");
        }
        else
        {
            singleton.networkAddress = networkAddress;
            singleton.networkPort = networkPort;
            singleton.StartClient();
            Debug.Log("Connecting to IP : " + networkAddress);
        }

    }

    //匹配选择
    public void MatchSelect(MatchInfoSnapshot match)
    {
        currentMatchID = (System.UInt64)match.networkId;
        MatchSelected = match;
    }

    //停止连接
    public override void OnDropConnection(bool success, string extendedInfo)
    {
        Debug.Log("Game is procesed!");
        base.OnDropConnection(success, extendedInfo);
    }

    //加入匹配
    public override void OnMatchJoined(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);
        Debug.Log("Connecting success " + success + " " + extendedInfo + " " + matchInfo);
        if (success)
        {
            singleton.StartClient(matchInfo);
            Debug.Log("Connected!");
        }
        else
        {
            if (Popup.Pop != null)
            {
                Popup.Pop.Asking("Unable to connect", null, delegate
                {

                });
            }
        }
    }

    //客户端大厅场景改变
    public override void OnLobbyClientSceneChanged(NetworkConnection conn)
    {
        if (IsOnLobby())
        {
            if (MainMenuManager.menu != null)
                MainMenuManager.menu.OpenPanelByName("Home");
        }
    }

    //服务器信息
    public void SetServerInfo(string status, string host)
    {
        Debug.Log("Received server info " + status + " " + host);
    }

    //主机开启
    public override void OnStartHost()
    {
        base.OnStartHost();
        Debug.Log("On Start Host");
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.OpenPanelByName("Lobby");
        SetServerInfo("Hosting", networkAddress);
    }

    //创建匹配队列
    public override void OnMatchCreate(bool success, string extendedInfo, MatchInfo matchInfo)
    {
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);
        Debug.Log("On Match created!! " + success);
        base.OnMatchCreate(success, extendedInfo, matchInfo);
        currentMatchID = (System.UInt64)matchInfo.networkId;
    }

    //销毁匹配过程
    public override void OnDestroyMatch(bool success, string extendedInfo)
    {
        Debug.Log("On Destroy match");
        base.OnDestroyMatch(success, extendedInfo);
        StopMatchMaker();
        StopHost();
    }

    //移除玩家
    public void RemovePlayer(PlayerLobby player)
    {
        player.RemovePlayer();
    }

    //销毁匹配
    public void DestroyMatch()
    {
        if (isMatchMaking)
        {
            matchMaker.DestroyMatch((NetworkID)currentMatchID, 0, OnDestroyMatch);
        }
        else
        {
            StopHost();
        }
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.OpenPreviousPanel();
    }

    //离开匹配
    public void LeaveMatch()
    {
        MatchSelected = null;
        StopClient();
        if (isMatchMaking)
        {
            StopMatchMaker();
        }
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.OpenPreviousPanel();
    }

    //服务器大厅创建玩家
    public override GameObject OnLobbyServerCreateLobbyPlayer(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("On Lobby server create lobby player");
        GameObject obj = Instantiate(lobbyPlayerPrefab.gameObject) as GameObject;
        PlayerLobby newPlayer = obj.GetComponent<PlayerLobby>();
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            PlayerLobby p = lobbySlots[i] as PlayerLobby;

            if (p != null)
            {
                p.RpcUpdatePlayerLobby();
            }
        }
        if (ServerLog.instance != null)
            ServerLog.instance.Log("Player lobby:" + conn.address + " created");
        return obj;
    }

    //服务器大厅移除玩家
    public override void OnLobbyServerPlayerRemoved(NetworkConnection conn, short playerControllerId)
    {
        Debug.Log("On Lobby server remove lobby player");
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            PlayerLobby p = lobbySlots[i] as PlayerLobby;

            if (p != null)
            {
                p.RpcUpdatePlayerLobby();
            }
        }
        if (ServerLog.instance != null)
            ServerLog.instance.Log("Player lobby:" + conn.address +" removed");
    }

    //服务器大厅断开连接
    public override void OnLobbyServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("On lobby disconnect");
        if (ServerLog.instance != null)
            ServerLog.instance.Log("Player lobby:" + conn.address + " disconnected");
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            PlayerLobby p = lobbySlots[i] as PlayerLobby;

            if (p != null)
            {
                p.RpcUpdatePlayerLobby();
            }
        }
    }

    //服务器玩家准备
    public override void OnLobbyServerPlayersReady()
    {
        if (AutoStart)
            StartServerGame();
    }

    //计大厅算玩家数量
    public int GetLobbyPlayerCount()
    {
        int playercount = 0;
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            if (lobbySlots[i] != null)
            {
                playercount += 1;
            }
        }
        return playercount;
    }

    //服务器端游戏开始
    public bool StartServerGame()
    {
        bool isallready = true;
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            if (lobbySlots[i] != null)
            {
                if (!lobbySlots[i].readyToBegin)
                {
                    Debug.Log("Can't Start!, Someone is not ready");
                    isallready = false;
                }
            }
        }
        if (isallready)
        {
            if (!startingServer)
            {
                StartCoroutine(ServerCountdownCoroutine());
                if (UnitZ.Hud != null)
                    UnitZ.Hud.ProcessPopup.SetActive(true);
            }
        }
        //全部准备
        return isallready;
    }

    bool startingServer;
    
    //服务器计数-协程
    public IEnumerator ServerCountdownCoroutine()
    {
        startingServer = true;
        float remainingTime = 1;
        int floorTime = Mathf.FloorToInt(remainingTime);

        while (remainingTime > 0)
        {
            yield return null;
            remainingTime -= Time.deltaTime;
            int newFloorTime = Mathf.FloorToInt(remainingTime);

            if (newFloorTime != floorTime)
            {
                //to avoid flooding the network of message, we only send a notice to client when the number of plain seconds change.
                floorTime = newFloorTime;
                if (ServerLog.instance != null)
                    ServerLog.instance.Log("Starting match.." + floorTime);

                for (int i = 0; i < lobbySlots.Length; ++i)
                {
                    if (lobbySlots[i] != null)
                    {
                        //there is maxPlayer slots, so some could be == null, need to test it before accessing!
                        (lobbySlots[i] as PlayerLobby).RpcUpdateCountdown(floorTime);
                    }
                }
            }
        }
        for (int i = 0; i < lobbySlots.Length; ++i)
        {
            if (lobbySlots[i] != null)
            {
                (lobbySlots[i] as PlayerLobby).RpcUpdateCountdown(0);
                Begin(lobbySlots[i].connectionToClient);
            }
        }
        startingServer = false;
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);

        ServerChangeScene(playScene);
    }

    //客户端进行回调
    public override void OnClientConnect(NetworkConnection conn)
    {
        base.OnClientConnect(conn);
        conn.RegisterHandler(MsgKicked, KickedMessageHandler);
        conn.RegisterHandler(MsgBegin, BeginMessageHandler);
        if (!NetworkServer.active)
        {
            if (MainMenuManager.menu != null)
                MainMenuManager.menu.OpenPanelByName("Lobby");
            SetServerInfo("Client", networkAddress);
        }
        if (ServerLog.instance != null)
            ServerLog.instance.Log(" Client:" + conn.address + " connected");
    }

    //客户端断开连接
    public override void OnClientDisconnect(NetworkConnection conn)
    {
        Debug.Log("on client disconnected");
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.OpenPreviousPanel();
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);
        if (ServerLog.instance != null)
            ServerLog.instance.Log(" Client:" + conn.address + " disconnected");
        base.OnClientDisconnect(conn);
    }

    //服务器端断开连接
    public override void OnServerDisconnect(NetworkConnection conn)
    {
        Debug.Log("on server disconnected");
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.OpenPreviousPanel();
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);
        base.OnServerDisconnect(conn);
    }

    //主机停止
    public override void OnStopHost()
    {
        Debug.Log("on stop host");
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);

        if (UnitZ.dedicatedManager != null)
            UnitZ.dedicatedManager.OnDisconnect();

        base.OnStopHost();
    }

    //服务器停止
    public override void OnStopServer()
    {
        Debug.Log("on stop server");
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);

        if (UnitZ.dedicatedManager != null)
            UnitZ.dedicatedManager.OnDisconnect();
        base.OnStopServer();
    }

    //客户端报错
    public override void OnClientError(NetworkConnection conn, int errorCode)
    {
        if (MainMenuManager.menu != null)
            MainMenuManager.menu.OpenPreviousPanel();
        if (UnitZ.Hud != null)
            UnitZ.Hud.ProcessPopup.SetActive(false);
        Debug.Log("Cient error : " + errorCode.ToString());
    }

    //玩家数量更改
    public void OnPlayersNumberModified(int count)
    {
        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;
    }

    //踢出玩家
    class KickMsg : MessageBase { }
    public void KickPlayer(NetworkConnection conn)
    {
        conn.Send(MsgKicked, new KickMsg());
    }
    class BeginkMsg : MessageBase { }
    public void Begin(NetworkConnection conn)
    {
        conn.Send(MsgBegin, new BeginkMsg());
    }

    public void KickedMessageHandler(NetworkMessage netMsg)
    {
        Debug.Log("Kicked by Server");
        netMsg.conn.Disconnect();
    }

    public void BeginMessageHandler(NetworkMessage netMsg)
    {
        Debug.Log("Game is begin..");
        this.SendMessage("OnGameIsBegin", SendMessageOptions.DontRequireReceiver);
    }

    //断开连接
    public void Disconnect()
    {
        MatchSelected = null;
        if (NetworkServer.connections.Count > 0)
        {
            Debug.Log("stop host");
            singleton.StopHost();
        }
        else
        {
            Debug.Log("stop client");
            singleton.StopClient();
        }
    }

    //角色查询
    public void RequestSpawnPlayer(Vector3 position, int connectid, string userid, string usename, int characterindex, string characterkey, byte team, int spawnpoint, NetworkConnection conn)
    {
        GameObject player = UnitZ.playerManager.InstantiatePlayer(connectid, userid, usename, characterkey, characterindex, team, spawnpoint);
        if (player == null)
            return;

        player.GetComponent<CharacterSystem>().NetID = connectid;
        NetworkServer.ReplacePlayerForConnection(conn, player, 0);
        Debug.Log("Spawn player Net ID" + connectid + " info " + characterindex + " key " + characterkey);
    }

    //物体查询
    public GameObject RequestSpawnObject(GameObject gameobj, Vector3 position, Quaternion rotation)
    {
        GameObject obj = (GameObject)Instantiate(gameobj, position, rotation);
        NetworkServer.Spawn(obj);
        return obj;
    }

    //物体查询
    public GameObject RequestSpawnItem(GameObject gameobj, int numtag, int num, Vector3 position, Quaternion rotation)
    {
        //Debug.Log("Request spawn object : "+gameobj+" numtag : "+numtag+" num : "+num);
        GameObject obj = (GameObject)Instantiate(gameobj, position, rotation);
        ItemData data = (ItemData)obj.GetComponent<ItemData>();
        data.SetupDrop(numtag, num);
        NetworkServer.Spawn(obj);
        return obj;
    }

    //回调查询
    public GameObject RequestSpawnBackpack(GameObject gameobj, string backpackdata, Vector3 position, Quaternion rotation)
    {
        //Debug.Log("Request spawn object : "+gameobj+" numtag : "+numtag+" num : "+num);
        GameObject obj = (GameObject)Instantiate(gameobj, position, rotation);
        ItemBackpack data = (ItemBackpack)obj.GetComponent<ItemBackpack>();
        data.SetDropItem(backpackdata);
        NetworkServer.Spawn(obj);
        return obj;
    }
}
