using UnityEngine;

//AI物品开始
public class AIItemStarter : MonoBehaviour
{
    [HideInInspector]
    public CharacterSystem character;
    [HideInInspector]
    public AICharacterShooterNav ai;
    public ItemSet[] ItemSets;
    public float TimeWait = 60;
    private float timeTmp = 0;
    [HideInInspector]
    public int CurrentSet = 0;

    void Start()
    {
        character = this.GetComponent<CharacterSystem>();
        ai = character.GetComponent<AICharacterShooterNav>();
        timeTmp = Time.time;
        TimeWait = Random.Range(TimeWait / 2, TimeWait);
    }

    void Update()
    {
        //AI会慢慢获得更多物品
        if (character)
        {
            if (character.isServer)
            {
                if (ItemSets.Length > CurrentSet)
                {
                    if (Time.time >= timeTmp + TimeWait)
                    {
                        if (getNewSet())
                        {
                            CurrentSet += 1;
                            if (ai)
                                ai.Fighting = true;
                        }
                        timeTmp = Time.time;
                    }
                }
            }
        }
    }

    bool getNewSet()
    {
        if (character != null && character.inventory != null)
        {
            for (int i = 0; i < ItemSets.Length; i++)
            {
                if (i == CurrentSet)
                {
                    TimeWait = ItemSets[i].NextTime + Random.Range(0,10);

                    if (ItemSets[i].RandomEquipOne)
                    {
                        //随机装备一些物品
                        character.inventory.AddItemByItemData(ItemSets[i].Items[Random.Range(0, ItemSets[i].Items.Length)], 1, -1, -1);
                        character.inventory.UpdateInventoryToAll();
                    }
                    else
                    {
                        //装备列表所有物品
                        for (int v = 0; v < ItemSets[i].Items.Length; v++)
                        {
                            character.inventory.AddItemByItemData(ItemSets[i].Items[v], 1, -1, -1);
                        }
                        character.inventory.UpdateInventoryToAll();
                    }
                    return true;
                }
            }
        }
        return false;
    }
}

[System.Serializable]
public class ItemSet
{
    public ItemData[] Items;
    public bool RandomEquipOne = true;
    public float NextTime = 60;
}