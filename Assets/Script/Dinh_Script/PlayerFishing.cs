using UnityEngine;

public class PlayerFishing : MonoBehaviour
{
    // Kéo QTE_Manager vào đây
    [SerializeField] private FishingQTE fishingQTE;

    // Kéo các file .asset (CaDiec, CaMap) vào đây
    [SerializeField] private FishData[] fishInThisArea;

    // Biến để lưu con cá vừa cắn câu
    private FishData currentBitingFish;

    void Start()
    {
        if (fishingQTE == null)
        {
            Debug.LogError("Chưa gắn FishingQTE vào PlayerFishing!");
            return;
        }

        // ĐĂNG KÝ: "Này QTE, khi nào xong việc thì báo tôi"
        fishingQTE.OnQTESuccess += HandleFishingSuccess;
        fishingQTE.OnQTEFailure += HandleFishingFailure;
    }

    // Đây là hàm logic câu cá của bạn
    // (Ví dụ: khi nhấn phím 'F' ở gần nước)
    private void StartFishingAttempt()
    {
        if (fishInThisArea.Length == 0) return;

        // ... (Chờ một lúc cho cá cắn câu) ...

        // GIẢ LẬP CÁ CẮN CÂU:
        OnFishBite();
    }


    // Hàm này được gọi khi cá cắn câu
    private void OnFishBite()
    {
        // 1. Chọn ngẫu nhiên 1 con cá
        currentBitingFish = fishInThisArea[Random.Range(0, fishInThisArea.Length)];

        Debug.Log($"Một con {currentBitingFish.fishName} đã cắn câu!");

        // 2. Bắt đầu QTE và GỬI ĐỘ KHÓ của con cá đó
        fishingQTE.StartQTE(currentBitingFish);
    }

    // Hàm này tự động chạy khi QTE báo "Thành công"
    private void HandleFishingSuccess()
    {
        Debug.Log($"Bạn đã bắt được: {currentBitingFish.fishName}!");

        // TODO: Thêm 'currentBitingFish' vào túi đồ (Inventory)
        // Ví dụ: Inventory.instance.AddItem(currentBitingFish);
    }

    // Hàm này tự động chạy khi QTE báo "Thất bại"
    private void HandleFishingFailure()
    {
        Debug.Log($"Con {currentBitingFish.fishName} đã trốn thoát!");
    }

    // --- Dùng để TEST ---
    // --- Dùng để TEST ---
    void Update()
    {
        // Nhấn T để giả lập bắt đầu câu
        if (Input.GetKeyDown(KeyCode.T))
        {
            // Chỉ cho phép câu nếu QTE không chạy

            // THAY THẾ DÒNG CŨ:
            // if (!fishingQTE.gameObject.activeInHierarchy)

            // BẰNG DÒNG MỚI NÀY:
            if (!fishingQTE.IsQTEActive())
            {
                StartFishingAttempt();
            }
        }
    }

    // Hủy đăng ký khi không cần nữa
    void OnDestroy()
    {
        if (fishingQTE != null)
        {
            fishingQTE.OnQTESuccess -= HandleFishingSuccess;
            fishingQTE.OnQTEFailure -= HandleFishingFailure;
        }
    }
}