using Management;
using UnityEngine;
using UnityEngine.Tilemaps;

public class TrangSoilInteraction : MonoBehaviour
{
    [Header("Tham chiếu bắt buộc")]
    public TileCursorFollow tileCursorFollow;   // Lấy tilemap + con trỏ từ đây
    public InventoryManager inventory;          // Lấy item id từ inventory

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

        tilemap = tileCursorFollow.targetTilemap;
    }

    void Update()
    {
        if (tilemap == null || tileCursorFollow.cursorObject == null || inventory == null)
            return;

        // Kiểm tra khi nhấn chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            var currentItem = inventory.holdingItem;
            if (currentItem == null)
                return;

            Vector3Int cellPos = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);
            TileBase tile = tilemap.GetTile(cellPos);
            if (tile == null) return;

            Sprite currentSprite = tilemap.GetSprite(cellPos);

            // =========================
            // 🔹 XÉT THEO ITEM TYPE TRƯỚC
            // =========================
            switch (currentItem.itemType)
            {
                // ----- Dụng cụ cuốc đất -----
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

                // ----- Dụng cụ tưới nước -----
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

                // ----- Hạt giống -----
                case ItemType.Seed:
                    // Chỉ trồng trên đất xới hoặc đất có nước
                    if (currentSprite == tilledSprite || currentSprite == wateredSprite)
                    {
                        // Lấy prefab từ item
                        if (currentItem.plantPrefab != null)
                        {
                            // Tính vị trí trung tâm ô tile
                            Vector3 spawnPos = tilemap.CellToWorld(cellPos) + new Vector3(0f, 0.5f, 0f);

                            // Sinh cây ra tại ô được chọn
                            Instantiate(currentItem.plantPrefab, spawnPos, Quaternion.identity);

                            Debug.Log($"🌱 Đã trồng {currentItem.id} tại {cellPos}");
                        }
                        else
                        {
                            Debug.LogWarning($"⚠ Hạt giống {currentItem.id} chưa có prefab cây để trồng!");
                        }
                    }
                    else
                    {
                        Debug.Log("❌ Không thể trồng ở đây. Cần đất đã xới hoặc tưới.");
                    }
                    break;

                // ----- Các loại khác -----
                default:
                    Debug.Log($"⚙ Không có hành động với item type {currentItem.itemType}");
                    break;
            }
        }
    }
}
