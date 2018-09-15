//管理器
//初始化各部分管理器，进行各种资源的设置管理

using UnityEngine;
using System.Collections;

public class UnitZManager : MonoBehaviour
{   
	public int TargetFrameRate = 60;    //目标帧数
    public bool IsMobile = false;       //默认非手机

    void Start()
    {
		Application.targetFrameRate = TargetFrameRate;
    }

    void Awake()
    {
        //游戏网络管理
        UnitZ.gameNetwork = (GameNetwork)GameObject.FindObjectOfType(typeof(GameNetwork));
        //游戏逻辑管理
        UnitZ.gameManager = (GameManager)GameObject.FindObjectOfType(typeof(GameManager));
        //角色管理
        UnitZ.characterManager = (CharacterManager)GameObject.FindObjectOfType(typeof(CharacterManager));
        //物品管理
        UnitZ.itemManager = (ItemManager)GameObject.FindObjectOfType(typeof(ItemManager));
        //物品合成管理
        UnitZ.itemCraftManager = (ItemCrafterManager)GameObject.FindObjectOfType(typeof(ItemCrafterManager));
        //玩家管理
        UnitZ.playerManager = (PlayerManager)GameObject.FindObjectOfType(typeof(PlayerManager));
        //玩家存储
        UnitZ.playerSave = (PlayerSave)GameObject.FindObjectOfType(typeof(PlayerSave));
        //关卡管理
        UnitZ.levelManager = (LevelManager)GameObject.FindObjectOfType(typeof(LevelManager));
        //AI管理
        UnitZ.aiManager = (AIManager)GameObject.FindObjectOfType(typeof(AIManager));
        //显示器管理
        UnitZ.Hud = (PlayerHUDCanvas)GameObject.FindObjectOfType(typeof(PlayerHUDCanvas));
        //专用服务器管理
        UnitZ.dedicatedManager = (DedicatedManager)GameObject.FindObjectOfType(typeof(DedicatedManager));
        //游戏key
        UnitZ.GameKeyVersion = Application.version;
        //手机/电脑
        UnitZ.IsMobile = IsMobile;
    }
}
public static class UnitZ
{
    public static AIManager aiManager;
    public static GameNetwork gameNetwork;
    public static GameManager gameManager;
    public static CharacterManager characterManager;
    public static ItemManager itemManager;
    public static ItemCrafterManager itemCraftManager;
    public static PlayerManager playerManager;
    public static PlayerSave playerSave;
    public static LevelManager levelManager;
    public static PlayerHUDCanvas Hud;
    public static string GameKeyVersion = "";
    public static bool IsOnline = false;
    public static NetworkGameplayManager NetworkGameplay;
    public static DedicatedManager dedicatedManager;
    public static bool IsMobile = false;

}