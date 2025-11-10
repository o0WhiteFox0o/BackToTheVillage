using Management;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrapInteraction : MonoBehaviour
{
    [Header("Tham chiếu bắt buộc")]
    [SerializeField] private InventoryManager inventoryManager;
    [SerializeField] private TileCursorFollow tileCursorFollow;
    [SerializeField] private Tilemap targetTilemap;

    [Header("Cấu hình Layer")]
    [SerializeField] private LayerMask fishingZoneLayer;
    [SerializeField] private LayerMask placedObjectLayer;

    // Biến nội bộ
    private FishingZone currentValidZone;
    private Vector3 cursorGridPosition;
    private bool isPlacementValid = false;

    void Update()
    {
        // 1. Lấy vật phẩm đang cầm
        ItemScriptableObject holdingItem = inventoryManager.holdingItem;

        bool isHoldingTrap = (holdingItem != null && holdingItem.itemType == ItemType.Trap);
        bool isHoldingBait = (holdingItem != null && holdingItem.itemType == ItemType.Bait); //

        // 2. Logic ĐẶT BẪY (Ưu tiên cao nhất)
        // Kích hoạt khi cầm bẫy và con trỏ đang bật
        if (isHoldingTrap && tileCursorFollow.IsCursorActive())
        {
            HandlePlacement(holdingItem);
        }
        // 3. Logic TƯƠNG TÁC (Thêm mồi / Thu hoạch)
        // Kích hoạt khi nhấn chuột phải
        else if (Input.GetMouseButtonDown(1))
        {
            HandleInteraction(holdingItem, isHoldingBait);
        }
        // 4. (Tùy chọn) Tắt con trỏ nếu đang bật mà không cầm bẫy
        else if (!isHoldingTrap && tileCursorFollow.IsCursorActive())
        {
            // Lỗi này không nên xảy ra vì InventoryManager xử lý, nhưng đây là dự phòng
            tileCursorFollow.SetCursorActive(false);
        }
    }

    /// <summary>
    /// Xử lý logic khi đang cầm bẫy (kiểm tra vị trí, đặt bẫy)
    /// </summary>
    private void HandlePlacement(ItemScriptableObject trapItem)
    {
        // (Phần này giữ nguyên từ code trước)
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector3Int cellPos = targetTilemap.WorldToCell(mouseWorldPos);
        cursorGridPosition = targetTilemap.GetCellCenterWorld(cellPos);

        ValidatePlacement();
        tileCursorFollow.SetPlacementValid(isPlacementValid); //

        if (isPlacementValid && Input.GetMouseButtonDown(0))
        {
            if (inventoryManager.RemoveItem(trapItem, 1))
            {
                PlaceTrapObject(trapItem);
            }
        }
    }

    /// <summary>
    /// (Hàm này giữ nguyên từ code trước)
    /// </summary>
    private void ValidatePlacement()
    {
        Collider2D fishZoneHit = Physics2D.OverlapPoint(cursorGridPosition, fishingZoneLayer);
        Collider2D objectHit = Physics2D.OverlapPoint(cursorGridPosition, placedObjectLayer);

        if (fishZoneHit != null && objectHit == null)
        {
            currentValidZone = fishZoneHit.GetComponent<FishingZone>();
            isPlacementValid = (currentValidZone != null);
        }
        else
        {
            isPlacementValid = false;
            currentValidZone = null;
        }
    }

    /// <summary>
    /// (Hàm này giữ nguyên từ code trước)
    /// </summary>
    private void PlaceTrapObject(ItemScriptableObject trapItem)
    {
        if (trapItem.itemPrefab == null)
        {
            Debug.LogError($"Item {trapItem.name} không có 'itemPrefab'!");
            return;
        }

        GameObject trapInstance = Instantiate(trapItem.itemPrefab, cursorGridPosition, Quaternion.identity);
        PlacedTrap placedTrap = trapInstance.GetComponent<PlacedTrap>();
        if (placedTrap != null)
        {
            // Bẫy bây giờ tự động khởi tạo ở trạng thái Empty
            placedTrap.Initialize(currentValidZone);
        }
        else
        {
            Debug.LogError($"Prefab của {trapItem.name} thiếu script PlacedTrap!");
        }
    }


    /// <summary>
    /// HÀM ĐƯỢC NÂNG CẤP: Xử lý click chuột phải (Thêm mồi hoặc Thu hoạch)
    /// </summary>
    private void HandleInteraction(ItemScriptableObject holdingItem, bool isHoldingBait)
    {
        // Lấy vị trí chuột
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        mouseWorldPos.z = 0;

        // Kiểm tra xem có click trúng bẫy không
        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, placedObjectLayer);

        if (hit == null) return; // Click vào không khí

        // Nếu click trúng, lấy script PlacedTrap
        PlacedTrap trap = hit.GetComponent<PlacedTrap>();
        if (trap == null) return; // Click trúng vật gì đó không phải bẫy

        // --- Logic TƯƠNG TÁC MỚI ---
        if (isHoldingBait)
        {
            // 1. Nếu đang cầm mồi: Thử thêm mồi vào bẫy
            // (Cần ép kiểu holdingItem sang BaitSO)
            BaitSO bait = (BaitSO)holdingItem;
            trap.TryAddBait(bait, inventoryManager);
        }
        else
        {
            // 2. Nếu không cầm mồi: Thử thu hoạch
            trap.TryCollect(inventoryManager);
        }
    }
}