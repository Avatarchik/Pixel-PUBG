using UnityEngine;
using System.Collections;
using System.Collections.Generic;


//该类用于减少每个AI对象中FindGameObjectsWithTag函数的调用
//每一帧循环中FindGameObjectsWithTag调用次数太多了
public class AIManager : MonoBehaviour
{
    public Dictionary<string, TargetCollector> TargetList = new Dictionary<string, TargetCollector>();
    public int TargetTypeCount = 0;
    public float UpdateInterval = 0.1f;
    private float timeTmp;
    public string PlayerTag = "Player";

    void Start()
    {
        TargetList = new Dictionary<string, TargetCollector>();
    }

    public void Clear()
    {
        foreach (var target in TargetList)
        {
            if (target.Value != null)
                target.Value.Clear();
        }
        TargetList.Clear();
        TargetList = new Dictionary<string, TargetCollector>(0);
    }

    //找到目标Tag
    public TargetCollector FindTargetTag(string tag)
    {

        if (TargetList.ContainsKey(tag))
        {
            TargetCollector targetcollector;
            if (TargetList.TryGetValue(tag, out targetcollector))
            {
                targetcollector.IsActive = true;
                return targetcollector;
            }
            else
            {
                return null;
            }
        }
        else
        {
            TargetList.Add(tag, new TargetCollector(tag));
        }
        return null;
    }

    void Update()
    {
        if (Time.time > timeTmp + UpdateInterval)
        {
            int count = 0;

            foreach (var target in TargetList)
            {
                if (target.Value != null)
                {
                    if (target.Value.IsActive)
                    {
                        target.Value.SetTarget(target.Key);
                        target.Value.IsActive = false;
                        count += 1;
                    }
                }
            }
            TargetTypeCount = count;
            timeTmp = Time.time;
        }
    }

    //玩家是否在周围
    public bool IsPlayerAround(Vector3 position, float distance)
    {
        TargetCollector player = FindTargetTag(PlayerTag);
        if (player != null && player.Targets.Length > 0)
        {
            for (int i = 0; i < player.Targets.Length; i++)
            {
                if (player.Targets[i] != null)
                {
                    if (Vector3.Distance(position, player.Targets[i].transform.position) <= distance)
                    {
                        return true;
                    }
                }
            }
        }

        return false;
    }

}

public class TargetCollector
{
    public GameObject[] Targets;
    public bool IsActive;

    public TargetCollector(string tag)
    {
        SetTarget(tag);
    }
    public void Clear()
    {
        Targets = null;
    }
    public void SetTarget(string tag)
    {
        Targets = (GameObject[])GameObject.FindGameObjectsWithTag(tag);
    }

}

