using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//玩家显示面板
public class PlayerHUDCanvas : PanelsManager
{
    public GUIIStockItemLoader SecondItemLoader;
    public RectTransform Canvas;
    public ValueBar HPbar, WaterBar;
    public GameObject MiniMap;
    public Minimap Map;
    public Text AmmoText;
    public Image Crosshair, CrosshairHit, CrosshairScope;
    public Text GameTime;
    public Image ImageDrag;
    public KillFeedPopup KillFeed;
    public Text AliveText;
    public GUIItemEquippedLoader Equipped;
    public Text Info;
    private bool isShowinfo;
    private float timeInfo;
    public GameObject ProcessPopup;
    public GameObject MobileController;

    public void ResetAllHud()
    {
        if (Equipped)
            Equipped.RestEquipShortcut();
        CloseAllPanels();
    }

    void Awake()
    {
        UnitZ.Hud = this;
        //确保每个面板都关闭
        if (Pages.Length > 0)
            ClosePanel(Pages[0]);
    }

    //取得世界空间位置
    public Vector2 GetWorldSpaceUIposition(Vector3 position)
    {
        if (Camera.main == null)
            return Vector3.zero;

        Vector2 ViewportPosition = Camera.main.WorldToViewportPoint(position);
        Vector2 WorldObject_ScreenPosition = new Vector2(
        ((ViewportPosition.x * Canvas.sizeDelta.x) - (Canvas.sizeDelta.x * 0.5f)),
        ((ViewportPosition.y * Canvas.sizeDelta.y) - (Canvas.sizeDelta.y * 0.5f)));
        RectTransform ui = this.GetComponent<RectTransform>();
        return WorldObject_ScreenPosition;
    }

    //展示信息
    public void ShowInfo(string info, Vector3 position)
    {
        if (Info != null)
        {
            isShowinfo = true;
            Info.gameObject.SetActive(true);
            RectTransform inforec = Info.GetComponent<RectTransform>();
            inforec.anchoredPosition = GetWorldSpaceUIposition(position);
            Info.text = info;
            timeInfo = Time.time;
        }
    }

    void Update()
    {
        if (UnitZ.playerManager == null || Canvas == null)
            return;

        bool isMouselocked = false;
        if (UnitZ.playerManager.PlayingCharacter == null || (UnitZ.playerManager.PlayingCharacter && !UnitZ.playerManager.PlayingCharacter.IsAlive))
        {
            Canvas.gameObject.SetActive(false);
            if (Crosshair)
                Crosshair.gameObject.SetActive(false);
            if (CrosshairHit)
                CrosshairHit.gameObject.SetActive(false);
            if (CrosshairScope)
                CrosshairScope.gameObject.SetActive(false);
        }
        else
        {
            HumanCharacter player = UnitZ.playerManager.PlayingCharacter.GetComponent<HumanCharacter>();
            if (player != null)
            {
                if (Equipped)
                {
                    Equipped.UpdateEquippedShortcut();
                }

                isMouselocked = true;
                Canvas.gameObject.SetActive(true);

                //地图显示
                if (Map)
                {
                    Map.Player = player.gameObject;
                    bool iscrosshairopen = false;
                    if (CrosshairScope)
                        iscrosshairopen = !CrosshairScope.gameObject.activeSelf;

                    if (Map.gameObject.activeSelf && !iscrosshairopen)
                    {
                        Map.gameObject.SetActive(false);
                    }
                }

                //血条显示
                if (HPbar)
                {
                    HPbar.Value = player.HP;
                    HPbar.ValueMax = player.HPmax;
                }

                //饮品条
                if (WaterBar && player)
                {
                    WaterBar.Value = player.Pill;
                    WaterBar.ValueMax = player.PillMax;
                }

                //提示信息
                if (AmmoText != null)
                {
                    if (UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment != null)
                        AmmoText.text = UnitZ.playerManager.PlayingCharacter.inventory.FPSEquipment.Info;
                }

            }

            if (AliveText && UnitZ.NetworkGameplay != null)
            {
                AliveText.text = UnitZ.NetworkGameplay.PlayersAlive.ToString() + " Alive";
            }
        }

        for (int i = 0; i < Pages.Length; i++)
        {
            if (!Pages[i].LockMouse && Pages[i].gameObject.activeSelf)
            {
                isMouselocked = false;
            }
        }
        //手机模式
        if (MobileController != null && UnitZ.IsMobile)
        {
            MobileController.gameObject.SetActive(isMouselocked);
        }
        //小地图
        if (MiniMap)
        {
            MiniMap.SetActive(isMouselocked);
        }

        MouseLock.MouseLocked = isMouselocked;

        if (Info != null)
        {
            Info.gameObject.SetActive(isShowinfo);
            if (Time.time >= timeInfo + 0.1f)
            {
                isShowinfo = false;
            }
        }
    }

    //打开第二个背包
    public void OpenSecondInventory(CharacterInventory inventory, string type)
    {
        if (IsPanelOpened("InventoryTrade"))
        {
            ClosePanelByName("InventoryTrade");
        }
        else
        {
            SecondItemLoader.OpenInventory(inventory, type);
            OpenPanelByName("InventoryTrade");
        }
    }

    //关闭第二个背包
    public void CloseSecondInventory()
    {
        ClosePanelByName("InventoryTrade");
    }

}
