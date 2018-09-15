using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using System.Collections.Generic;

//周围物品GUI加载
public class GUIItemGroundLoader : MonoBehaviour
{
    public RectTransform Canvas;
    public RectTransform Item;
    public int Size = 5;
    public float Spacing = 3;

    private int updateTmp;
    private int numRaw;
    private int numItem;
    private ItemCollector[] getitems;

    void Start()
    {

    }

    private void OnEnable()
    {
        UpdateGUIInventory();
    }

    private void OnDisable()
    {
        getitems = new ItemCollector[1];
        tmp = "";
    }

    //未使用物品
    void AddItemToRaw(ItemCollector item)
    {
        GameObject obj = (GameObject)GameObject.Instantiate(Item.gameObject, Vector3.zero, Quaternion.identity);
        GUIItemCollector guiitem = obj.GetComponent<GUIItemCollector>();

        if (guiitem)
        {
            guiitem.Item = item;
            guiitem.currentInventory = null;
            guiitem.Type = "Ground";

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

    //重新显示所有
    public void UpdateGUIInventory()
    {
        if (getitems == null)
            return;

        Clear();
        for (int i = 0; i < getitems.Length; i++)
        {
            if (getitems[i] != null && getitems[i].Num > 0 && getitems[i].Item != null && getitems[i].ItemIndex > -1)
            {
                AddItemToRaw(getitems[i]);
            }
        }
    }

    //清除面板
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

    public string tmp;
    void Update()
    {
        if (UnitZ.playerManager == null || UnitZ.playerManager.PlayingCharacter == null || UnitZ.playerManager.PlayingCharacter.inventory == null)
            return;

        ItemCollector[] getrawitems = UnitZ.playerManager.PlayingCharacter.inventory.GetAllItemAround(2);
        //始终检查物品队列是否变化，若变化则更新面板
        string uniqueNum = "";
        for (int i = 0; i < getrawitems.Length; i++)
        {
            //合并物品
            uniqueNum += getrawitems[i].ItemIndex + ",";
        }
        //比较是否由新物品队列
        if (uniqueNum != tmp)
        {
            getitems = getrawitems;
            tmp = uniqueNum;
            UpdateGUIInventory();
        }
    }
}
