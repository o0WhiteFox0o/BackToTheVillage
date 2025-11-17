using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;
using Management;

public class SoilInteraction : MonoBehaviour
{
    [Header("Tham chiếu bắt buộc")]
    public TileCursorFollow tileCursorFollow;   // Con trỏ + tilemap
    public InventoryManager inventory;          // Lấy item hiện cầm
    public PlantedManager plantedManager;       // Quản lý các vị trí cây đã trồng

    [Header("Các sprite tile")]
    public Sprite grassSprite;    // đất cỏ
    public Sprite tilledSprite;   // đất xới
    public Sprite wateredSprite;  // đất xới có nước

    private Tilemap tilemap;

    public delegate void ToolUseAnimation(string triggerName);
    public static event ToolUseAnimation OnToolUse;

    private void OnEnable()
    {
        InputManager.OnLeftClick += HandleLeftClick;
    }

    private void OnDisable()
    {
        InputManager.OnLeftClick -= HandleLeftClick;
    }

    void Start()
    {
        if (tileCursorFollow == null)
        {
            Debug.LogError("Chưa gán TileCursorFollow!");
            enabled = false;
            return;
        }

        if (plantedManager == null)
        {
            Debug.LogError("Chưa gán PlantedManager!");
            enabled = false;
            return;
        }

        tilemap = tileCursorFollow.targetTilemap;
    }

    void Update()
    {
        if (tileCursorFollow == null || tileCursorFollow.cursorObject == null || tilemap == null)
            return;

        // Lấy ô của Player và ô của con trỏ
        Vector3Int playerCell = tilemap.WorldToCell(transform.position);
        Vector3Int cursorCell = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);

        int dx = Mathf.Abs(cursorCell.x - playerCell.x);
        int dy = Mathf.Abs(cursorCell.y - playerCell.y);

        // Hợp lệ nếu ở ô Player hoặc 8 ô liền kề
        bool isValid = (dx <= 1 && dy <= 1);
        tileCursorFollow.SetPlacementValid(isValid);
    }

    private void HandleLeftClick(Vector2 mouseWorldPos)
    {
        if (tilemap == null || tileCursorFollow.cursorObject == null || inventory == null)
            return;

        Vector3Int playerCell = tilemap.WorldToCell(transform.position);
        Vector3Int cursorCell = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);

        int dx = Mathf.Abs(cursorCell.x - playerCell.x);
        int dy = Mathf.Abs(cursorCell.y - playerCell.y);

        // Kiểm tra hợp lệ ô Player hoặc 8 ô xung quanh
        if (dx > 1 || dy > 1)
        {
            tileCursorFollow.SetPlacementValid(false);
            Debug.Log("❌ Chỉ thao tác tại ô Player hoặc 8 ô liền kề!");
            return;
        }

        tileCursorFollow.SetPlacementValid(true);

        var currentItem = inventory.holdingItem;
        if (currentItem == null) return;

        TileBase tile = tilemap.GetTile(cursorCell);
        if (tile == null) return;

        Sprite currentSprite = tilemap.GetSprite(cursorCell);

        switch (currentItem.itemType)
        {
            case ItemType.Hoe:
                if (currentSprite == grassSprite)
                {
                    Tile newTile = ScriptableObject.CreateInstance<Tile>();
                    newTile.sprite = tilledSprite;
                    newTile.name = "TilledSoil";
                    tilemap.SetTile(cursorCell, newTile);
                    Debug.Log($"⛏ Đã xới đất tại {cursorCell}");
                    Debug.Log($"⛏ Đã xới đất tại vị trí Player {playerCell}");
                    OnToolUse?.Invoke("isDigging");
                }
                break;

            case ItemType.WateringCan:
                if (currentSprite == tilledSprite)
                {
                    Tile newTile = ScriptableObject.CreateInstance<Tile>();
                    newTile.sprite = wateredSprite;
                    newTile.name = "WateredSoil";
                    tilemap.SetTile(cursorCell, newTile);
                    Debug.Log($"💧 Đã tưới đất tại {cursorCell}");
                    OnToolUse?.Invoke("isWatering");
                }
                break;

            case ItemType.Seed:
                if (currentSprite != tilledSprite && currentSprite != wateredSprite)
                {
                    Debug.Log("❌ Không thể trồng ở đây. Cần đất đã xới hoặc tưới.");
                    return;
                }

                if (plantedManager.IsPositionOccupied(cursorCell))
                {
                    Debug.Log("❌ Vị trí này đã có cây rồi!");
                    return;
                }

                if (currentItem.plantPrefab != null)
                {
                    Vector3 spawnPos = tilemap.CellToWorld(cursorCell) + new Vector3(0f, 0.5f, 0f);
                    Instantiate(currentItem.plantPrefab, spawnPos, Quaternion.identity);

                    plantedManager.AddPosition(cursorCell);
                    inventory.RemoveItem(currentItem, 1);

                    Debug.Log($"🌱 Đã trồng {currentItem.id} tại {cursorCell}");
                }
                else
                {
                    Debug.LogWarning($"⚠ Hạt giống {currentItem.id} chưa có prefab cây để trồng!");
                }
                break;

            default:
                Debug.Log($"⚙ Không có hành động với item type {currentItem.itemType}");
                break;
        }
    }

    /// <summary>
    /// Khô lại tất cả ô đất đã tưới khi qua ngày
    /// </summary>
    public void DryAllWateredTiles()
    {
        if (tilemap == null) return;

        BoundsInt bounds = tilemap.cellBounds;
        int dryCount = 0;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            Sprite sprite = tilemap.GetSprite(pos);
            if (sprite == wateredSprite)
            {
                Tile newTile = ScriptableObject.CreateInstance<Tile>();
                newTile.sprite = tilledSprite;
                newTile.name = "TilledSoil";
                tilemap.SetTile(pos, newTile);
                dryCount++;
            }
        }

        if (dryCount > 0)
            Debug.Log($"🌤 {dryCount} ô đất đã khô lại sau khi qua ngày!");
    }
}
