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
        if (tilemap == null || tileCursorFollow.cursorObject == null || inventory == null )
            return;

        // Kiểm tra khi nhấn chuột trái
        if (Input.GetMouseButtonDown(0))
        {
            // Lấy item hiện tại
            var currentItem = inventory.holdingItem;
            if (currentItem == null)
                return;

            // Nếu đang cầm cuốc (T001)
            if (currentItem.id == "T001")
            {
                Vector3Int cellPos = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);
                TileBase tile = tilemap.GetTile(cellPos);

                if (tile == null) return;

                Sprite currentSprite = tilemap.GetSprite(cellPos);

                // Nếu là đất cỏ -> đổi thành đất xới
                if (currentSprite == grassSprite)
                {
                    Tile newTile = ScriptableObject.CreateInstance<Tile>();
                    newTile.sprite = tilledSprite;
                    newTile.name = "TilledSoil";
                    tilemap.SetTile(cellPos, newTile);
                    Debug.Log($"⛏ Đã xới đất tại {cellPos}");
                }
            }

            // Nếu đang cầm bình tưới (S001)
            else if (currentItem.id == "S001")
            {
                Vector3Int cellPos = tilemap.WorldToCell(tileCursorFollow.cursorObject.position);
                TileBase tile = tilemap.GetTile(cellPos);
                if (tile == null) return;

                Sprite currentSprite = tilemap.GetSprite(cellPos);

                // Nếu là đất xới -> đổi thành đất xới có nước
                if (currentSprite == tilledSprite)
                {
                    Tile newTile = ScriptableObject.CreateInstance<Tile>();
                    newTile.sprite = wateredSprite;
                    newTile.name = "WateredSoil";
                    tilemap.SetTile(cellPos, newTile);
                    Debug.Log($"💧 Đã tưới đất tại {cellPos}");
                }
            }
        }
    }
}
