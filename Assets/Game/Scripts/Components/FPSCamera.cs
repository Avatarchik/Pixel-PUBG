using UnityEngine;
using System.Collections;

public class FPSCamera : MonoBehaviour
{
    public Vector3 ThirdViewOffset = new Vector3(1, 0, -2);
    public Vector3 FreeViewOffset = new Vector3(0, 0, -10);
    public float ThirdViewZoomMult = 1;
    public int ThirdViewInvert = 1;
    public Camera MainCamera;
    public ItemSticker FPSItemView;
    public Transform RayPointer;

    [HideInInspector]
    public float HorizonSway = 0.4f;
    public float VerticalSway = 0.4f;
    public Vector3 aimOffset;
    public bool IsThirdView;
    public bool IsFreeView;
    public bool zooming;
    public bool hideWhenScoping;

    private Vector3 swayOffset;
    private Vector3 positionTmpOffset;
    private Vector3 rootDirTmp;
    private Vector3 fpsDirTmp;
    private float rootDirHDot;
    private float rootDirVDot;
    private Vector3 rootDifPos;
    private bool isThirdViewTmp;



    void Start()
    {
        positionTmpOffset = FPSItemView.transform.localPosition;
    }

    public void Aim(Vector3 offset)
    {
        aimOffset = offset;
    }

    public void Zoom()
    {

    }

    void Update()
    {
        if (FPSItemView == null)
            return;

        if ((IsThirdView && !zooming) || (IsThirdView && (zooming && !hideWhenScoping)))
        {
            RaycastHit hitRay;
            Vector3 dir = this.transform.forward;
            if (Physics.Raycast(MainCamera.transform.position, MainCamera.transform.forward, out hitRay))
            {
                dir = (hitRay.point - RayPointer.position).normalized;

            }
            if (Vector3.Dot(dir, this.transform.forward) > 0.7f)
            {
                RayPointer.forward = dir;
            }

            Vector3 thirdPos = new Vector3(ThirdViewOffset.x * ThirdViewInvert, ThirdViewOffset.y, ThirdViewOffset.z * ThirdViewZoomMult);
            Vector3 freePos = new Vector3(FreeViewOffset.x * ThirdViewInvert, FreeViewOffset.y, FreeViewOffset.z);

            if (IsThirdView)
            {
                MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, thirdPos, Time.deltaTime * 30);
                FPSItemView.transform.localScale = Vector3.zero;
            }
            if (IsFreeView)
            {
                MainCamera.transform.localPosition = Vector3.Lerp(MainCamera.transform.localPosition, freePos, Time.deltaTime * 30);
                FPSItemView.transform.localScale = Vector3.zero;
            }
            
        }
        else
        {
            MainCamera.transform.localPosition = Vector3.zero;
            FPSItemView.transform.localScale = Vector3.one;
            RayPointer.forward = this.transform.forward;
        }

        if (isThirdViewTmp != IsThirdView)
        {
            this.gameObject.transform.root.gameObject.SendMessageUpwards("OnViewChanged", SendMessageOptions.DontRequireReceiver);
            isThirdViewTmp = IsThirdView;
        }


        float DirH = Vector3.Dot(this.gameObject.transform.root.transform.right.normalized, rootDirTmp.normalized) * HorizonSway;
        float DirV = Vector3.Dot(this.transform.up.normalized, fpsDirTmp.normalized) * VerticalSway;

        if (aimOffset != Vector3.zero)
        {
            DirH = 0;
            DirV = 0;
        }
        rootDirHDot = Mathf.Lerp(rootDirHDot, DirH, 10 * Time.deltaTime);
        rootDirVDot = Mathf.Lerp(rootDirVDot, DirV, 10 * Time.deltaTime);
        swayOffset.x = rootDirHDot;
        swayOffset.y = rootDirVDot;

        Vector3 offsetTarget = positionTmpOffset + (-swayOffset) + aimOffset;
        FPSItemView.transform.localPosition = Vector3.Lerp(FPSItemView.transform.localPosition, offsetTarget, 5 * Time.deltaTime);
        rootDirTmp = this.gameObject.transform.root.transform.forward;
        fpsDirTmp = this.transform.forward;
    }

}
