using UnityEngine;
using UnityEngine.UI;
using System.Collections;

//GUI物品收集
public class GUIItemCollector : MonoBehaviour
{

    public ItemCollector Item;
    public Image Icon;
    public Text Num;
    public Text Name;
    public bool UpdateEnabled;
    [HideInInspector]
    public CharacterInventory currentInventory;
    public string Type = "";

    void Start()
    {
        //
    }

    public void SetItemCollector(ItemCollector item)
    {
        Item = item;
    }

    void FixedUpdate()
    {
        //更新GUI元素
        if (!UpdateEnabled)
            return;

        if (Item != null)
        {
            if (Icon != null)
            {
                Icon.enabled = true;
                if (Item.Item)
                    Icon.sprite = Item.Item.ImageSprite;
            }
            if (Num != null)
            {
                Num.enabled = true;
                Num.text = Item.Num.ToString();
            }
            if(Name != null)
            {
                if (Item.Item != null)
                {
                    Name.enabled = true;
                    Name.text = Item.Item.ItemName.ToString();
                   /* string use = "";
                    if (Item.InUse)
                        use = "X";
                    Name.text = Item.EquipIndex.ToString()+" "+ use;*/
                }
            }
        }
        if (Item == null || (Item != null && Item.Num <= 0))
        {
            if (Icon != null)
                Icon.enabled = false;
            if (Num != null)
                Num.enabled = false;

            if (Name != null)
                Name.enabled = false;
        }
    }
}
