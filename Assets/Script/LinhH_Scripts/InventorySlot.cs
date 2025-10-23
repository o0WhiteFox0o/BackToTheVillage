using GameUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return; // không có gì để thả
        if (eventData.pointerDrag != null && transform.childCount == 0)
        {
            DragableItem inventoryItem = eventData.pointerDrag.GetComponent<DragableItem>();
            inventoryItem.parentAfterDrag = transform;
        }
    }
}
