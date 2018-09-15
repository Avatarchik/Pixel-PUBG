using UnityEngine;
using System.Collections;

public class ItemEquipment : MonoBehaviour
{

    public EquipType itemType;
    public bool AutoToggle = false;
    public bool AutoEquip = false;
    [HideInInspector]
    public int UsingType;
    [Range(0, 1)]
    public float Armor = 0;
    public int InventoryPlus = 0;

    void Start()
    {

    }

    void Update()
    {

    }

    public virtual void Action(Vector3 direction, byte num,byte spread, byte seed)
    {

    }
}
