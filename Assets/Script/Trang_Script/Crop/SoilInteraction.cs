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

        Vector3Int playerCell = tilemap.WorldToCell(transform.position);
        Vector3Int cursorCell = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);

        int dx = Mathf.Abs(cursorCell.x - playerCell.x);
        int dy = Mathf.Abs(cursorCell.y - playerCell.y);

        // ❗ Hợp lệ khi 4 hướng hoặc chính giữa
        bool isValid = (dx + dy == 1) || (dx == 0 && dy == 0);

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

        // ❗ Cho 4 ô xung quanh + ô hiện tại
        bool isValidSpot = (dx + dy == 1) || (dx == 0 && dy == 0);

        if (!isValidSpot)
        {
            tileCursorFollow.SetPlacementValid(false);
            Debug.Log("❌ Chỉ thao tác 4 ô xung quanh hoặc ô đang đứng!");
            return;
        }

        tileCursorFollow.SetPlacementValid(true);

        var currentItem = inventory.holdingItem;
        if (currentItem == null) return;

        TileBase tile = tilemap.GetTile(cursorCell);
        if (tile == null) return;

        Sprite currentSprite = tilemap.GetSprite(cursorCell);

        // === Các case như cũ ===
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
                    Debug.Log("❌ Cần đất xới hoặc đất tưới để trồng!");
                    return;
                }

                if (plantedManager.IsPositionOccupied(cursorCell))
                {
                    Debug.Log("❌ Vị trí này đã có cây!");
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
                else Debug.LogWarning($"⚠ Seed {currentItem.id} chưa có prefab!");
                break;
        }
    }


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
