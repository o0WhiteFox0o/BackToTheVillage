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

    private FishingZone currentValidZone;
    private Vector3 cursorGridPosition;
    private bool isPlacementValid = false;

    void Update()
    {
        ItemScriptableObject holdingItem = inventoryManager.holdingItem;

        bool isHoldingTrap = (holdingItem != null && holdingItem.itemType == ItemType.Trap);
        bool isHoldingBait = (holdingItem != null && holdingItem.itemType == ItemType.Bait);

        if (isHoldingTrap && tileCursorFollow.IsCursorActive())
        {
            HandlePlacement(holdingItem);
        }
        else if (Input.GetKeyDown(KeyCode.F))
        {
            HandleInteraction(holdingItem, isHoldingBait);
        }
        else if (!isHoldingTrap && tileCursorFollow.IsCursorActive())
        {
            tileCursorFollow.SetCursorActive(false);
        }
    }

    private void HandlePlacement(ItemScriptableObject trapItem)
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        Vector3Int cellPos = targetTilemap.WorldToCell(mouseWorldPos);
        cursorGridPosition = targetTilemap.GetCellCenterWorld(cellPos);
        cursorGridPosition.z = 0;

        ValidatePlacement();
        tileCursorFollow.SetPlacementValid(isPlacementValid);

        if (isPlacementValid && Input.GetKeyDown(KeyCode.F))
        {
            if (inventoryManager.RemoveItem(trapItem, 1))
            {
                PlaceTrapObject(trapItem);
            }
        }
    }

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
            placedTrap.Initialize(currentValidZone);
        }
        else
        {
            Debug.LogError($"Prefab của {trapItem.name} thiếu script PlacedTrap!");
        }
    }


    /// <summary>
    /// Xử lý tương tác (Thêm mồi, Thu hoạch, HOẶC Nhặt lại bẫy)
    /// </summary>
    private void HandleInteraction(ItemScriptableObject holdingItem, bool isHoldingBait)
    {
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = -Camera.main.transform.position.z;
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        Collider2D hit = Physics2D.OverlapPoint(mouseWorldPos, placedObjectLayer);

        if (hit == null) return;

        PlacedTrap trap = hit.GetComponent<PlacedTrap>();
        if (trap == null) return;

        if (isHoldingBait)
        {
            // 1. Ưu tiên: Cầm mồi -> Thử đặt mồi
            BaitSO bait = (BaitSO)holdingItem;
            trap.TryAddBait(bait, inventoryManager);
        }
        else
        {
            // 2. Không cầm mồi (cầm tay không, cầm cuốc, v.v.)
            switch (trap.CurrentState)
            {
                case PlacedTrap.TrapState.ReadyToCollect:
                    // 2a. Bẫy đầy -> Thu hoạch cá
                    trap.TryCollect(inventoryManager);
                    break;

                case PlacedTrap.TrapState.Empty:
                    // 2b. Bẫy rỗng -> Nhặt lại bẫy
                    // (Kiểm tra xem trapItemSO đã được gán trong Prefab chưa)
                    if (trap.trapItemSO != null)
                    {
                        if (inventoryManager.AddItem(trap.trapItemSO, 1))
                        {
                            Destroy(trap.gameObject); // Nhặt thành công, xóa bẫy
                        }
                        else
                        {
                            Debug.Log("Túi đồ đầy, không thể nhặt bẫy!");
                        }
                    }
                    else
                    {
                        Debug.LogError("LỖI: Chưa gán 'Trap Item SO' trên Prefab bẫy!");
                    }
                    break;

                case PlacedTrap.TrapState.Baited:
                    // 2c. Bẫy đang có mồi -> Không làm gì cả
                    Debug.Log("Bẫy đang hoạt động, không thể nhặt.");
                    break;
            }
        }
    }
}