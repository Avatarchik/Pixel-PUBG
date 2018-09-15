using UnityEngine;
using UnityEngine.EventSystems;
using System.Collections;

public class TooltipTrigger : MonoBehaviour, IPointerEnterHandler, IPointerExitHandler, ISelectHandler, IDeselectHandler,IPointerClickHandler
{
	// add this script to any object with GUIItemCollector component attached.
	
	public ItemCollector Item;
	public string Type = "inventory";
	private Vector3 pointerPosition;

	public void Start ()
	{
		
	}

	public void OnPointerClick (PointerEventData eventData)
	{
		pointerPosition = new Vector3 (eventData.position.x, eventData.position.y - 18f, 0f);
		Select (pointerPosition);

	}

	public void OnPointerEnter (PointerEventData eventData)
	{
		// show TooltipDetails when mouse is over it
		GUIItemCollector guiItem = this.GetComponent<GUIItemCollector> ();
		if (guiItem)
			Item = guiItem.Item;
		
		if (TooltipDetails.Instance) {
			StartCoroutine (TooltipDetails.Instance.OnHover (eventData, Item));
		}
	}

	public void OnSelect (BaseEventData eventData)
	{
		// no script here
	}

	public void OnPointerExit (PointerEventData eventData)
	{
		if (TooltipDetails.Instance)
			TooltipDetails.Instance.hover = false;
	}

	public void OnDeselect (BaseEventData eventData)
	{
		// no script here
	}

	void Select (Vector3 position)
	{
		GUIItemCollector guiItem = this.GetComponent<GUIItemCollector> ();
		if (guiItem)
			Item = guiItem.Item;
		
		if (Item != null) {
			if (TooltipUsing.Instance){
				TooltipUsing.Instance.ShowTooltip (Item, position, Type);
			}else{
				Debug.Log(TooltipUsing.Instance);
			}
		}
		if (TooltipDetails.Instance)
			TooltipDetails.Instance.HideTooltip ();
	}

}
