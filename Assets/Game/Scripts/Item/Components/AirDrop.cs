using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class AirDrop : NetworkBehaviour
{

    public GameObject Parachute;
    public ItemData[] Item;
    public int ItemNum = 3;
    public Vector3 Offset = new Vector3(0, 0.1f, 0);
    private List<GameObject> itemList = new List<GameObject>();
    private bool unpackaged = false;
    public string PlayerTag = "Player";
    public float DistanceUnpack = 3;
    private Rigidbody rig;
    public float FloatForce = 10;
    void Start()
    {
        rig = this.GetComponent<Rigidbody>();
    }

    void OnCollisionEnter(Collision collision)
    {
        if (Parachute)
            Parachute.SetActive(false);
    }

    void Update()
    {
        if (!isServer)
            return;

        if (Parachute)
        {
            if (Parachute.activeSelf)
            {
                if (rig)
                    rig.AddForce(Vector3.up * FloatForce);
            }
        }
        if (!unpackaged)
        {
            TargetCollector players = UnitZ.aiManager.FindTargetTag(PlayerTag);
            if (players != null && players.Targets != null)
            {
                for (int p = 0; p < players.Targets.Length; p++)
                {
                    if (players.Targets[p] != null)
                    {
                        if (Vector3.Distance(this.transform.position, players.Targets[p].transform.position) < DistanceUnpack)
                        {
                            Spawn();
                            unpackaged = true;
                        }
                    }
                }
            }
        }
    }

    void Spawn()
    {
        if (!isServer)
            return;

        for (int i = 0; i < ItemNum; i++)
        {
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
            }
        }
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
