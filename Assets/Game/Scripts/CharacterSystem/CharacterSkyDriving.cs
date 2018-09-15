using UnityEngine.Networking;
using UnityEngine;

//角色飞机上时
public class CharacterSkyDriving : NetworkBehaviour
{
    //降落伞图形
    public GameObject Parachute;
    private CharacterSystem character;
    private PlayerView view;
    public float ViewDistance = 10;
    private bool isJumpped = false;
    private bool isGrounded = false;
    public bool IsGrounded;
    [SyncVar]
    public bool Released;
    private float timeTmp;
    public float TimeReleaseParachute = 5;
    private float timeReleaseTmp = 0;

    private void Awake()
    {
        view = this.GetComponent<PlayerView>();
        character = this.GetComponent<CharacterSystem>();
    }

    void Start()
    {
        isJumpped = false;
        isGrounded = false;
    }

    public void OnOutOfVehicle(bool needparachute)
    {
        //命令跳下飞机
        if (needparachute)
            CmdJumpOutThePlane();
    }

    [Command(channel = 0)]
    private void CmdJumpOutThePlane()
    {
        character.Sit(2);
        RpcJumpOutThePlane();
        timeTmp = Time.time;
        isJumpped = true;
        isGrounded = false;
        timeReleaseTmp = Time.time;
    }

    [ClientRpc(channel = 0)]
    void RpcJumpOutThePlane()
    {
        isJumpped = true;
        isGrounded = false;
        timeReleaseTmp = Time.time;
        if (view != null)
        {
            view.View = PlayerViewType.FreeView;
            view.OrbitDistance = ViewDistance;
        }
    }

    public void ReleaseParachute()
    {
        if (!Released && isJumpped && !isGrounded)
        {
            //如果不是地面或从未发布，命令释放降落伞
            CmdReleaseParachute();
            character.Sit(0);
            Released = true;
        }
    }

    [Command(channel = 0)]
    private void CmdReleaseParachute()
    {
        //改变角色视角
        Released = true;
        character.Sit(0);
    }

    [Command(channel = 0)]
    private void CmdLanded()
    {
        isGrounded = true;
        character.Sit(0);
        RpcLanded();
    }

    [ClientRpc(channel = 0)]
    void RpcLanded()
    {
        isGrounded = true;
        Released = false;

        //着陆后改为第一人称视角
        if (view.View == PlayerViewType.FreeView)
        {
            view.View = PlayerViewType.FirstVeiw;
        }
    }

    void Update()
    {
        if (character == null || character.Motor == null)
            return;

        //检查是否为地面
        IsGrounded = character.Motor.grounded;
        //隐藏降落伞
        if (Parachute)
            Parachute.SetActive(Released);

        if (isServer)
        {
            if (isJumpped && !isGrounded)
            {
                //在开始着陆检查前等待2秒，以防止撞击飞机而不是地面
                if (Time.time > timeTmp + 2)
                {
                    if (IsGrounded)
                    {
                        // if landed
                        CmdLanded();
                    }
                }
                if (!Released)
                {
                    //降落伞可以及时自动释放
                    if (Time.time >= timeReleaseTmp + TimeReleaseParachute)
                    {
                        ReleaseParachute();
                    }
                }
            }
        }
        else
        {
            if (isJumpped && !isGrounded)
            {
                view.OrbitDistance = ViewDistance;
            }
        }
    }
}
