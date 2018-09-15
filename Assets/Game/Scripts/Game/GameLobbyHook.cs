using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

public class GameLobbyHook : LobbyHook
{
    public void Start()
    {
        
    }
    public override void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer)
    {
        PlayerLobby lobby = lobbyPlayer.GetComponent<PlayerLobby>();
    }
}
