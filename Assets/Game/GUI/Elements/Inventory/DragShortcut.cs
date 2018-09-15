using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DragShortcut : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler {

	public bool dragOnSurfaces = true;
	private GameObject m_DraggingIcon;
	private RectTransform m_DraggingPlane;
	
	void Start () {
	
	}
	
	public void OnBeginDrag(PointerEventData eventData)
	{
		var canvas = FindInParents<Canvas>(gameObject);
		if (canvas == null)
			return;

		// We have clicked something that can be dragged.
		// What we want to do is create an icon for this.
		m_DraggingIcon = UnitZ.Hud.ImageDrag.gameObject;
		m_DraggingIcon.SetActive (true);
		m_DraggingIcon.GetComponent<DragIconHanddle> ().Owner = this.transform.root.gameObject;
		m_DraggingIcon.GetComponent<DragIconHanddle> ().Type = 1;
		//m_DraggingIcon.transform.SetParent (canvas.transform, false);
		m_DraggingIcon.transform.SetAsLastSibling();
		
		var image = m_DraggingIcon.GetComponent<Image>();

		GameObject icon = this.gameObject.transform.Find("Icon").gameObject;
		if(icon!=null){
			image.color =  icon.GetComponent<Image>().color;
			image.sprite = icon.GetComponent<Image>().sprite;
		}
		image.GetComponent<RectTransform>().sizeDelta = this.GetComponent<RectTransform>().sizeDelta;
		//image.SetNativeSize();
		
		if (dragOnSurfaces)
			m_DraggingPlane = transform as RectTransform;
		else
			m_DraggingPlane = canvas.transform as RectTransform;
		
		SetDraggedPosition(eventData);
	}

	public void OnDrag(PointerEventData data)
	{
		if (m_DraggingIcon != null)
			SetDraggedPosition(data);
	}

	private void SetDraggedPosition(PointerEventData data)
	{
		// get 3d position from pointer data.
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
		// when stop draging. so remove icon
		if (m_DraggingIcon != null)
			m_DraggingIcon.SetActive (false);
	}

	static public T FindInParents<T>(GameObject go) where T : Component
	{
		// get component on any object when a cusor is onver on it
		if (go == null) return null;
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
