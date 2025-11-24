using GameUI;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using Management;

// GẮN SCRIPT NÀY VÀO PREFAB "InventorySlot"
public class InventorySlot : MonoBehaviour, IDropHandler, IPointerClickHandler, IPointerEnterHandler, IPointerExitHandler
{
    [SerializeField] private GameObject selectedHighlight;

    private UI_GameplayUIManager gameplayUIManager;


    private void Start()
    {
        gameplayUIManager = FindObjectOfType<UI_GameplayUIManager>();

        if (gameplayUIManager == null)
        {
            Debug.LogError("Can't load a component of Inventory Slot.");
        }
    }


    /// <summary>
    /// Xử lý CHUỘT PHẢI (Chia Stack / Gỡ Mồi)
    /// VÀ CHUỘT TRÁI (Thả item sau khi chia)
    /// </summary>
    public void OnPointerClick(PointerEventData eventData)
    {
        DragableItem itemOnCursor = DragableItem.itemBeingHeld;
        DragableItem itemInSlot = GetComponentInChildren<DragableItem>();

        // ==========================================================
        // CASE 1: TAY ĐANG CẦM ITEM (TỪ SPLIT)
        // ==========================================================
        if (itemOnCursor != null)
        {
            // Nếu đang cầm item, click chuột trái là để "THẢ"
            if (eventData.button == PointerEventData.InputButton.Left)
            {
                if (itemInSlot == null)
                {
                    PlaceItemInEmptySlot(itemOnCursor);
                }
                else
                {
                    HandleDropOnFilledSlot(itemOnCursor, itemInSlot, itemOnCursor.parentBeforeDrag); // Dùng logic Gộp/Swap
                }
            }
            // (Thêm logic thả 1 item bằng chuột phải nếu muốn)
            return;
        }

        // ==========================================================
        // CASE 2: TAY ĐANG RỖNG (itemOnCursor == null)
        // ==========================================================
        if (eventData.button == PointerEventData.InputButton.Right)
        {
            if (itemInSlot == null) return; // Click phải ô trống

            // --- LOGIC 2A: GỠ MỒI ---
            if (itemInSlot.itemScriptableObj.itemType == ItemType.FishingRod &&
                itemInSlot.attachedBait != null)
            {
                TryRemoveBait(itemInSlot);
            }
            // --- LOGIC 2B: CHIA STACK ---
            else if (itemInSlot.itemScriptableObj.stackable && itemInSlot.quantity > 1)
            {
                SplitStack(itemInSlot);
            }
        }
    }

    /// <summary>
    /// Xử lý khi KÉO-THẢ-THƯỜNG (Chuột trái)
    /// </summary>
    public void OnDrop(PointerEventData eventData)
    {
        if (eventData.pointerDrag == null) return;

        DragableItem draggedItem = eventData.pointerDrag.GetComponent<DragableItem>();
        Transform originSlot = draggedItem.parentBeforeDrag;

        // TH1: Thả vào ô trống
        if (transform.childCount == GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT)
        {
            draggedItem.parentAfterDrag = transform;
        }
        // TH2: Thả vào ô đã có item
        else
        {
            DragableItem itemInSlot = transform.GetComponentInChildren<DragableItem>();
            if (itemInSlot == null || itemInSlot == draggedItem) return;

            // Xử lý Gộp, Gắn Mồi, hoặc Hoán đổi
            HandleDropOnFilledSlot(draggedItem, itemInSlot, originSlot);
        }
    }

