using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public enum UnitZGameType
{
    Connect, HostOnline, HostLan, Single
}

public class MainMenuManager : PanelsManager
{
    public static MainMenuManager menu = null;
    public string SceneStart = "zombieland";
    public Text CharacterName;
    public GameObject Preloader;
    [HideInInspector]
    public UnitZGameType StartType = UnitZGameType.Single;
    private CharacterCreatorCanvas characterCreator;

    void Start()
    {
        UnitZ.gameManager.IsPlaying = false;
        UnitZ.Hud.ResetAllHud();
        menu = this;
        MouseLock.MouseLocked = false;
        characterCreator = (CharacterCreatorCanvas)GameObject.FindObjectOfType(typeof(CharacterCreatorCanvas));
        // load latest scene played
        if (PlayerPrefs.GetString("StartScene") != "")
        {
            SceneStart = PlayerPrefs.GetString("StartScene");
        }
    }

    void Update()
    {
        if (CharacterName && UnitZ.gameManager)
        {
            CharacterName.text = UnitZ.gameManager.UserName;
        }
    }

    public void LevelSelected(string name)
    {
        SceneStart = name;
        PlayerPrefs.SetString("StartScene", SceneStart);

    }

    public void StopFindGame()
    {
        if (UnitZ.gameNetwork)
            UnitZ.gameNetwork.StopMatchMaker();
    }

    public void ConnectIP()
    {
        EnterGame(UnitZGameType.Connect);
    }

    public void StartSinglePlayer()
    {
        if (UnitZ.gameManager)
        {
            UnitZ.gameNetwork.StopMatchMaker();
            EnterGame(UnitZGameType.Single);
        }
    }

    public void HostGameOnline()
    {
        if (UnitZ.gameNetwork)
        {
            UnitZ.gameNetwork.StartMatchMaker();
            EnterGame(UnitZGameType.HostOnline);
        }
    }
    public void HostGame()
    {
        if (UnitZ.gameNetwork)
        {
            UnitZ.gameNetwork.StopMatchMaker();
            EnterGame(UnitZGameType.HostLan);
        }
    }

    public void FindInternetGame()
    {
        UnitZ.gameNetwork.StartMatchMaker();
        OpenPanelByName("FindGame");
    }

    public void EnterGame(UnitZGameType type)
    {
        StartType = type;
        if (characterCreator)
            characterCreator.SetCharacter();

        switch (StartType)
        {
            case UnitZGameType.HostOnline:
                Debug.Log("Host Game Online");
                UnitZ.gameNetwork.HostGame(SceneStart, true);
                break;
            case UnitZGameType.HostLan:
                Debug.Log("Host Game");
                UnitZ.gameNetwork.HostGame(SceneStart, false);
                break;
            case UnitZGameType.Connect:
                Debug.Log("Connect Game");
                UnitZ.gameNetwork.JoinGame();
                break;
            case UnitZGameType.Single:
                Debug.Log("Single Game");
                UnitZ.gameNetwork.HostGame(SceneStart, false);
                UnitZ.gameNetwork.StartServerGame();
                break;
        }  
    }

    public void ConnectingDeny()
    {
        UnitZ.gameNetwork.Disconnect();
    }

    public void ExitGame()
    {
        UnitZ.gameNetwork.Disconnect();
        Application.Quit();
    }
    
        
    public void OpenWebsite(string url){
        Application.OpenURL(url);   
    }

}
