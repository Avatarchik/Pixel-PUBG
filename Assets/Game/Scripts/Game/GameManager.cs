//游戏管理
using UnityEngine;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{
	public string UserName = "";    //玩家名字
	public string Team = "";        //队伍名字
	public string UserID = "";      //用户ID
	public string CharacterKey = "";//角色key
    public int PlayerNetID = -1;    //网络ID
    public int BotNumber = 1;       //机器人数量
    public int BotMax = 20;         //机器人最大数量

    [HideInInspector]
	public bool IsRefreshing = false;
	[HideInInspector]
	public AsyncOperation loadingScene;
    [HideInInspector]
    public bool IsPlaying = false;
    [HideInInspector]
    public bool IsBattleStart = false;

    //初始化游戏
    void Awake ()
	{
		DontDestroyOnLoad (this.gameObject);
	}

    //更新预加载
    public void UpdateProfile()
    {
        PlayerPrefs.SetString("user_name", UserName);
    }

    //游戏开始
	void Start ()
	{
		UserName = PlayerPrefs.GetString ("user_name");
	}

    //重新开始
	public void RestartGame ()
	{
		if (UnitZ.playerManager != null)
			UnitZ.playerManager.Reset ();
	}

    //退出游戏
    public void QuitGame ()
	{
		if (UnitZ.NetworkGameplay != null)
			UnitZ.NetworkGameplay.chatLog.Clear ();
		
		if (UnitZ.playerManager != null)
			UnitZ.playerManager.Reset ();

        UnitZ.gameNetwork.Disconnect ();
        UnitZ.aiManager.Clear();
    }

    //更换游戏场景
	public void StartLoadLevel (string level)
	{
		loadingScene = SceneManager.LoadSceneAsync(level);
	}

}
