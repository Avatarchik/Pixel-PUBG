using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;


public class TooltipInstance : MonoBehaviour
{
    protected PanelsManager panelsManager;
    protected ItemCollector Item;

    void Awake()
    {
        panelsManager = this.GetComponent<PanelsManager>();
    }

    void Start()
    {
        panelsManager = this.GetComponent<PanelsManager>();
        HideTooltip();
    }

    public bool hover = false;
    public virtual IEnumerator OnHover(PointerEventData eventData, ItemCollector itemCol)
    {
        // Heep showing until hover is "false"

        if (!hover)
        {
            Item = itemCol;
            yield return new WaitForSeconds(0.2f);
            hover = true;
        } 
        while (hover && !MouseLock.MouseLocked && !TooltipUsing.Instance.gameObject.activeSelf)
        {
            ShowTooltip(itemCol, new Vector3(eventData.position.x, eventData.position.y - 18f, 0f));
            yield return new WaitForEndOfFrame();
        }
        // skip this line until while loop is done.
        Item = null;
        hover = false;
        HideTooltip();
    }

    void Update()
    {
        if (MouseLock.MouseLocked)
            gameObject.SetActive(false);
    }

    public virtual void ShowTooltip(ItemCollector itemCol, Vector3 pos)
    {
        if (itemCol == null || MouseLock.MouseLocked)
            return;
        transform.position = pos;
        gameObject.SetActive(true);
    }

    public virtual void ShowTooltip(ItemCollector itemCol, Vector3 pos, string type)
    {
        if (itemCol == null)
            return;

        if (panelsManager)
        {
            panelsManager.DisableAllPanels();
            panelsManager.OpenPanelByName(type);
        }

        Item = itemCol;
        transform.position = pos;
        gameObject.SetActive(true);
    }

    public void HideTooltip()
    {
        gameObject.SetActive(false);
    }
}
