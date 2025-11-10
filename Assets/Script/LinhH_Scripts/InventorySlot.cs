using GameUI;
using UnityEngine;
using UnityEngine.EventSystems;
using Management;

public class InventorySlot : MonoBehaviour, IDropHandler
{
    [SerializeField] private GameObject selectedHighlight;


    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return; // không có gì để thả

        Transform originSlot = eventData.pointerDrag.GetComponent<DragableItem>().parentBeforeDrag;
        DragableItem draggedItem = eventData.pointerDrag.GetComponent<DragableItem>();

        // TH1: Thả vào ô trống
        if (transform.childCount == GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT)
        {
            draggedItem.parentAfterDrag = transform;
        }
        // TH2: Thả vào ô đã có item
        else
        {
            DragableItem itemInSlot = transform.GetComponentInChildren<DragableItem>();
            if (itemInSlot == null) return;

            // --- KIỂM TRA MỒI CÂU ---
            var draggedSO = draggedItem.itemScriptableObj;
            var inSlotSO = itemInSlot.itemScriptableObj;

            // 1: Kéo MỒI (dragged) thả vào CẦN CÂU (inSlot)
            if (draggedSO is BaitSO baitData && inSlotSO is FishingRodSO)
            {
                // Thử gắn mồi vào Cần câu (itemInSlot)
                bool success = itemInSlot.TryAttachBait(baitData, draggedItem.quantity);
                if (success)
                {
                    // Gắn thành công -> Hủy GameObject mồi vừa kéo
                    Destroy(draggedItem.gameObject);
                }
                else
                {
                    // Gắn thất bại (cần tre, khác loại mồi) -> Hoán đổi như cũ
                    PerformSwap(draggedItem, itemInSlot, originSlot);
                }
            }
            //  2: Kéo CẦN CÂU (dragged) thả vào MỒI (inSlot)
            else if (draggedSO is FishingRodSO && inSlotSO is BaitSO baitDataSlot)
            {
                // Thử gắn mồi (itemInSlot) vào Cần câu (draggedItem)
                bool success = draggedItem.TryAttachBait(baitDataSlot, itemInSlot.quantity);
                if (success)
                {
                    // Gắn thành công -> Hủy GameObject mồi (itemInSlot)
                    Destroy(itemInSlot.gameObject);
                    // Di chuyển cần câu vào slot mới
                    draggedItem.parentAfterDrag = transform;
                }
                else
                {
                    // Gắn thất bại -> Hoán đổi như cũ
                    PerformSwap(draggedItem, itemInSlot, originSlot);
                }
            }
            // 3: Không liên quan -> Hoán đổi như cũ
            else
            {
                PerformSwap(draggedItem, itemInSlot, originSlot);
            }
        }
    }


    /// <summary>
    /// Tách hàm hoán đổi (swap) ra cho sạch
    /// </summary>
    private void PerformSwap(DragableItem dragItem, DragableItem itemInSlot, Transform originSlot)
    {
        // thiết lập vị trí của item tại vị trí thả
        itemInSlot.transform.SetParent(originSlot);
        itemInSlot.transform.localPosition = Vector3.zero;

        // thiết lập vị trí của item được kéo
        dragItem.parentAfterDrag = transform;
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
