using UnityEngine;
using UnityEngine.Networking;
using System.Collections;
using System.Collections.Generic;

//车辆
[RequireComponent(typeof(CarControl))]
public class Vehicle : DamageManager
{
    public bool NeedParachute = false;
    public Seat[] Seats;
    public string VehicleName;
    [SyncVar]
    public string VehicleID;
    [HideInInspector]
    public bool incontrol;
    [SyncVar(hook = "OnSeatDataChanged")]
    public string SeatsData;
    [SyncVar]
    private Vector3 positionSync;
    [SyncVar]
    private Quaternion rotationSync;
    public bool hasDriver;

    void Awake()
    {
        if (Seats.Length <= 0)
        {
            Seats = (Seat[])this.GetComponentsInChildren(typeof(Seat));
        }
    }

    public override void OnDestroyed()
    {
        for (int i = 0; i < Seats.Length; i++)
        {
            Seats[i].CleanSeat();
        }
        base.OnDestroyed();
    }

    public override void OnStartClient()
    {
        if (isServer)
        {
            VehicleID = netId.ToString();
        }
        OnSeatDataChanged(SeatsData);
        base.OnStartClient();
    }

    void OnSeatDataChanged(string seatsdata)
    {
        SeatsData = seatsdata;
        string[] passengerData = seatsdata.Split(","[0]);
        if (passengerData.Length >= Seats.Length)
        {
            for (int i = 0; i < Seats.Length; i++)
            {
                int.TryParse(passengerData[i], out Seats[i].PassengerID);
            }
        }
    }

    void GenSeatsData()
    {
        string seatdata = "";
        for (int i = 0; i < Seats.Length; i++)
        {
            if (Seats[i].PassengerID != -1)
            {
                seatdata += Seats[i].PassengerID + ",";
            }
            else
            {
                seatdata += "-1,";
            }
        }
        SeatsData = seatdata;
    }

    public void EjectAllSeat()
    {
        for (int i = 0; i < Seats.Length; i++)
        {
            if (Seats[i].passenger != null)
            {
                Seats[i].passenger.OutVehicle();
            }
        }
    }

    void UpdatePassengerOnSeats()
    {
        hasDriver = false;
        for (int i = 0; i < Seats.Length; i++)
        {
            if (Seats[i].PassengerID != -1)
            {
                //搜索具有乘客ID的玩家将其捕捉到车辆中。
                NetworkInstanceId passengerid = new NetworkInstanceId((uint)Seats[i].PassengerID);
                GameObject obj = ClientScene.FindLocalObject(passengerid);
                if (obj)
                {
                    CharacterDriver driver = obj.GetComponent<CharacterDriver>();

                    if (driver)
                    {
                        driver.NeedParachute = NeedParachute;

                        FPSController fps = driver.GetComponent<FPSController>();
                        if (fps)
                        {
                            fps.FixedRotation();
                        }

                        if (Seats[i].ForceView != PlayerViewType.None)
                        {
                            PlayerView view = driver.GetComponent<PlayerView>();
                            if (view)
                            {
                                view.View = Seats[i].ForceView;
                                view.OrbitDistance = Seats[i].ViewDistance;
                            }
                        }
                        if (Seats[i].FixRotation)
                        {
                            driver.transform.rotation = Seats[i].transform.rotation;
                        }
                        driver.character.Motor.grounded = false;
                        driver.transform.position = Seats[i].transform.position;
                        driver.transform.parent = Seats[i].transform;
                        driver.CurrentVehicle = this;
                        driver.character.controller.enabled = false;
                        driver.DrivingSeat = Seats[i];
                        hasDriver = true;
                        if (driver.character.IsAlive == false)
                        {
                            Seats[i].PassengerID = -1;
                        }
                    }
                }
            }
            else
            {
                Seats[i].CleanSeat();
            }
        }

        if (isServer)
        {
            GenSeatsData();
        }
    }



    public void GetOutTheVehicle(CharacterDriver driver)
    {
        Debug.Log("Get out this car " + driver.netId.ToString());
        for (int i = 0; i < Seats.Length; i++)
        {
            if (Seats[i].PassengerID == driver.netId.Value)
            {
                Seats[i].PassengerID = -1;
                return;
            }
        }
    }

    public virtual void Pickup(CharacterSystem character)
    {
        character.SendMessage("PickupCarCallback", this);
    }

    public void GetInTheVehicle(CharacterDriver driver, int seatID)
    {
        if (driver && seatID != -1 && seatID >= 0 && seatID < Seats.Length)
        {
            driver.CurrentVehicle = this;
            Seats[seatID].PassengerID = (int)driver.netId.Value;
            Seats[seatID].passenger = driver;
        }
    }

    public int FindOpenSeatID()
    {
        for (int i = 0; i < Seats.Length; i++)
        {
            if (Seats[i].PassengerID == -1)
            {
                return i;
            }
        }
        return -1;
    }


    public virtual void Drive(Vector2 input, bool brake)
    {

    }


    public void UpdateFunction()
    {
        DamageUpdate();
        UpdatePassengerOnSeats();

        if (isServer)
        {
            positionSync = this.transform.position;
            rotationSync = this.transform.rotation;
        }

        this.transform.position = Vector3.Lerp(this.transform.position, positionSync, 0.5f);
        this.transform.rotation = Quaternion.Lerp(this.transform.rotation, rotationSync, 0.5f);
    }

    void Update()
    {
        UpdateFunction();
        UpdateDriver();
    }


    public void UpdateDriver()
    {
        for (int i = 0; i < Seats.Length; i++)
        {
            if (Seats[i].IsDriver && Seats[i].passenger != null)
            {
                return;
            }
        }
        incontrol = false;
    }

    public void GetInfo()
    {
        string info = "Get in\n" + VehicleName;
        UnitZ.Hud.ShowInfo(info, this.transform.position);
    }

}
