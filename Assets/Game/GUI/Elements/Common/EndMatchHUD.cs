using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EndMatchHUD : MonoBehaviour
{

    public Text PlayerName;
    public Text KillCount;
    public Text Rank;
    public Text Rank2;

    void Start()
    {

    }

    private void OnEnable()
    {
        PlayerData playerData = UnitZ.NetworkGameplay.playersManager.GetPlayerData(UnitZ.gameManager.PlayerNetID);

        if (PlayerName)
            PlayerName.text = playerData.Name;

        if (KillCount)
            KillCount.text = playerData.Score.ToString();

        if (Rank)
            Rank.text = "#" + UnitZ.NetworkGameplay.EndgameRanked.ToString();

        if (Rank2)
            Rank2.text = "#" + UnitZ.NetworkGameplay.EndgameRanked.ToString() + "/" + UnitZ.NetworkGameplay.PlayersAliveMax.ToString();
    }

    public void EnableSpectre(bool on)
    {
        if (UnitZ.playerManager)
            UnitZ.playerManager.SpectreMode(on);
    }

    public void QutGame()
    {
        this.gameObject.SetActive(false);
        if (UnitZ.gameManager)
        {
            UnitZ.gameManager.QuitGame();
        }
    }
}
