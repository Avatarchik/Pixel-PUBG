using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

[RequireComponent (typeof(Image))]
public class DragItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
{
	public bool dragOnSurfaces = true;
	public GUIItemCollector GUIItem;
	
	private GameObject m_DraggingIcon;
	private RectTransform m_DraggingPlane;

	void Start ()
	{
		// instance a GUIItemCollector component
		if (GUIItem == null)
			GUIItem = this.GetComponent<GUIItemCollector> ();
		
		if (GUIItem != null && GUIItem.Item != null)
			GUIItem.Icon.sprite = GUIItem.Item.Item.ImageSprite;
	}


	// create icon function.
	public void OnBeginDrag (PointerEventData eventData)
	{
		var canvas = FindInParents<Canvas> (gameObject);
		if (canvas == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcon = UnitZ.Hud.ImageDrag.gameObject;
		m_DraggingIcon.SetActive (true);

		m_DraggingIcon.GetComponent<DragIconHanddle> ().Owner = this.transform.root.gameObject;
		m_DraggingIcon.GetComponent<DragIconHanddle> ().Type = 0;
		m_DraggingIcon.transform.SetAsLastSibling ();
		
		var image = m_DraggingIcon.GetComponent<Image> ();
        image.color = Color.white;
        image.sprite = GetComponent<GUIItemCollector>().Icon.sprite;
		image.GetComponent<RectTransform> ().sizeDelta = GetComponent<GUIItemCollector>().Icon.GetComponent<RectTransform> ().sizeDelta;
		//image.SetNativeSize();
		
		if (dragOnSurfaces)
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;
		
		SetDraggedPosition (eventData);
	}

	public void OnDrag (PointerEventData data)
	{
		if (m_DraggingIcon != null)
			SetDraggedPosition (data);
	}

	private void SetDraggedPosition (PointerEventData data)
	{
		// get 3d Position from pointer
		this.gameObject.SendMessage ("Draging", SendMessageOptions.DontRequireReceiver);
		if (dragOnSurfaces && data.pointerEnter != null && data.pointerEnter.transform as RectTransform != null)
			m_DraggingPlane = data.pointerEnter.transform as RectTransform;
		
		var rt = m_DraggingIcon.GetComponent<RectTransform> ();
		Vector3 globalMousePos;
		if (RectTransformUtility.ScreenPointToWorldPointInRectangle (m_DraggingPlane, data.position, data.pressEventCamera, out globalMousePos)) {
			rt.position = globalMousePos;
			rt.rotation = m_DraggingPlane.rotation;
		}
	}

	public void OnEndDrag (PointerEventData eventData)
	{
		// when stop draging. so remove icon
		if (m_DraggingIcon != null)
			m_DraggingIcon.SetActive (false);
	}

	static public T FindInParents<T> (GameObject go) where T : Component
	{
		// get component on any object when a cusor is onver on it
		if (go == null)
			return null;
		var comp = go.GetComponent<T> ();

		if (comp != null)
			return comp;
		
		Transform t = go.transform.parent;
		while (t != null && comp == null) {
			comp = t.gameObject.GetComponent<T> ();
			t = t.parent;
		}
		return comp;
	}
}
