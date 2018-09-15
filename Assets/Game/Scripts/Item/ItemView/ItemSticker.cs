using UnityEngine;
using System.Collections;
#if UNITY_EDITOR
using UnityEditor;
#endif

public enum EquipType
{
    None, PrimaryUse, Head, Armor, BackPack, Foot, FPSItemView, PrimaryGun, SecondaryGun, MeleeWeapon, Grenade, Use
}

public class ItemSticker : MonoBehaviour
{
    public int Index;
    public EquipType equipType;
    public ItemCollector itemCollector;
    public string Tag;
    public Vector3 RotationOffset;
    public int ItemIndex;

    void Start()
    {

    }
    [HideInInspector]
    public bool IsVisible = true;
    public void Visible(bool visible)
    {
        for (int i = 0; i < this.transform.childCount; i++)
        {
            Renderer[] ren = this.transform.GetChild(i).gameObject.GetComponentsInChildren<Renderer>();
            for (int r = 0; r < ren.Length; r++)
            {
                ren[r].enabled = visible;
            }
        }
        IsVisible = visible;
    }

    public ItemEquipment GetEquipped()
    {
        if (this.transform.childCount > 0)
            return this.transform.GetChild(0).GetComponent<ItemEquipment>();

        return null;
    }

    public bool IsEmpty()
    {
        if (this.transform.childCount <= 0)
            return true;

        return false;
    }

    void OnDrawGizmos()
    {
#if UNITY_EDITOR

        Gizmos.color = Color.yellow;
        Gizmos.DrawSphere(this.transform.position, 0.1f);
        Gizmos.DrawWireCube(this.transform.position, Vector3.one * 0.3f);
        Handles.Label(transform.position, this.name);
        Gizmos.color = Color.red;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.right * 0.2f);
        Gizmos.color = Color.green;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.up * 0.2f);
        Gizmos.color = Color.blue;
        Gizmos.DrawLine(this.transform.position, this.transform.position + this.transform.forward * 0.2f);
#endif
    }
}
