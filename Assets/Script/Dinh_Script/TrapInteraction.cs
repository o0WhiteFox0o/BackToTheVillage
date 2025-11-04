using Management;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrapInteraction : MonoBehaviour
{
    [SerializeField] private float interactRange = 1.5f;
    [SerializeField] private LayerMask interactableLayer;//Đặt Layer cho bẫy
    [SerializeField] private InventoryManager inventory;
    [SerializeField] private TileCursorFollow tileCursorFollow;
    [SerializeField] private Player playerMovement;// dùng để lấy hướng nhìn
    // Start is called before the first frame update
    void Start()
    {
        if (inventory == null) inventory = GetComponent<InventoryManager>();
        if (playerMovement == null) playerMovement = GetComponent<Player>();
    }

    // Update is called once per frame
    void Update()
    {
        // Ấn F để đặt bẫy
        if (Input.GetKeyDown(KeyCode.F) && inventory.holdingItem != null && inventory.holdingItem.itemType == ItemType.Trap)
        {
            PlaceTrap();
        }

        // --- SỬA Ở ĐÂY ---
        // Ấn E hoăc NHẤN Chuột trái (GetMouseButtonDown)
        if (Input.GetKeyDown(KeyCode.E) || Input.GetMouseButtonDown(0))
        // --- KẾT THÚC SỬA ---
        {
            InteractWithTrap();
        }
    }
    // (Trong PlayerInteraction.cs)
    void PlaceTrap()
    {
        ItemScriptableObject trapToPlace = inventory.holdingItem; // Lưu lại item bẫy
        if (trapToPlace == null || trapToPlace.itemPrefab == null) return;

        // (Logic tìm vị trí đặt bẫy)
        Vector2 placementPos = (Vector2)transform.position + playerMovement.lastMovement.normalized * 1f;

        // (Kiểm tra xem vị trí có hợp lệ không, ví dụ: phải là nước)

        // --- SỬA Ở ĐÂY ---
        // Thử xóa 1 Bẫy khỏi túi
        if (inventory.RemoveItem(trapToPlace, 1))
        {
            // Nếu xóa thành công, tạo bẫy ra thế giới game
            Debug.Log("Đặt bẫy tại: " + placementPos);
            Instantiate(trapToPlace.itemPrefab, placementPos, Quaternion.identity);
        }
        else
        {
            // Lỗi (hiếm khi xảy ra)
            Debug.LogError("Không thể xóa bẫy khỏi túi đồ!");
        }
    }
    // --- HÀM ĐÃ HOÀN THIỆN ---
    private void InteractWithTrap()
    {
        // 1. Kiểm tra xem con trỏ có đang hoạt động không
        // (Giả sử tileCursorFollow có hàm/biến để check, nếu không, bỏ qua bước này)
        // if (tileCursorFollow == null || !tileCursorFollow.IsCursorActive()) return;

        // 2. Lấy vị trí của con trỏ
        Vector2 cursorPosition = tileCursorFollow.transform.position;

        // 3. Kiểm tra xem con trỏ có ở trong tầm tương tác của người chơi không
        if (Vector2.Distance(transform.position, cursorPosition) > interactRange)
        {
            Debug.Log("Con trỏ quá xa để tương tác.");
            return;
        }

        // 4. Kiểm tra xem tại vị trí con trỏ có bẫy nào không
        // Dùng OverlapCircle với bán kính nhỏ (0.2f) để dễ trúng hơn OverlapPoint
        Collider2D[] hits = Physics2D.OverlapCircleAll(cursorPosition, 0.2f, interactableLayer);

        if (hits.Length > 0)
        {
            // 5. Lấy component FishTrap từ vật thể đầu tiên tìm thấy
            FishTrap trap = hits[0].GetComponent<FishTrap>();
            if (trap != null)
            {
                // 6. Kiểm tra lại khoảng cách từ người chơi TỚI CÁI BẪY (chứ không phải con trỏ)
                if (Vector2.Distance(transform.position, trap.transform.position) <= interactRange)
                {
                    Debug.Log("Đang tương tác với bẫy: " + trap.name);
                    // Gọi hàm Interact của bẫy và truyền inventory vào
                    trap.Interact(inventory);
                }
                else
                {
                    Debug.Log("Bẫy ở ngoài tầm với.");
                }
            }
        }
        else
        {
            // Debug.Log("Không có gì để tương tác tại vị trí con trỏ.");
        }
    }
    // --- KẾT THÚC ---
}
