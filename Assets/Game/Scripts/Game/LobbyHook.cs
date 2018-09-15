using UnityEngine;
using UnityEngine.Networking;

public abstract class LobbyHook : MonoBehaviour
{
    public virtual void OnLobbyServerSceneLoadedForPlayer(NetworkManager manager, GameObject lobbyPlayer, GameObject gamePlayer) { }
}


