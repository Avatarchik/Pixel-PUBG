using UnityEngine;
using UnityEngine.Networking;
using System.Collections;

[RequireComponent(typeof(NetworkIdentity))]

public class ItemData : NetworkBehaviour
{
    public Sprite ImageSprite;
    public string ItemName;
    public string Description;
    public int Price;
    public bool Stack = true;
    public bool UniqueItem = false;
    public bool HideOnUse = true;
    
    public FPSItemEquipment ItemFPS;
    public ItemEquipment ItemEquip;
    [SyncVar]
    public int Quantity = 1;
    [HideInInspector]
    [SyncVar]
    public int NumTag = -1;
    public AudioClip SoundPickup;
    public string ItemID;
    [HideInInspector]
    public int ItemIndex;
    public bool SavePreviousUse = true;

    public virtual void Pickup(CharacterSystem character)
    {
        character.SendMessage("PickupItemCallback", this);
        RemoveItem();
    }

    public void SetupDrop(int numtag, int num)
    {
        //Debug.Log("Setup drop");
        NumTag = numtag;
        Quantity = num;
    }



    public void RemoveItem()
    {
        Destroy(this.gameObject);
    }

    void Start()
    {

    }

    public void GetInfo()
    {
        string info = "按住 F 捡起\n" + ItemName + " x " + Quantity;
        UnitZ.Hud.ShowInfo(info,this.transform.position);
    }


}
