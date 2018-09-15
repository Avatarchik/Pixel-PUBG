using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

//飞机
public class Airplane : Vehicle {
    [Header("Airplane")]
    public float FlyDistance = 1000;
    public float EjectDistance = 500;
    public float Speed = 20;
    public bool DropSupply;
    public GameObject DropObject;
    private Vector3 initialPos;

	void Start () {
        initialPos = this.transform.position;
        dropped = false;
        ejected = false;
    }

    private Vector3 dropPos;
    public void SetDrop(Vector3 position)
    {
        dropPos = position;
        dropPos.y = 0;
    }

    bool dropped;
    [Command(channel = 0)]
    public void CmdDrop()
    {
        if (!dropped)
        {
            UnitZ.gameNetwork.RequestSpawnObject(DropObject, this.transform.position, DropObject.transform.rotation);
            dropped = true;
        }
    }

    [Command(channel = 0)]
    public void CmdRemove()
    {
        RpcRemove();
    }

    [ClientRpc(channel = 0)]
    public void RpcRemove()
    {
        Destroy(this.gameObject);
    }

    bool ejected = false;
    void Update () {
        UpdateFunction();
        this.transform.position += this.transform.forward * Speed * Time.deltaTime;

        if (isServer)
        {
            if (DropSupply)
            {
                Vector3 planePos = this.transform.position;
                planePos.y = 0;
                if (Vector3.Distance(planePos, dropPos) <= 20)
                {
                    CmdDrop();
                }
            }
            float distance = Vector3.Distance(initialPos, this.transform.position);
            if(distance >= EjectDistance)
            {
                if (!ejected)
                {
                    EjectAllSeat();
                    ejected = true;
                }
            }
            if (distance >= FlyDistance)
            {
                CmdRemove();
            }
        }
    }
}
