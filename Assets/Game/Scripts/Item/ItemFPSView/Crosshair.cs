using UnityEngine;
using System.Collections;

//准星
public class Crosshair : MonoBehaviour
{

    private FPSController fpsController;
    public Sprite CrosshairImg;
    public Sprite CrosshairZoomImg;
    public Sprite CrosshairHit;

    public float HitDuration = 0.2f;
    private float timeTemp = 0;

    void Start()
    {
        if (this.transform.root)
        {
            fpsController = this.transform.root.GetComponent<FPSController>();
        }
        else
        {
            fpsController = this.transform.GetComponent<FPSController>();
        }
    }

    public void Hit()
    {
        timeTemp = Time.time;
    }

    void Update()
    {

    }

    void OnGUI()
    {
        if (fpsController)
        {
            if (fpsController.character && fpsController.character.IsMine)
            {

                if (CrosshairImg)
                {
                    UnitZ.Hud.Crosshair.sprite = CrosshairImg;
                    UnitZ.Hud.Crosshair.gameObject.SetActive(true);
                }
                else
                {
                    UnitZ.Hud.Crosshair.gameObject.SetActive(false);
                }


                if (CrosshairZoomImg)
                {
                    UnitZ.Hud.CrosshairScope.sprite = CrosshairZoomImg;
                    UnitZ.Hud.CrosshairScope.gameObject.SetActive(fpsController.zooming);
                    if (CrosshairImg)
                        UnitZ.Hud.Crosshair.gameObject.SetActive(!fpsController.zooming);
                }
                else
                {
                    UnitZ.Hud.CrosshairScope.gameObject.SetActive(false);
                }

                if (Time.time < timeTemp + HitDuration)
                {
                    UnitZ.Hud.CrosshairHit.gameObject.SetActive(true);
                    UnitZ.Hud.CrosshairHit.sprite = CrosshairHit;
                }
                else
                {
                    UnitZ.Hud.CrosshairHit.gameObject.SetActive(false);
                }
            }
        }
    }
}
