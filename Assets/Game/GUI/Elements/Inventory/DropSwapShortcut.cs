using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropSwapShortcut : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	private GUIItemCollector guiItem;
	
	public void Start ()
	{
		guiItem = this.GetComponent<GUIItemCollector> ();
	}
	
	public void OnDrop (PointerEventData data)
	{
		// shortcut can be swap with another shortcut. if it dropped on eachother
		if(UnitZ.playerManager && UnitZ.playerManager.PlayingCharacter){
			if (GetDropSprite (data) != null)
				UnitZ.playerManager.PlayingCharacter.inventory.SwarpShortcut (guiItem.Item, GetDropSprite (data).Item);
		}
	}

	public void OnPointerEnter (PointerEventData data)
	{
		// no script here
	}

	public void OnPointerExit (PointerEventData data)
	{
		// no script here
	}
	
	private GUIItemCollector GetDropSprite (PointerEventData data)
	{
		// get component on any object when a cusor is onver on it
		var originalObj = data.pointerDrag;
		if (originalObj == null)
			return null;

		var srcImage = originalObj.GetComponent<GUIItemCollector> ();
		if (srcImage == null)
			return null;
		
		return srcImage;
	}
}