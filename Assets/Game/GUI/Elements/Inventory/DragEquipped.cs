using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


//拖动已装备物品
[RequireComponent(typeof(Image))]
public class DragEquipped : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
    public bool dragOnSurfaces = true;
    public GUIItemCollector GUIItem;

    private GameObject m_DraggingIcon;
    private RectTransform m_DraggingPlane;

    void Start()
    {
        //实例化物品GUI控制器
        if (GUIItem == null)
            GUIItem = this.GetComponent<GUIItemCollector>();
    }


    //创建图标
    public void OnBeginDrag(PointerEventData eventData)
    {
        var canvas = FindInParents<Canvas>(gameObject);
        if (canvas == null)
            return;

        //已经点击可拖动物品
        //为该物品创造一个图标
        m_DraggingIcon = UnitZ.Hud.ImageDrag.gameObject;
        m_DraggingIcon.SetActive(true);

        m_DraggingIcon.GetComponent<DragIconHanddle>().Owner = this.transform.root.gameObject;
        m_DraggingIcon.GetComponent<DragIconHanddle>().Type = 0;
        m_DraggingIcon.transform.SetAsLastSibling();

        var image = m_DraggingIcon.GetComponent<Image>();
        image.color = Color.white;
        if (GUIItem != null && GUIItem.Icon != null)
        {
            if (GUIItem.Item != null && GUIItem.Item.Item != null)
            {
                UnitZ.Hud.ImageDrag.gameObject.SetActive(true);
                image.sprite = GUIItem.Item.Item.ImageSprite;
                image.GetComponent<RectTransform>().sizeDelta = GUIItem.Icon.GetComponent<RectTransform>().sizeDelta;
            }
            else
            {
                UnitZ.Hud.ImageDrag.gameObject.SetActive(false);
            }
        }
        if (dragOnSurfaces)
            m_DraggingPlane = transform as RectTransform;
        else
            m_DraggingPlane = canvas.transform as RectTransform;

        SetDraggedPosition(eventData);
    }

    //拖动中
    public void OnDrag(PointerEventData data)
    {
        if (m_DraggingIcon != null)
            SetDraggedPosition(data);
    }

    //设置拖动位置
    private void SetDraggedPosition(PointerEventData data)
    {
        //取得目标位置
        this.gameObject.SendMessage("Draging", SendMessageOptions.DontRequireReceiver);
        if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
            m_DraggingPlane = data.pointerEnter.transform as RectTransform;

        var rt = m_DraggingIcon.GetComponent<RectTransform>();
        Vector3 globalMousePos;
        if (RectTransformUtility.ScreenPointToWorldPointInRectangle(m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos))
        {
            rt.position = globalMousePos;
            rt.rotation = m_DraggingPlane.rotation;
        }
    }

    public void OnEndDrag(PointerEventData eventData)
    {
        //停止拖动后，移动图标
        if (m_DraggingIcon != null)
            m_DraggingIcon.SetActive(false);
    }

    static public T FindInParents<T>(GameObject go) where T : Component
    {
        //光标选中后，获取任何组件
        if (go == null)
            return null;
        var comp = go.GetComponent<T>();

        if (comp != null)
            return comp;

        Transform t = go.transform.parent;
        while (t != null && comp == null)
        {
            comp = t.gameObject.GetComponent<T>();
            t = t.parent;
        }
        return comp;
    }
}