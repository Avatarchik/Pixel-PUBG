using UnityEngine;

//玩家视角
public enum PlayerViewType
{
    FirstVeiw, ThirdView, FreeView, None
}
public class PlayerView : MonoBehaviour
{
    public FPSCamera FPSCamera;
    public OrbitCamera FreeCamera;
    public GameObject[] PlayerObjects;
    [System.NonSerialized]
    public OrbitCamera freeCamera;
    public float OrbitDistance = 10;
    public PlayerViewType View;
    private CharacterSystem character;


    void Start()
    {
        if (freeCamera == null && character.isLocalPlayer)
            freeCamera = GameObject.Instantiate(FreeCamera, this.transform.position, Quaternion.identity);

        ViewUpdate();
    }

    void Awake()
    {
        character = this.GetComponent<CharacterSystem>();
        FPSCamera = GetComponentInChildren<FPSCamera>();
    }

    public void SwithView()
    {
        switch (View)
        {
            case PlayerViewType.FirstVeiw:
                View = PlayerViewType.ThirdView;
                break;
            case PlayerViewType.ThirdView:
                View = PlayerViewType.FirstVeiw;
                break;
        }
    }
    public void SwithViewSide()
    {
        FPSCamera.ThirdViewInvert = FPSCamera.ThirdViewInvert * -1;
    }

    void LateUpdate()
    {
        if (freeCamera)
            freeCamera.gameObject.SetActive(View == PlayerViewType.FreeView);

        ViewUpdate();
        FPSCamera.IsThirdView = (View == PlayerViewType.ThirdView);
        if (character.Motor.MotorPreset.Length > character.MovementIndex)
            FPSCamera.transform.localPosition = character.Motor.MotorPreset[character.MovementIndex].FPSCamOffset;
    }

    public void ViewUpdate()
    {
        if (character && character.IsMine && character.isLocalPlayer)
        {
            //玩家视角
            if (View == PlayerViewType.FirstVeiw)
            {
                hidePlayerObjects(false);
                if (FPSCamera)
                    FPSCamera.gameObject.SetActive(true);
                if (freeCamera)
                    freeCamera.gameObject.SetActive(false);
            }

            if (View == PlayerViewType.FreeView)
            {
                hidePlayerObjects(true);
                if (FPSCamera)
                    FPSCamera.gameObject.SetActive(false);
                if (freeCamera)
                {
                    freeCamera.gameObject.SetActive(true);
                    freeCamera.distance = OrbitDistance;
                }
            }

            if (View == PlayerViewType.ThirdView)
            {
                if (FPSCamera.zooming && FPSCamera.hideWhenScoping)
                {
                    hidePlayerObjects(false);
                }
                else
                {
                    hidePlayerObjects(true);
                }
            }
        }
        if (character && !character.IsMine && !character.isLocalPlayer)
        {
            //其他人观看

            if (FPSCamera)
                FPSCamera.gameObject.SetActive(false);
            if (freeCamera)
                freeCamera.gameObject.SetActive(false);

            hidePlayerObjects(true);
        }
    }

    private void hidePlayerObjects(bool hide)
    {
        foreach (GameObject go in PlayerObjects)
        {
            if (go != null)
                go.SetActive(hide);
        }
    }

    public void Hide(Transform trans, bool hide)
    {
        foreach (Transform ob in trans)
        {
            ob.gameObject.SetActive(hide);
        }
        trans.gameObject.SetActive(hide);
    }

}