    /// <summary>
    /// Xử lý logic khi thả 1 item (itemOnCursor) lên 1 item khác (itemInSlot)
    /// </summary>
    private void HandleDropOnFilledSlot(DragableItem itemOnCursor, DragableItem itemInSlot, Transform originSlot)
    {
        var draggedSO = itemOnCursor.itemScriptableObj;
        var inSlotSO = itemInSlot.itemScriptableObj;

        // Logic 1: Gộp Stack
        if (draggedSO == inSlotSO && draggedSO.stackable)
        {
            itemInSlot.AddCount(itemOnCursor.quantity);
            Destroy(itemOnCursor.gameObject);
        }
        // Logic 2: Kéo MỒI (dragged) thả vào CẦN CÂU (inSlot)
        else if (draggedSO is BaitSO baitData && inSlotSO is FishingRodSO)
        {
            if (itemInSlot.TryAttachBait(baitData, itemOnCursor.quantity))
            {
                Destroy(itemOnCursor.gameObject);
            }
            else
            {
                PerformSwap(itemOnCursor, itemInSlot, originSlot); // Gắn thất bại -> Swap
            }
        }
        // Logic 3: Kéo CẦN CÂU (dragged) thả vào MỒI (inSlot)
        else if (draggedSO is FishingRodSO && inSlotSO is BaitSO baitDataSlot)
        {
            if (itemOnCursor.TryAttachBait(baitDataSlot, itemInSlot.quantity))
            {
                Destroy(itemInSlot.gameObject); // Hủy mồi
                itemOnCursor.parentAfterDrag = transform; // Di chuyển cần câu vào
            }
            else
            {
                PerformSwap(itemOnCursor, itemInSlot, originSlot); // Gắn thất bại -> Swap
            }
        }
        // Logic 4: Không liên quan -> Hoán đổi
        else
        {
            PerformSwap(itemOnCursor, itemInSlot, originSlot);
        }

        // Nếu item đang cầm là từ (Split), thì giờ nó đã được thả
        if (DragableItem.itemBeingHeld == itemOnCursor)
        {
            DragableItem.itemBeingHeld = null;
            itemOnCursor.GetComponent<Image>().raycastTarget = true;
        }
    }

    private void PerformSwap(DragableItem dragItem, DragableItem itemInSlot, Transform originSlot)
    {
        itemInSlot.transform.SetParent(originSlot);
        itemInSlot.transform.localPosition = Vector3.zero;
        itemInSlot.parentAfterDrag = originSlot; // Cập nhật cho item trong slot

        dragItem.parentAfterDrag = transform;
    }


    private void TryRemoveBait(DragableItem rod)
    {
        BaitSO baitToReturn = rod.attachedBait;
        int quantityToReturn = rod.baitQuantity;
        if (baitToReturn == null || quantityToReturn <= 0) return;

        if (InventoryManager.Instance.AddItem(baitToReturn, quantityToReturn))
        {
            rod.attachedBait = null;
            rod.baitQuantity = 0;
            rod.UpdateBaitVisuals();
        }
    }

    private void SplitStack(DragableItem itemInSlot)
    {
        int quantityToMove = Mathf.CeilToInt(itemInSlot.quantity / 2.0f);

        itemInSlot.SubtractCount(quantityToMove);

        GameObject newItemObj = Instantiate(InventoryManager.Instance.itemPrefab, transform.root);
        DragableItem newItem = newItemObj.GetComponent<DragableItem>();
        newItem.InitializeItem(itemInSlot.itemScriptableObj, quantityToMove);
        newItem.attachedBait = itemInSlot.attachedBait;
        newItem.baitQuantity = itemInSlot.baitQuantity;
        newItem.UpdateBaitVisuals();

        DragableItem.itemBeingHeld = newItem; // "Cầm" item lên
        newItem.GetComponent<Image>().raycastTarget = false;
    }

    // Đặt item (từ split) vào ô trống
    private void PlaceItemInEmptySlot(DragableItem itemOnCursor)
    {
        itemOnCursor.transform.SetParent(transform);
        itemOnCursor.transform.localPosition = Vector3.zero;
        itemOnCursor.parentAfterDrag = transform;
        itemOnCursor.GetComponent<Image>().raycastTarget = true;
        DragableItem.itemBeingHeld = null; // Thả tay
    }

    public void HighlightSlot()
    {
        selectedHighlight.SetActive(true);
    }

    public void DisableHighlight()
    {
        selectedHighlight.SetActive(false);
    }


    /// <summary>
    /// Được gọi khi con trỏ chuột hover vào slot.
    /// </summary>
    public void OnPointerEnter(PointerEventData eventData)
    {
        // nếu slot không chứa vật phẩm nào thì không làm gì
        if (transform.childCount == GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT) { return; }

        gameplayUIManager.EnableItemInfoUI(transform);
    }


    /// <summary>
    /// Được gọi khi con trỏ chuột di chuyển ra khỏi slot.
    /// </summary>
    public void OnPointerExit(PointerEventData eventData)
    {
        // nếu slot không chứa vật phẩm nào thì không làm gì
        if (transform.childCount == GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT) { return; }

        gameplayUIManager.DisableItemInfoUI();
    }
}