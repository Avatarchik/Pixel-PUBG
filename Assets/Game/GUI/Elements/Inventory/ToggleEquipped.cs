using System.Collections;
using System.Collections.Generic;
using UnityEngine;

//已装备物品
public class ToggleEquipped : MonoBehaviour
{

    public GUIItemEquipped Equipped;
    public KeyCode Key;

    void Start()
    {

    }

    void Update()
    {
        if (Input.GetKeyDown(Key))
        {
            if (UnitZ.playerManager != null && UnitZ.playerManager.PlayingCharacter != null && UnitZ.playerManager.PlayingCharacter.inventory != null)
                UnitZ.playerManager.PlayingCharacter.inventory.ToggleUseEquipped(Equipped.CurrentItemCollector);
        }
    }
}
