using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]

//物品生成
public class ItemSpawner : NetworkBehaviour
{
    public string PlayerTag = "Player";
    public bool SpawnByPlayerEnter = true;
    public bool SpawnJustOnce = true;
    public bool SpawnOnStart = true;
    public float timeSpawn = 120;
    public ItemData[] Item;
    public int ItemMax = 3;
    public Vector3 Offset = new Vector3(0, 0.1f, 0);
    private float timeTemp = 0;
    private List<GameObject> itemList = new List<GameObject>();
    bool alreadySpawned = false;

    void Start()
    {
        alreadySpawned = false;
        this.GetComponent<NetworkIdentity>().serverOnly = true;
        full = false;
    }

    public override void OnStartClient()
    {
        full = false;
        if (SpawnOnStart)
            Spawn();
        base.OnStartClient();
    }

    bool full = false;

    void Spawn()
    {
        if (!isServer)
            return;

        ItemData itemPick = null;
        Vector3 spawnPoint = DetectGround(transform.position + new Vector3(Random.Range(-(int)(this.transform.localScale.x / 2.0f), (int)(this.transform.localScale.x / 2.0f)), 0, Random.Range((int)(-this.transform.localScale.z / 2.0f), (int)(this.transform.localScale.z / 2.0f))));

        if (Item.Length > 0)
        {
            itemPick = Item[Random.Range(0, Item.Length)];
        }
        else
        {
            itemPick = UnitZ.itemManager.ItemsList[Random.Range(0, UnitZ.itemManager.ItemsList.Length)];
        }

        if (itemPick != null)
        {
            GameObject objitem = UnitZ.gameNetwork.RequestSpawnItem(itemPick.gameObject, itemPick.NumTag, itemPick.Quantity, spawnPoint, Quaternion.identity);
            if (objitem)
                itemList.Add(objitem);
        }
    }

    private int ObjectsNumber;

    void ObjectExistCheck()
    {
        ObjectsNumber = 0;
        foreach (var obj in itemList)
        {
            if (obj != null)
                ObjectsNumber++;
        }
    }

    void Update()
    {
        if (!isServer || (SpawnJustOnce && alreadySpawned))
            return;

        bool active = false;
        if (SpawnByPlayerEnter)
        {
            TargetCollector players = UnitZ.aiManager.FindTargetTag(PlayerTag);
            if (players != null && players.Targets != null)
            {
                for (int p = 0; p < players.Targets.Length; p++)
                {
                    if (players.Targets[p] != null)
                    {
                        if (Vector3.Distance(this.transform.position, players.Targets[p].transform.position) < this.transform.localScale.x)
                        {
                            active = true;
                        }
                    }
                }
            }
        }
        else
        {
            active = true;
        }

        if (!active)
            return;

        if (SpawnJustOnce)
        {
            if (!alreadySpawned)
            {
                alreadySpawned = true;
                int itemcount = Random.Range(1, ItemMax + 1);
                for (int i = 0; i < itemcount; i++)
                {
                    Spawn();
                }
            }
        }
        else
        {
            ObjectExistCheck();

            if (ObjectsNumber >= ItemMax)
            {
                full = true;
            }
            else
            {
                if (full)
                {
                    timeTemp = Time.time;
                    full = false;
                }

                if (Time.time > timeTemp + timeSpawn)
                {
                    Spawn();
                    timeTemp = Time.time;
                }
            }
        }
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.blue;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawWireCube(transform.position, this.transform.localScale);
    }

    Vector3 DetectGround(Vector3 position)
    {
        RaycastHit hit;
        if (Physics.Raycast(position, -Vector3.up, out hit, 1000.0f))
        {
            return hit.point + Offset;
        }
        return position;
    }
}
