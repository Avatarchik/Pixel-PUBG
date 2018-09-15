using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;

//玩家大厅
public class PlayerLobby : NetworkLobbyPlayer
{
    [SyncVar(hook = "OnMyName")]
    public string playerName = "";
    [SyncVar(hook = "OnMyTeam")]
    public byte playerTeam = 0;
    public Button RemoveButton;
    public Text Name;
    public Text Status;

    [HideInInspector]
    public bool IsReady;

    public override void OnClientEnterLobby()
    {
        GameObject lobby = GameObject.Find("LobbyInstance");
        if (lobby)
            this.transform.SetParent(lobby.transform);

        base.OnClientEnterLobby();

        if (UnitZ.gameNetwork != null)
            UnitZ.gameNetwork.OnPlayersNumberModified(1);

        if (GameLobby.instance != null)
        {
            GameLobby.instance.AddPlayer(this);
            GameLobby.instance.DisplayDirectServerWarning(isServer && UnitZ.gameNetwork.matchMaker == null);
        }
        if (isLocalPlayer)
        {
            SetupLocalPlayer();
        }
        else
        {
            SetupOtherPlayer();
        }
        OnMyName(playerName);
        OnMyTeam(playerTeam);
    }

    public override void OnStartAuthority()
    {
        base.OnStartAuthority();
        SetupLocalPlayer();
    }

    void SetupOtherPlayer()
    {
        RemoveButton.gameObject.SetActive(isServer);
        OnClientReady(readyToBegin);
    }

    void SetupLocalPlayer()
    {
        OnPlayerSetup();
        // server always ready
        if (isServer) 
            SendReadyToBeginMessage();

        if (!UnitZ.gameNetwork.IsDedicatedServer)
        {
            if (GameLobby.instance != null)
            {
                if (GameLobby.instance.playerListContentTransform != null)
                    CmdNameChanged(UnitZ.gameManager.UserName + (GameLobby.instance.playerListContentTransform.childCount - 1));
                GameLobby.instance.currentPlayerLobby = this;
            }
            CmdTeamChange();
        }
        else
        {
            if (isServer)
            {
                if (UnitZ.dedicatedManager != null)
                {
                    Debug.Log("adding player lobby");
                    UnitZ.dedicatedManager.CurrentPlayerLobby = this;
                }
                CmdNameChanged("Server");
            }
        }
        if (UnitZ.gameNetwork != null) UnitZ.gameNetwork.OnPlayersNumberModified(0);
    }

    public void OnPlayerSetup()
    {
        if (!isLocalPlayer)
            return;

        int localPlayerCount = 0;
        foreach (PlayerController p in ClientScene.localPlayers)
            localPlayerCount += (p == null || p.playerControllerId == -1) ? 0 : 1;

        if (RemoveButton != null)
            RemoveButton.interactable = localPlayerCount > 1;
    }


    public override void OnClientReady(bool readyState)
    {
        IsReady = readyState;
        if (readyState)
        {
            if (Status)
                Status.text = "Ready";
        }
        else
        {
            if (Status)
                Status.text = "Waiting";
        }
    }

    public void OnMyName(string newName)
    {
        playerName = newName;
        if (Name)
            Name.text = newName;
    }

    public void OnMyTeam(byte team)
    {
        playerTeam = team;
    }

    public void OnPlayerListChanged(int idx)
    {
        RectTransform rect = this.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(0, -idx * (rect.sizeDelta.y + 3));
    }

    public void OnReadyClicked()
    {
        SendReadyToBeginMessage();
    }

    public void OnRemovePlayerClick()
    {
        if (isLocalPlayer)
        {
            UnitZ.gameNetwork.LeaveMatch();
        }
        else if (isServer)
            UnitZ.gameNetwork.KickPlayer(connectionToClient);
    }

    [ClientRpc]
    public void RpcUpdateCountdown(int countdown)
    {
        //Debug.Log("Match Starting in " + countdown);
    }

    [ClientRpc]
    public void RpcUpdatePlayerLobby()
    {
        OnPlayerSetup();
    }

    [Command]
    public void CmdTeamChange()
    {
        playerTeam = 0;
    }

    [Command]
    public void CmdNameChanged(string name)
    {
        playerName = name;
        if (Name)
            Name.text = name;
    }

    public void OnDestroy()
    {
        if (GameLobby.instance != null)
            GameLobby.instance.RemovePlayer(this);
        if (UnitZ.gameNetwork != null) UnitZ.gameNetwork.OnPlayersNumberModified(-1);
    }
}