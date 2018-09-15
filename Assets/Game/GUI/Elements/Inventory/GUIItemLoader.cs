using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

public class GUIItemLoader : MonoBehaviour
{
    public RectTransform Canvas;
    public RectTransform Item;
    public int Size = 5;
    public float Spacing = 3;

    private int updateTmp;
    private int numRaw;
    private int numItem;

    public string Type;
    [HideInInspector]
    public CharacterInventory currentInventory;

    void Start()
    {

    }

    void OnEnable()
    {
        UpdateGUIInventory();
    }

    void AddItemToRaw(ItemCollector item)
    {
        // Instantiate a GUIItemCollector to canvas in a raw.
        GameObject obj = (GameObject)GameObject.Instantiate(Item.gameObject, Vector3.zero, Quaternion.identity);
        GUIItemCollector guiitem = obj.GetComponent<GUIItemCollector>();

        if (guiitem)
        {
            guiitem.Item = item;
            guiitem.currentInventory = currentInventory;
            guiitem.Type = Type;

            if (item.Item.ImageSprite)
                guiitem.Icon.sprite = item.Item.ImageSprite;

            guiitem.Num.text = item.Num.ToString();
            guiitem.Name.text = item.Item.ItemName;

        }

        obj.transform.SetParent(Canvas.gameObject.transform);
        RectTransform rect = obj.GetComponent<RectTransform>();
        rect.anchoredPosition = new Vector2(((rect.sizeDelta.x + Spacing) * numItem) + Spacing, -(((rect.sizeDelta.y + Spacing) * numRaw) + Spacing));
        rect.localScale = Item.gameObject.transform.localScale;
        numItem++;

        if (numItem >= Size)
        {
            numItem = 0;
            numRaw += 1;
        }
        Canvas.sizeDelta = new Vector2(Canvas.sizeDelta.x, (Item.sizeDelta.y + Spacing) * (numRaw + 1));
    }

    public void UpdateGUIInventory()
    {
        if (currentInventory == null)
            return;

        // Instantiate a all item from inventory.

        Clear();
        List<ItemCollector> getitems = currentInventory.Items;
        for (int i = 0; i < getitems.Count; i++)
        {
            if (getitems[i].Num > 0 && getitems[i].Item != null && getitems[i].ItemIndex > -1)
            {
                if (!getitems[i].Item.HideOnUse || getitems[i].EquipIndex == -1)
                    AddItemToRaw(getitems[i]);
            }
        }
    }

    void Clear()
    {
        if (Canvas == null)
            return;

        numItem = 0;
        numRaw = 0;

        foreach (Transform child in Canvas.transform)
        {
            GameObject.Destroy(child.gameObject);
        }
    }

    public void UpdateFunction()
    {
        if (currentInventory == null)
            return;

        // GUI inventory will update only when any items on ChracterSystem.inventory has been changed
        // so if any items was removed or added a ChracterSystem.inventory.UpdateCount will count + 1
        // and updateTmp just using for collect the latest number of UpdateCount.

        if (currentInventory.UpdateCount != updateTmp)
        {
            updateTmp = currentInventory.UpdateCount;
            UpdateGUIInventory();
        }
    }

    void Update()
    {
        UpdateFunction();
    }
}
