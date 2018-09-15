using UnityEngine;

//玩家输入控制
public class PlayerInputController : MonoBehaviour
{
    private FPSController fpsControl;

    void Update()
    {
        if (UnitZ.IsMobile)
            return;
        //在此改变控制器

        if (UnitZ.gameManager.IsPlaying)
        {
            //只在游戏模式
            //游戏主菜单打开
            if (Input.GetKeyDown(KeyCode.Escape))
            {
                UnitZ.Hud.TogglePanelByName("InGameMenu");
            }
            //打开第二面板
            if (Input.GetKeyDown(KeyCode.N))
            {
                UnitZ.Hud.TogglePanelByName("Scoreboard");
            }
        }

        if (UnitZ.playerManager != null && UnitZ.playerManager.PlayingCharacter != null)
        {
            //从当前玩家处取得控制器
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();

            if (UnitZ.playerManager.PlayingCharacter.isLocalPlayer && fpsControl != null)
            {
                //移动
                fpsControl.MoveCommand(new Vector3(Input.GetAxis("Horizontal"), 0, Input.GetAxis("Vertical")), Input.GetButton("Jump"));

                //改变状态-站立/蹲下/趴下
                if (Input.GetKeyDown(KeyCode.C))
                {
                    fpsControl.Sit();
                }
                //与车辆互相作用
                if (Input.GetKeyDown(KeyCode.F))
                {
                    fpsControl.OutVehicle();
                }
                //加速跑
                fpsControl.Sprint(Input.GetKey(KeyCode.LeftShift));
                //瞄准控制
                if (MouseLock.MouseLocked)
                {
                    fpsControl.Aim(new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")));
                    fpsControl.Trigger1(Input.GetButton("Fire1"));
                    fpsControl.Trigger2(Input.GetButtonDown("Fire2"));
                }
                //与物体作用
                if (Input.GetKeyDown(KeyCode.F))
                {
                    fpsControl.Interactive();
                }
                //改变视角
                if (Input.GetKeyDown(KeyCode.V))
                {
                    fpsControl.SwithView();
                }
                //改变视角
                if (Input.GetKeyDown(KeyCode.B))
                {
                    fpsControl.SwithSideView();
                }
                //换子弹
                if (Input.GetKeyDown(KeyCode.R))
                {
                    fpsControl.Reload();
                }
                //打开背包
                if (Input.GetKeyDown(KeyCode.Tab))
                {
                    UnitZ.Hud.TogglePanelByName("Inventory");
                }
                //打开地图
                if (Input.GetKeyDown(KeyCode.M))
                {
                    UnitZ.Hud.TogglePanelByName("Map");
                }

                //总是检查所有互动物品
                fpsControl.Checking();
            }
        }
    }

}
