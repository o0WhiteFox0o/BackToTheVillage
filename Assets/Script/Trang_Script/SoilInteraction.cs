using Management;
using UnityEngine;
using UnityEngine.Tilemaps;

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
        if (tilemap == null || tileCursorFollow.cursorObject == null || inventory == null)
            return;

        if (Input.GetMouseButtonDown(0))
        {
            var currentItem = inventory.holdingItem;
            if (currentItem == null) return;

            Vector3Int cellPos = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);
            TileBase tile = tilemap.GetTile(cellPos);
            if (tile == null) return;

            Sprite currentSprite = tilemap.GetSprite(cellPos);

            switch (currentItem.itemType)
            {
                case ItemType.Hoe:
                    if (currentSprite == grassSprite)
                    {
                        Tile newTile = ScriptableObject.CreateInstance<Tile>();
                        newTile.sprite = tilledSprite;
                        newTile.name = "TilledSoil";
                        tilemap.SetTile(cellPos, newTile);
                        Debug.Log($"⛏ Đã xới đất tại {cellPos}");
                    }
                    break;

                case ItemType.WateringCan:
                    if (currentSprite == tilledSprite)
                    {
                        Tile newTile = ScriptableObject.CreateInstance<Tile>();
                        newTile.sprite = wateredSprite;
                        newTile.name = "WateredSoil";
                        tilemap.SetTile(cellPos, newTile);
                        Debug.Log($"💧 Đã tưới đất tại {cellPos}");
                    }
                    break;

                case ItemType.Seed:
                    if (currentSprite != tilledSprite && currentSprite != wateredSprite)
                    {
                        Debug.Log("❌ Không thể trồng ở đây. Cần đất đã xới hoặc tưới.");
                        return;
                    }

                    // ✅ Kiểm tra trùng
                    if (plantedManager.IsPositionOccupied(cellPos))
                    {
                        Debug.Log("❌ Vị trí này đã có cây rồi!");                     
                        return;
                    }

                    // Trồng cây
                    if (currentItem.plantPrefab != null)
                    {
                        Vector3 spawnPos = tilemap.CellToWorld(cellPos) + new Vector3(0f, 0.5f, 0f);
                        Instantiate(currentItem.plantPrefab, spawnPos, Quaternion.identity);

                        plantedManager.AddPosition(cellPos);
                        inventory.RemoveItem(currentItem, 1);

                        Debug.Log($"🌱 Đã trồng {currentItem.id} tại {cellPos}");
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
    }
}
