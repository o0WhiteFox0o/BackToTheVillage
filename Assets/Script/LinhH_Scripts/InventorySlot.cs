using GameUI;
using UnityEngine;
using UnityEngine.EventSystems;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject selectedHighlight;


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return; // không có gì để thả

        var originSlot = eventData.pointerDrag.GetComponent<DragableItem>().parentBeforeDrag;

        // thả item vào slot nếu slot trống
        if (transform.childCount == GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT)
        {
            DragableItem inventoryItem = eventData.pointerDrag.GetComponent<DragableItem>();
            inventoryItem.parentAfterDrag = transform;
        }
        // đổi chổ 2 item nếu slot có chứa item
        else
        {
            // lấy vị trí slot của item được kéo và item tại vị trí người chơi thả
            var dragItem = eventData.pointerDrag.GetComponent<DragableItem>();
            var itemInSlot = transform.GetComponentInChildren<DragableItem>();

            // thiết lập vị trí của item tại vị trí thả
            itemInSlot.transform.SetParent(originSlot);
            itemInSlot.transform.localPosition = Vector3.zero;

            // thiết lập vị trí của item được kéo
            dragItem.parentAfterDrag = transform;
        }
    }


    public void HighlightSlot()
    {
        selectedHighlight.SetActive(true);
    }


    public void DisableHighlight()
    {
        selectedHighlight.SetActive(false);
    }
}
