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
    public SoilManager soilManager;             // Quản lý trạng thái đất

    [Header("Sprite đất")]
    public List<Sprite> grassSprites;    // Các sprite cỏ
    public Sprite tilledSprite;          // đất xới
    public Sprite wateredSprite;         // đất đã tưới

    private Tilemap tilemap;

    public delegate void ToolUseAnimation(string triggerName);
    public static event ToolUseAnimation OnToolUse;

    private void OnEnable()
    {
        SYS_InputManager.OnLeftClick += HandleLeftClick;
    }

    private void OnDisable()
    {
        SYS_InputManager.OnLeftClick -= HandleLeftClick;
    }

    void Start()
    {
        if (tileCursorFollow == null || plantedManager == null || soilManager == null)
        {
            Debug.LogError("Chưa gán TileCursorFollow / PlantedManager / SoilManager!");
            enabled = false;
            return;
        }

        tilemap = tileCursorFollow.targetTilemap;
    }

    void Update()
    {
        if (tileCursorFollow == null || tileCursorFollow.cursorObject == null || tilemap == null || inventory == null)
            return;

        Vector3Int playerCell = tilemap.WorldToCell(transform.position);
        Vector3Int cursorCell = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);

        int dx = Mathf.Abs(cursorCell.x - playerCell.x);
        int dy = Mathf.Abs(cursorCell.y - playerCell.y);

        bool isNearby = (dx + dy == 1) || (dx == 0 && dy == 0);
        bool isValid = false;

        var currentItem = inventory.holdingItem;
        if (currentItem != null)
        {
            switch (currentItem.itemType)
            {
                case ItemType.Hoe:
                    // Cuốc có thể xới cỏ hoặc đất bất kỳ gần người
                    isValid = isNearby;
                    break;

                case ItemType.WateringCan:
                    // Tưới chỉ hợp lệ trên đất đã xới nhưng chưa tưới
                    isValid = isNearby && soilManager.IsHoed(cursorCell) && !soilManager.IsWatered(cursorCell);
                    break;

                case ItemType.Seed:
                    // Trồng chỉ hợp lệ trên đất đã xới hoặc đã tưới, chưa có cây
                    isValid = isNearby &&
                              (soilManager.IsHoed(cursorCell) || soilManager.IsWatered(cursorCell)) &&
                              !soilManager.IsPlanted(cursorCell);
                    break;
            }
        }

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

        switch (currentItem.itemType)
        {
            case ItemType.Hoe:
                if (grassSprites.Contains(currentSprite))
                {
                    OnToolUse?.Invoke("isDigging");
                    StartCoroutine(DelayedTilling(cursorCell, 0.8f));
                }
                break;

            case ItemType.WateringCan:
                if (soilManager.IsHoed(cursorCell) && !soilManager.IsWatered(cursorCell))
                {
                    OnToolUse?.Invoke("isWatering");
                    StartCoroutine(DelayedWatering(cursorCell, 0.8f));
                }
                break;

            case ItemType.Seed:
                if (!soilManager.IsHoed(cursorCell))
                {
                    Debug.Log("❌ Cần đất đã xới để trồng!");
                    return;
                }

                //if (!soilManager.IsWatered(cursorCell))
                //{
                //    Debug.Log("❌ Cần tưới đất trước khi trồng!");
                //    return;
                //}

                if (soilManager.IsPlanted(cursorCell))
                {
                    Debug.Log("❌ Vị trí này đã có cây!");
                    return;
                }

                if (currentItem.plantPrefab != null)
                {
                    Vector3 spawnPos = tilemap.CellToWorld(cursorCell) + new Vector3(0f, 0.5f, 0f);
                    GameObject plant = Instantiate(currentItem.plantPrefab, spawnPos, Quaternion.identity);
                    CropBehaviour cropBehaviour = plant.GetComponent<CropBehaviour>();

                    if (cropBehaviour != null)
                    {
                        cropBehaviour.soilManager = soilManager;    // thêm tham chiếu
                        cropBehaviour.cellPosition = cursorCell;    // lưu ô đất
                    }

                    soilManager.AddPlanted(cursorCell);
                    plantedManager.AddPosition(cursorCell);
                    inventory.RemoveItem(currentItem, 1);

                    Debug.Log($"🌱 Đã trồng {currentItem.id} tại {cursorCell}");
                }
                else Debug.LogWarning($"⚠ Seed {currentItem.id} chưa có prefab!");
                break;
        }
    }

    // --- Coroutine xới đất ---
    private IEnumerator DelayedTilling(Vector3Int cell, float delay)
    {
        yield return new WaitForSeconds(delay);

        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = tilledSprite;
        newTile.name = "TilledSoil";
        tilemap.SetTile(cell, newTile);

        soilManager.AddHoed(cell);

        Debug.Log($"⛏ Đã xới đất tại {cell}");
    }

    // --- Coroutine tưới đất ---
    private IEnumerator DelayedWatering(Vector3Int cell, float delay)
    {
        yield return new WaitForSeconds(delay);

        Tile newTile = ScriptableObject.CreateInstance<Tile>();
        newTile.sprite = wateredSprite;
        newTile.name = "WateredSoil";
        tilemap.SetTile(cell, newTile);

        soilManager.AddWatered(cell);

        Debug.Log($"💧 Đã tưới đất tại {cell}");
    }

    // --- Làm khô tất cả ô đã tưới ---
    public void DryAllWateredTiles()
    {
        if (tilemap == null) return;

        BoundsInt bounds = tilemap.cellBounds;
        int dryCount = 0;

        foreach (var pos in bounds.allPositionsWithin)
        {
            TileBase tile = tilemap.GetTile(pos);
            if (tile == null) continue;

            if (soilManager.IsWatered(pos))
            {
                Tile newTile = ScriptableObject.CreateInstance<Tile>();
                newTile.sprite = tilledSprite;
                newTile.name = "TilledSoil";
                tilemap.SetTile(pos, newTile);

                dryCount++;
            }
        }

        // Xoá trạng thái đã tưới
        soilManager.ClearWateredTiles();

        if (dryCount > 0)
            Debug.Log($"🌤 {dryCount} ô đất đã khô lại sau khi qua ngày!");
    }
}
