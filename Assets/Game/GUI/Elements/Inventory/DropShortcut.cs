using System.Reflection;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class DropShortcut : MonoBehaviour, IDropHandler, IPointerEnterHandler, IPointerExitHandler
{
	public int ShortcutIndex;
	public KeyCode Key;
	private GUIItemCollector guiItem;
	
	public void Start ()
	{
		guiItem = this.GetComponent<GUIItemCollector> ();
	}
	
	public void OnDrop (PointerEventData data)
	{
		if (UnitZ.playerManager == null || UnitZ.playerManager.PlayingCharacter == null || guiItem == null || data == null)
			return;
		
		ItemCollector itemDrag = GetItemDrop (data);
		// shortcut can be swap with another shortcut. if it dropped on eachother
		if (itemDrag != null) {
			UnitZ.playerManager.PlayingCharacter.inventory.SwarpShortcut (itemDrag, guiItem.Item);
			itemDrag.Shortcut = ShortcutIndex;
			guiItem.SetItemCollector (itemDrag);
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
	
	public void UseItem ()
	{
		if (guiItem != null && guiItem.Item != null && UnitZ.playerManager != null && UnitZ.playerManager.PlayingCharacter != null) {
			UnitZ.playerManager.PlayingCharacter.inventory.EquipItemByCollector (guiItem.Item);
		}	
	}
	
	public void LateUpdate ()
	{
		if (guiItem == null || UnitZ.playerManager == null || UnitZ.playerManager.PlayingCharacter == null || UnitZ.playerManager.PlayingCharacter.inventory == null)
			return;
		
		// Get actual item data to the shortcut by matching a shortcut index with items in inventory.
		ItemCollector item = UnitZ.playerManager.PlayingCharacter.inventory.GetItemCollectorByShortCutIndex(ShortcutIndex);
		guiItem.SetItemCollector (item);	
		
	}
	
	public void Update ()
	{
		// press a key to use a shortcut item.
		if (Input.GetKeyDown (Key)) {
			UseItem ();
		}
	}
	
	private ItemCollector GetItemDrop (PointerEventData data)
	{
		// get component on any object when a cusor is onver on it
		var originalObj = data.pointerDrag;
		
		if (originalObj == null){
			return null;
		}
		
		if(originalObj.GetComponent<DragItem> ()){
			return originalObj.GetComponent<DragItem> ().GUIItem.Item;
		}
		
		if(originalObj.GetComponent<GUIItemCollector> ()){
			return originalObj.GetComponent<GUIItemCollector> ().Item;
		}
		
		return null;
	}
	
	

}