using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;
using UnityEngine.Networking.Types;
using System.Collections;
using UnityEngine.UI;

public class GUIGameRoom : MonoBehaviour
{

    public Text RoomName;
    public MatchInfoSnapshot Match;

    void Start()
    {
    }

    public void JoinRoom()
    {
        if (UnitZ.gameNetwork)
        {
            UnitZ.gameNetwork.MatchSelect(Match);
            if (MainMenuManager.menu != null)
            {
                MainMenuManager.menu.EnterGame(UnitZGameType.Connect);
            }
        }
    }
}
