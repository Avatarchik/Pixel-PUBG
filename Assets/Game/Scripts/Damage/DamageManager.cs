using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(AudioSource))]
//伤害管理
public class DamageManager : NetworkBehaviour
{
    [Header("Living")]
    public bool IsAlive = true;
    [SyncVar(hook = "OnHPChanged")]
    public byte HP = 100;
    public byte HPmax = 100;

    public GameObject DeadReplacement;
    public float DeadReplaceLifeTime = 180;
    public bool DestroyOnDead = true;
    public AudioClip[] SoundPain;
    public AudioSource Audiosource;


    [HideInInspector]
    public bool dieByLifeTime = false;
    [HideInInspector]
    public bool spectreThis = false;
    [HideInInspector]
    [SyncVar]
    public byte Team = 0;
    //[HideInInspector]
    [SyncVar]
    public int NetID = -1;
    [HideInInspector]
    [SyncVar]
    public string UserID = "";
    [HideInInspector]
    [SyncVar]
    public string UserName = "";
    [HideInInspector]
    [SyncVar]
    public int LastHitByID = -1;
    private Vector3 directionHit;

    void Start()
    {
        Audiosource = this.GetComponent<AudioSource>();
    }

    public override void OnStartClient()
    {
        if (HP <= 0)
        {
            SetEnable(false);
        }

        base.OnStartClient();
    }

    void Update()
    {
        DamageUpdate();
    }

    public void DamageUpdate()
    {
        if (HP > HPmax)
            HP = HPmax;
    }

    public void DirectDamage(DamagePackage pack)
    {
        ApplyDamage((byte)((float)pack.Damage), pack.Direction, pack.ID, pack.Team);
    }

    public void ApplyDamage(byte damage, Vector3 direction, int attackerID, byte team)
    {
        directionHit = direction;
        DoApplyDamage(damage, direction, attackerID, team);
        if (Audiosource && SoundPain.Length > 0)
        {
            Audiosource.PlayOneShot(SoundPain[Random.Range(0, SoundPain.Length)]);
        }
    }

    public void DoApplyDamage(byte damage, Vector3 direction, int attackerID, byte team)
    {
        if (isServer)
        {
            if (UnitZ.gameManager && !UnitZ.gameManager.IsBattleStart)
                return;

            lastHP = HP;
            directionHit = direction;
            LastHitByID = attackerID;
            if (Team != team || team == 0)
            {
                if (HP <= 0)
                    return;

                if (damage >= HP)
                {
                    HP = 0;
                    CmdOnDead(LastHitByID, NetID, "Kill");
                    return;
                }
                else
                {
                    HP -= damage;
                }

                if (HP <= 0)
                {
                    CmdOnDead(LastHitByID, NetID, "Kill");
                }
            }
        }
    }

    private byte lastHP = 0;
    private bool alreadyDead = false;
    void OnHPChanged(byte hp)
    {
        if (hp <= 0)
        {
            SetEnable(false);
        }
        else
        {
            if (hp >= HPmax)
            {
                if (!IsAlive)
                {
                    SetEnable(true);
                }
            }
        }

        if (!IsAlive && lastHP > 0)
        {
            if (!alreadyDead)
            {
                SpawnDeadBody();
                alreadyDead = true;
            }
        }

        lastHP = HP;
        HP = hp;
    }


    void SpawnDeadBody()
    {
        if (!isQuitting)
        {
            if (DeadReplacement)
            {
                GameObject deadbody = (GameObject)GameObject.Instantiate(DeadReplacement, this.transform.position, Quaternion.identity);
                if (spectreThis && deadbody)
                {
                    LookAfterDead(deadbody.gameObject);
                }
                CopyTransformsRecurse(this.transform, deadbody);
                if (dieByLifeTime)
                    DeadReplaceLifeTime = 3;
                GameObject.Destroy(deadbody, DeadReplaceLifeTime);
            }
        }
    }

    [Command(channel = 0)]
    void CmdOnDead(int killer, int me, string killtype)
    {
        RpcODead(killer, me, killtype);
    }

    [ClientRpc(channel = 0)]
    void RpcODead(int killer, int me, string killtype)
    {
        OnThisThingDead();
        OnKilled(killer, me, killtype);
        if (DestroyOnDead)
            GameObject.Destroy(this.gameObject, 5);
    }

    public void ReSpawn(byte team, int spawner)
    {
        CmdRespawn(team, spawner);
    }

    public void ReSpawn(int spawner)
    {
        CmdRespawn(Team, spawner);
    }

    [Command(channel = 0)]
    void CmdRespawn(byte team, int spawner)
    {
        HP = HPmax;
        Team = team;
        RpcRespawn(team, spawner);
    }

    [ClientRpc(channel = 0)]
    void RpcRespawn(byte team, int spawner)
    {
        HP = HPmax;
        Team = team;
        if (isLocalPlayer)
        {
            this.transform.position = UnitZ.playerManager.FindASpawnPoint(spawner);
            Debug.Log("RPC respawn");
        }
        OnRespawn();
        this.SendMessage("Respawn", SendMessageOptions.DontRequireReceiver);
    }


    public virtual void SetEnable(bool enable)
    {
        IsAlive = enable;

        foreach (Transform ob in this.transform)
        {
            ob.gameObject.SetActive(enable);
        }

        Renderer[] renderers = this.GetComponentsInChildren<Renderer>();
        foreach (Renderer ob in renderers)
        {
            ob.enabled = enable;
        }
    }

    public virtual void OnKilled(int killer, int me, string killtype)
    {
        // Do something when get killed
    }

    public virtual void OnThisThingDead()
    {
        // Do something when dying
    }

    public virtual void OnRespawn()
    {
        alreadyDead = false;
        // Do something when respawn
    }

    public virtual void OnDestroyed()
    {
        // De something before removed
    }

    public void CopyTransformsRecurse(Transform src, GameObject dst)
    {
        dst.transform.position = src.position;
        dst.transform.rotation = src.rotation;
        if (dst.GetComponent<Rigidbody>())
            dst.GetComponent<Rigidbody>().AddForce(directionHit * 5, ForceMode.VelocityChange);

        foreach (Transform child in dst.transform)
        {
            var curSrc = src.Find(child.name);
            if (curSrc)
            {
                CopyTransformsRecurse(curSrc, child.gameObject);
            }
        }
    }

    void LookAfterDead(GameObject obj)
    {
        //玩家死亡后，摄像机看向尸体
        SpectreCamera spectre = (SpectreCamera)GameObject.FindObjectOfType(typeof(SpectreCamera));
        if (spectre)
        {
            spectre.LookingAtObject(obj);
        }
    }

    private bool isQuitting = false;

    void OnApplicationQuit()
    {
        isQuitting = true;
    }

    public override void OnNetworkDestroy()
    {
        if (isQuitting)
            return;

        OnDestroyed();
        if (isServer)
        {
            if (NetID != -1)
            {
                if (UnitZ.NetworkGameplay.playersManager != null)
                {
                    UnitZ.NetworkGameplay.playersManager.RemovePlayer(NetID);
                }
            }
        }
        base.OnNetworkDestroy();
    }


}
