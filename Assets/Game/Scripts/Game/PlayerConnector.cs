using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

//玩家连接
public class PlayerConnector : NetworkBehaviour
{
    [SyncVar]
    public GameObject PlayingCharacter;
    public NetworkInstanceId NetID;
    public float SpawnwDelay = 1;
    public bool AutoSpawn = true;

    [SyncVar]
    public int ConnectID = -1;
    private float timetmp;

    void Start()
    {
        timetmp = Time.time;
        Debug.Log("Spawn Player connecter server " + isServer);
    }

    public override void OnStartLocalPlayer()
    {
        GetNetID();
        base.OnStartLocalPlayer();
    }

    void Update()
    {
        if (!isLocalPlayer || ConnectID == -1)
            return;

        if (isLocalPlayer && isServer)
        {
            if (UnitZ.gameNetwork.IsDedicatedServer)
            {
                if (UnitZ.dedicatedManager != null)
                    UnitZ.dedicatedManager.CurrentPlayer = this;
                return;
            }
        }

        if (PlayingCharacter == null)
        {
            if (UnitZ.playerManager.PlayingCharacter != null)
            {
                PlayingCharacter = UnitZ.playerManager.PlayingCharacter.gameObject;

                if (PlayingCharacter != null)
                    CmdTellServerMyCharacter(PlayingCharacter.gameObject);
            }
        }

        UnitZ.gameManager.PlayerNetID = ConnectID;

        if (isLocalPlayer && PlayingCharacter == null && AutoSpawn)
        {
            if (Time.time > timetmp + SpawnwDelay)
            {
                CmdRequestSpawnPlayer(Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey, -1);
                Destroy(this.gameObject);
                timetmp = Time.time;
            }
        }
    }

    public void RequestSpawnWithTeam(byte team, int spawnpoint)
    {
        CmdRequestSpawnWithTeam(Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey, team, spawnpoint);
        Destroy(this.gameObject);
    }

    public void RequestSpawn(int spawnpoint)
    {
        CmdRequestSpawnPlayer(Vector3.zero, ConnectID, UnitZ.gameManager.UserID, UnitZ.gameManager.UserName, UnitZ.characterManager.CharacterIndex, UnitZ.gameManager.CharacterKey, spawnpoint);
        Destroy(this.gameObject);
    }

    [Command]
    public void CmdRequestSpawnPlayer(Vector3 position, int connectid, string userid, string usename, int characterindex, string characterkey, int spawn)
    {
        UnitZ.gameNetwork.RequestSpawnPlayer(position, connectid, userid, usename, characterindex, characterkey, 0, spawn, this.connectionToClient);
        NetworkServer.Destroy(this.gameObject);
    }

    [Command]
    public void CmdRequestSpawnWithTeam(Vector3 position, int connectid, string userid, string usename, int characterindex, string characterkey, byte team, int spawn)
    {
        UnitZ.gameNetwork.RequestSpawnPlayer(position, connectid, userid, usename, characterindex, characterkey, team, spawn, this.connectionToClient);
        NetworkServer.Destroy(this.gameObject);
    }

    [Client]
    void GetNetID()
    {
        NetID = this.GetComponent<NetworkIdentity>().netId;
        ConnectID = (int)NetID.Value;
        CmdTellServerMyInfo((int)NetID.Value, UnitZ.gameManager.UserName, UnitZ.gameManager.Team, UnitZ.GameKeyVersion);
    }

    [Command]
    void CmdTellServerMyInfo(int id, string username, string team, string gamekey)
    {
        ConnectID = id;
        if (UnitZ.NetworkGameplay && UnitZ.NetworkGameplay.playersManager)
            UnitZ.NetworkGameplay.playersManager.UpdatePlayerInfo(id, 0, 0, username, team, gamekey, true);

        Debug.Log("NetID "+id + " has connect to the server");
    }

    [Command]
    void CmdTellServerMyCharacter(GameObject player)
    {
        PlayingCharacter = player;
    }
}
