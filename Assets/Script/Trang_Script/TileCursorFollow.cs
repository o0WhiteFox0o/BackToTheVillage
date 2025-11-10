using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCursorFollow : MonoBehaviour
{
    public Tilemap targetTilemap;
    public Transform cursorObject; // Object con trỏ (phải có SpriteRenderer)

    private SpriteRenderer spriteRenderer; // Tham chiếu đến SpriteRenderer của con trỏ
    [SerializeField] private Color validColor = Color.white;   // Màu khi vị trí hợp lệ
    [SerializeField] private Color invalidColor = Color.red; // Màu khi vị trí không hợp lệ

    private bool isActive = false; // Biến nội bộ để lưu trạng thái bật/tắt

    void Start()
    {
        // --- SỬA HÀM START ---
        if (cursorObject != null)
        {
            // Lấy SpriteRenderer từ cursorObject
            spriteRenderer = cursorObject.GetComponent<SpriteRenderer>();
            if (spriteRenderer == null)
            {
                Debug.LogError("Cursor Object không có SpriteRenderer!", cursorObject);
            }
        }
        else
        {
            Debug.LogError("Chưa gán Cursor Object cho TileCursorFollow!", this.gameObject);
        }

        SetCursorActive(false); // Tắt con trỏ khi bắt đầu game
        // --- KẾT THÚC SỬA ---
    }

    void Update()
    {
        if (cursorObject == null || targetTilemap == null)
            return;

        // --- SỬA KIỂM TRA ---
        // Nếu con trỏ đang tắt (theo biến nội bộ) thì không cập nhật
        if (!isActive)
            return;
        // --- KẾT THÚC SỬA ---

        // Lấy vị trí chuột trên màn hình
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // Chuyển sang tọa độ thế giới
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        // Chuyển sang tọa độ tile
        Vector3Int cellPos = targetTilemap.WorldToCell(mouseWorldPos);

        // Lấy tâm của ô tile
        Vector3 cellCenter = targetTilemap.GetCellCenterWorld(cellPos);

        // Đặt object tại ô đó
        cursorObject.position = cellCenter;
    }

    // Hàm bật/tắt con trỏ
    public void SetCursorActive(bool active)
    {
        isActive = active; // Cập nhật trạng thái nội bộ
        if (cursorObject != null)
        {
            cursorObject.gameObject.SetActive(active);
        }

        // Khi bật con trỏ, luôn reset màu về trạng thái hợp lệ
        if (active)
        {
            SetPlacementValid(true);
        }
    }

    // Hàm để script TrapInteraction kiểm tra xem con trỏ có đang bật không
    public bool IsCursorActive()
    {
        return isActive;
    }

    // Hàm mới để cập nhật màu sắc con trỏ (đỏ hoặc trắng)
    public void SetPlacementValid(bool isValid)
    {
        // Chỉ đổi màu nếu con trỏ đang được bật
        if (!isActive || spriteRenderer == null) return;

        // Đặt màu dựa trên tính hợp lệ
        spriteRenderer.color = isValid ? validColor : invalidColor;
    }
    // --- KẾT THÚC HÀM MỚI ---
}