using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class TooltipDetails : TooltipInstance
{

    // gui elements, need to assign them to these parameter.
    public Text Header;
    public Text Content;
    private static TooltipDetails tooltip;

    void Start()
    {
        tooltip = this;
        HideTooltip();
    }

    public static TooltipDetails Instance
    {
        get
        {
            if (tooltip == null)
            {
                TooltipsManager toolmanage = (TooltipsManager)GameObject.FindObjectOfType(typeof(TooltipsManager));
                if (toolmanage)
                {
                    for (int i = 0; i < toolmanage.AllToolTips.Length; i++)
                    {
                        if (toolmanage.AllToolTips[i].GetComponent<TooltipDetails>())
                        {
                            tooltip = toolmanage.AllToolTips[i].GetComponent<TooltipDetails>();
                            break;
                        }
                    }
                }
            }
            return tooltip;
        }
    }

    public override void ShowTooltip(ItemCollector itemCol, Vector3 pos)
    {
        if (itemCol == null || itemCol.Item == null || MouseLock.MouseLocked)
            return;

        // update GUI elements with name and description of ItemCollector

        if (Header)
            Header.text = itemCol.Item.ItemName;
        if (Content)
            Content.text = itemCol.Item.Description;

        if (TooltipUsing.Instance.gameObject.activeSelf)
            hover = false;

        base.ShowTooltip(itemCol, pos);
    }
}
