using UnityEngine;
using System.Collections;
using UnityEngine.Networking;

[RequireComponent(typeof(NetworkIdentity))]
[RequireComponent(typeof(CharacterInventory))]
public class ItemStocker : NetworkBehaviour
{

    public string StockID = "mybox";
    public CharacterInventory inventory;

    private int updateTemp = 0;
    private bool stockLoaded = false;
    private ObjectPlacing placing;

    public float DistanceLimit = 2;
    public Vector3 Offset;
    public string ActiveText = "Interactive";

    private CharacterSystem characterTemp;
    [SyncVar(hook = "OnStockChanged")]
    public string DataText = "";
    public bool AddStarterItem;
    public bool DestroyIfEmpty = false;
    public float DestroyDelay = 1;

    void Start()
    {
        inventory = this.GetComponent<CharacterInventory>();
        placing = this.GetComponent<ObjectPlacing>();

        if (placing)
        {
            StockID = placing.ItemUID;
        }
        else
        {
            stockLoaded = true;
        }

        if (isServer)
            LoadStock();

        if (AddStarterItem)
        {
            if (inventory)
                inventory.SetupStarterItem();
        }
    }


    void OnStockChanged(string datatext)
    {
        DataText = datatext;
        GetUpdateStock();
    }

    public void OpenStock()
    {
        GetUpdateStock();
    }


    void Update()
    {
        if (inventory == null)
            return;

        if (isServer && updateTemp != inventory.UpdateCount && stockLoaded)
        {
            UpdateStock();
            SaveStock();
            updateTemp = inventory.UpdateCount;
        }

        if (characterTemp)
        {
            if (Vector3.Distance(this.transform.position, characterTemp.transform.position + Offset) > DistanceLimit)
            {
                OnExit();
            }
            else
            {
                OnStay();
            }
        }

        if (DestroyIfEmpty)
        {
            if (inventory.Items.Count <= 0)
            {
                Destroy(this.gameObject, DestroyDelay);
            }
        }
    }

    public void Pickup(CharacterSystem character)
    {
        character.SendMessage("PickupStockCallback", this);
    }

    public void PickUpStock(CharacterSystem character)
    {
        if (character && character.IsMine)
        {
            character.inventory.PeerTrade = inventory;
            OpenStock();
            UnitZ.Hud.OpenSecondInventory(inventory, "Stock");
        }
        characterTemp = character;
    }


    void SaveStock()
    {
        if (inventory == null || placing == null)
            return;

        DataText = inventory.GetItemDataText();
        PlayerPrefs.SetString(StockID, DataText);
    }

    void LoadStock()
    {
        if (inventory == null || placing == null)
            return;

        if (PlayerPrefs.HasKey(StockID))
        {
            inventory.SetItemsFromText(PlayerPrefs.GetString(StockID));
            stockLoaded = true;
        }
        else
        {
            stockLoaded = true;
            SaveStock();
        }
    }

    void UpdateStock()
    {
        if (isServer)
        {
            DataText = inventory.GetItemDataText();
        }
    }

    public void GetUpdateStock()
    {
        inventory.SetItemsFromText(DataText);
    }


    public void OnStay()
    {

    }

    public void OnExit()
    {
        UnitZ.Hud.CloseSecondInventory();
        characterTemp.inventory.PeerTrade = null;
        characterTemp = null;
    }


    public void GetInfo()
    {
        UnitZ.Hud.ShowInfo(ActiveText,this.transform.position);
    }

}
