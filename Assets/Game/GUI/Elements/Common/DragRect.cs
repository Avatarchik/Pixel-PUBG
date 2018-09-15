using UnityEngine.EventSystems;
using UnityEngine.UI;
using UnityEngine;

public class DragRect : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{

    private RectTransform Rect;
    void Start()
    {
        Rect = this.GetComponent<RectTransform>();
    }

    void Update()
    {

    }

    Vector2 offset;
    public void OnBeginDrag(PointerEventData eventData)
    {
        offset = Rect.anchoredPosition - eventData.position;
    }

    public void OnDrag(PointerEventData data)
    {
        Rect.anchoredPosition = data.position + offset;
    }

    private void SetDraggedPosition(PointerEventData data)
    {

    }

    public void OnEndDrag(PointerEventData eventData)
    {

    }
}
