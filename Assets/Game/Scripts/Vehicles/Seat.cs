using UnityEngine;
using System.Collections;

#if UNITY_EDITOR
using UnityEditor;
#endif

//座位
public class Seat : MonoBehaviour
{
    public Vehicle VehicleRoot;
    public string SeatID = "S1";
    public string CharacterID = "";
    public int PassengerID = -1;
    public bool IsDriver = false;
    public bool Active = false;
    [System.NonSerialized]
    public CharacterDriver passenger;
    public PlayerViewType ForceView;
    public bool FixRotation = false;
    public float ViewDistance = 30;

    void Start()
    {
        if (VehicleRoot == null)
        {
            if (this.transform.root)
                VehicleRoot = this.transform.root.GetComponent<Vehicle>();
        }
    }

    public void CleanSeat()
    {
        CharacterDriver[] pasengers = (CharacterDriver[])this.transform.GetComponentsInChildren<CharacterDriver>();
        for (int i = 0; i < pasengers.Length; i++)
        {
            if (pasengers[i])
            {
                if (pasengers[i].transform.parent != null)
                    pasengers[i].NoVehicle();

                pasengers[i].transform.parent = null;
                pasengers[i].DrivingSeat = null;
                pasengers[i] = null;
            }
        }
        if (passenger)
        {
            passenger.NoVehicle();
            passenger = null;
        }
    }
    public void CheckSeat()
    {
        if (this.transform.childCount <= 0)
        {
            Active = false;
            passenger = null;
        }
    }


    void OnDrawGizmos()
    {
#if UNITY_EDITOR
        Gizmos.color = Color.green;
        Gizmos.DrawSphere(transform.position, 0.2f);
        Gizmos.DrawWireCube(transform.position, transform.localScale);
        Handles.Label(transform.position, "Seat");
#endif
    }
}
