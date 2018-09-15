using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.Networking;

//计分管理
public class ScoreManager : NetworkBehaviour
{
    public bool Toggle;
    private GUIKillBadgeManager guiBadgeManager;

    void Start()
    {
        guiBadgeManager = (GUIKillBadgeManager)GameObject.FindObjectOfType(typeof(GUIKillBadgeManager));
    }

    void Update()
    {
        if (Input.GetKeyDown(KeyCode.Tab))
        {
            if (UnitZ.gameManager)
                Toggle = !Toggle;
        }
    }

    private void UpdatePlayerScore(int id, int score, int dead)
    {
        //更新角色分数到服务器
        if (!isServer || UnitZ.NetworkGameplay == null || UnitZ.NetworkGameplay.playersManager == null)
            return;

        UnitZ.NetworkGameplay.playersManager.AddScore(id, score, dead);
    }

    public void AddScore(int score, int id)
    {
        //增加击杀分数
        UpdatePlayerScore(id, score, 0);
    }

    public void AddDead(int dead, int id)
    {
        //增加死亡分数
        UpdatePlayerScore(id, 0, dead);
    }

    public void UpdateScore(int killer, int victim, int score)
    {
        if (isServer)
            CmdScoreUpdate(killer, victim, score);
    }

    [Command(channel = 0)]
    private void CmdScoreUpdate(int killer, int victim, int score)
    {
        if (victim != -1)
            UpdatePlayerScore(victim, 0, 1);
        if (killer != -1)
            UpdatePlayerScore(killer, score, 0);

        int killerscore = 0;
        PlayersManager playersManager = UnitZ.NetworkGameplay.playersManager;
        if (playersManager != null)
        {
            PlayerData killerData = playersManager.GetPlayerData(killer);
            killerscore = killerData.Score;
        }
        RpcScoreReceived(killer, victim, killerscore);
    }

    [ClientRpc(channel = 0)]
    private void RpcScoreReceived(int killer, int victim, int score)
    {
        AddKillText(killer, victim, "Kill", score);
    }


    public void AddKillText(int killer, int victim, string killtype, int killerscore)
    {
        if (guiBadgeManager == null)
            guiBadgeManager = (GUIKillBadgeManager)GameObject.FindObjectOfType(typeof(GUIKillBadgeManager));

        if (UnitZ.NetworkGameplay == null || UnitZ.NetworkGameplay.playersManager == null)
            return;

        PlayersManager playersManager = UnitZ.NetworkGameplay.playersManager;

        if (playersManager != null)
        {
            PlayerData killerData = playersManager.GetPlayerData(killer);
            PlayerData victimData = playersManager.GetPlayerData(victim);
            string killername = "N/A";
            string victimname = "N/A";

            if (killer == -1)
                killerData.Name = "Bot";

            if (victim == -1)
                victimData.Name = "Bot";

            if (killerData.Name != "")
            {
                killername = killerData.Name;
            }
            if (victimData.Name != "")
            {
                victimname = victimData.Name;
            }

            if (guiBadgeManager)
                guiBadgeManager.PushKillText(killername + "(" + killerscore + ")", victimname, killtype);


            if (killer == UnitZ.gameManager.PlayerNetID)
            {
                if (UnitZ.Hud.KillFeed)
                {
                    UnitZ.Hud.KillFeed.Kill(victimname, killername, killerscore, killtype);
                }
            }
        }
    }


}
