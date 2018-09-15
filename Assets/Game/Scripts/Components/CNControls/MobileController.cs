using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using CnControls;

[RequireComponent(typeof(FPSController))]
public class MobileController : MonoBehaviour
{
    public float AimSensitivity = 0.1f;
    public GameObject[] controls;
    void Start()
    {

        if (controls.Length <= 0)
        {
            controls = new GameObject[this.transform.childCount];
            for (int i = 0; i < this.transform.childCount; i++)
            {
                controls[i] = this.transform.GetChild(i).gameObject;
            }
        }
    }
    void SetVisible(bool visible)
    {
        for (int i = 0; i < controls.Length; i++)
        {
            controls[i].SetActive(visible);
        }
    }

    public void SwithView()
    {
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
                fpsControl.SwithView();
        }
    }
    public void SwithSideView()
    {
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
                fpsControl.SwithSideView();
        }
    }

    public void OpenMap()
    {
        UnitZ.Hud.TogglePanelByName("Map");
    }

    public void OpenInventory()
    {
        UnitZ.Hud.TogglePanelByName("Inventory");
    }
    
    public void Zoom(){
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
            {
                fpsControl.Trigger2(true);
            }
        }
    }
    
    public void Interactive(){
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
            {
                fpsControl.OutVehicle();
                fpsControl.Interactive();
            }
        }
    }
    
    public void Reload(){
        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
            {
                fpsControl.Reload();
            }
        }
    }

    void Update()
    {

        if (UnitZ.playerManager.PlayingCharacter != null)
        {
            SetVisible(true);
            FPSController fpsControl = UnitZ.playerManager.PlayingCharacter.GetComponent<FPSController>();
            if (fpsControl)
            {
                MouseLock.IsMobileControl = true;
                fpsControl.MoveCommand(new Vector3(CnInputManager.GetAxis("Horizontal"), 0, CnInputManager.GetAxis("Vertical")), CnInputManager.GetButton("Jump"));
                fpsControl.Aim(new Vector2(CnInputManager.GetAxis("Touch X") * AimSensitivity, CnInputManager.GetAxis("Touch Y") * AimSensitivity));
                fpsControl.Trigger1(CnInputManager.GetButton("Touch Fire1"));
                /*fpsControl.Trigger2(CnInputManager.GetButtonDown("Fire2"));

                if (CnInputManager.GetButtonDown("Fire3"))
                {
                    fpsControl.OutVehicle();
                    fpsControl.Interactive();
                }

                if (CnInputManager.GetButtonDown("Submit"))
                {
                    fpsControl.Reload();
                }*/

                fpsControl.Checking();
            }

        }
        else
        {
            SetVisible(false);
        }
    }
}
