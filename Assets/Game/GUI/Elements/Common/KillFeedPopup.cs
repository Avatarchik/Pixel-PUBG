using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

//击杀提示
public class KillFeedPopup : MonoBehaviour
{
    public GameObject Frame;
    public Text KillCount;
    public Text KillDetail;
    public float Duration = 3;
    private float timeTmp;
    void Start()
    {

    }

    public void Kill(string victim, string killer, int killcount, string killtype)
    {
        if (KillCount != null)
        {
            KillCount.text = "Kill "+killcount.ToString();
        }
        if (KillDetail != null)
        {
            KillDetail.text = killer + " " + killtype + " " + victim;
        }
        timeTmp = Time.time;
        if (Frame != null)
            Frame.SetActive(true);
    }

    void Update()
    {
        if (Frame != null && Frame.activeSelf)
        {
            if (Time.time >= timeTmp + Duration)
            {
                Frame.SetActive(false);
            }
        }
    }
}
