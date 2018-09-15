using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GUIItemEquippedLoader : MonoBehaviour
{

    public GUIItemEquipped[] ItemEquippeds;

    void Start()
    {
        if(ItemEquippeds.Length <= 0)
        {
            ItemEquippeds = this.GetComponentsInChildren<GUIItemEquipped>();
        }
        UpdateEquippedShortcut();
    }

    public void RestEquipShortcut()
    {
        for (int i = 0; i < ItemEquippeds.Length; i++)
        {
            if (ItemEquippeds[i] != null)
            {
                ItemEquippeds[i].Clear();
            }
        }
    }

    void OnEnable()
    {
        setup = false;
        UpdateEquippedShortcut();
    }

    public void UpdateEquip()
    {
        for (int i = 0; i < ItemEquippeds.Length; i++)
        {
            if (ItemEquippeds[i] != null)
            {
                ItemEquippeds[i].SetupCollector();
            }
        }
    }
    bool setup = false;
    public void UpdateEquippedShortcut()
    {
        if (UnitZ.playerManager != null && UnitZ.playerManager.PlayingCharacter == null)
        {
            setup = false;
            return;
        }
        if (!setup)
        {
            for (int i = 0; i < ItemEquippeds.Length; i++)
            {
                if (ItemEquippeds[i] != null)
                {
                    ItemEquippeds[i].CurrentItemCollector = null;
                    ItemEquippeds[i].CurrentSticker = UnitZ.playerManager.PlayingCharacter.inventory.GetItemSticker(ItemEquippeds[i].Type, ItemEquippeds[i].Tag);
                }
            }
            setup = true;
        }
        UpdateEquip();
    }
}
