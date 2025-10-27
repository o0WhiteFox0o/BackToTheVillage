using Management; // Đảm bảo bạn có namespace này cho Inventory
using System.Collections;
using UnityEngine;
using TMPro; // THÊM DÒNG NÀY
using UnityEngine.UI;

public class PlayerFishing : MonoBehaviour
{
    [Header("Components (Các thành phần)")]
    [Tooltip("Kéo script di chuyển của Player (ví dụ: Player) vào đây")]
    [SerializeField] private Player playerMovement;
    [Tooltip("Kéo InventoryManager của bạn vào đây")]
    [SerializeField] private InventoryManager inventory;

    [Header("Hệ thống QTE")]
    [SerializeField] private FishingQTE fishingQTE;

    [Header("Hệ thống Quăng câu")]
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private GameObject castingPanel;
    [SerializeField] private Image castingBar;
    [Tooltip("Tốc độ di chuyển ngang của phao câu (world units/giây)")]
    [SerializeField] private float bobberTravelSpeed = 5f;
    [SerializeField] private float maxCastDistance = 7f;
    [SerializeField] private float minCastDistance = 2f;
    [SerializeField] private float chargeSpeed = 1f;

    [Header("UI Hiển thị Cá Bắt Được")] // Đảm bảo các biến này được khai báo
    [SerializeField] private GameObject caughtFishPanel;
    [SerializeField] private Image fishingIcon;
    [SerializeField] private TMP_Text fishNameText;   
    [SerializeField] private TMP_Text fishWeightText;
    [SerializeField] private TMP_Text fishLengthText;
    [SerializeField] private float caughtFishDisplayTime = 3.0f;

    private Coroutine displayFishCoroutine; // Biến lưu coroutine hiển thị UI

    [Header("Isometric Settings")]
    [SerializeField] private float arcHeight = 1.5f;

    [Header("Thời gian chờ (Ngâm phao)")]
    [SerializeField] private float minWaitTime = 2.0f;
    [SerializeField] private float maxWaitTime = 5.0f;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip successSound; // Thêm âm thanh thành công nếu muốn

    // Biến trạng thái
    private float currentCharge = 0f;
    private bool isCharging = false;
    private GameObject currentBobber;
    private FishData currentBitingFish; // Dùng biến này nhất quán
    private Coroutine waitingForBiteCoroutine;

    // --- THÊM BIẾN TRẠNG THÁI ---
    // (Lấy từ code Lure trước đó, giúp quản lý tốt hơn)
    private enum FishingState { Idle, Charging,Casting, BobberWaiting, FightingFish }
    private FishingState currentState = FishingState.Idle;
    // --- KẾT THÚC THÊM ---

    void Start()
    {
        if (fishingQTE != null)
        {
            fishingQTE.OnQTESuccess += HandleFishingSuccess;
            fishingQTE.OnQTEFailure += HandleFishingFailure;
        }
        castingPanel.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (playerMovement == null)
        {
            playerMovement = GetComponent<Player>();
        }
        if (caughtFishPanel != null) caughtFishPanel.SetActive(false);
        currentState = FishingState.Idle; // Đảm bảo trạng thái ban đầu
    }

    void Update()
    {
        HandleMovementLock();

        // Chỉ cho phép bắt đầu câu khi đang Idle
        if (currentState == FishingState.Idle && Input.GetKeyDown(KeyCode.F))
        {
            currentState = FishingState.Charging;
            isCharging = true; // Dùng cả bool isCharging cho logic gồng lực
            currentCharge = 0f;
            castingPanel.SetActive(true);
        }

        if (isCharging) // Dùng isCharging để xử lý gồng lực
        {
            currentCharge += Time.deltaTime * chargeSpeed;
            castingBar.fillAmount = currentCharge;
            if (currentCharge >= 1f) currentCharge = 1f;

            if (Input.GetKeyUp(KeyCode.F))
            {
                isCharging = false; // Tắt trạng thái gồng
                castingPanel.SetActive(false);
                CastBobber(); // Hàm CastBobber sẽ tự đổi currentState
            }
        }

        // Cho phép hủy khi đang chờ phao
        if (currentState == FishingState.BobberWaiting && Input.GetKeyDown(KeyCode.F))
        {
            CancelFishing();
        }
    }

    private void HandleMovementLock()
    {
        // Khóa di chuyển nếu không phải trạng thái Idle
        bool lockMovement = currentState != FishingState.Idle;
        if (playerMovement != null)
        {
            playerMovement.enabled = !lockMovement;
        }
    }

    // Hàm IsFishing() cũ không cần thiết nếu dùng state machine
    /*
    public bool IsFishing()
    {
        return isCharging || currentBobber != null || (fishingQTE != null && fishingQTE.IsQTEActive());
    }
    */

    private void CastBobber()
    {
        // 1. Lấy vị trí chuột và tính hướng quăng
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(castPoint.position).z;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 finalCastDirection = (mouseWorldPos - (Vector2)castPoint.position).normalized;

        // 2. Tính KHOẢNG CÁCH quăng
        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, currentCharge);

        // 3. Tính ĐIỂM ĐẾN (Destination)
        Vector2 destination = (Vector2)castPoint.position + (finalCastDirection * castDistance);

        // 4. Tính toán THỜI GIAN BAY
        float castDuration = castDistance / (bobberTravelSpeed + 0.01f);

        if (castSound != null && audioSource != null) audioSource.PlayOneShot(castSound);

        // 5. Tạo phao câu
        GameObject bobberGO = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Bobber bobberScript = bobberGO.GetComponent<Bobber>();
        bobberScript.playerFishingScript = this;

        // 6. Bắt đầu di chuyển
        bobberScript.StartCast(destination, arcHeight, castDuration);

        currentBobber = bobberGO;
        currentState = FishingState.Casting;
    }

    public void OnBobberLandedOnGround()
    {
        if (currentBobber == null) return; // Đã xử lý
        Debug.Log("Quăng trúng đất!");
        CleanUpAfterFailure(); // Dọn dẹp như thất bại
    }

    public void OnBobberLanded(FishData pickedFish)
    {
        if (currentBobber == null) return; // Đã xử lý (ví dụ: hủy câu)
        Debug.Log("Phao đã chạm nước. Bắt đầu chờ cá!");
        currentBitingFish = pickedFish; // Lưu con cá đã được chọn
        currentState = FishingState.BobberWaiting; // Đổi trạng thái sang chờ
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = StartCoroutine(WaitForBite());
    }

    private IEnumerator WaitForBite()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log($"Đang ngâm phao, chờ {waitTime} giây...");
        yield return new WaitForSeconds(waitTime);

        // Chỉ bắt đầu câu nếu vẫn đang chờ và phao còn đó
        if (currentState == FishingState.BobberWaiting && currentBobber != null)
        {
            Debug.Log("CÁ CẮN CÂU!");
            StartFishingAttempt();
        }
    }

    private void StartFishingAttempt()
    {
        if (currentBitingFish == null)
        {
            Debug.LogWarning("Không có cá nào trong khu vực này để bắt đầu QTE!");
            CancelFishing();
            return;
        }
        OnBite();
    }

    private void OnBite()
    {
        Debug.Log($"Một con {currentBitingFish.displayName} đã cắn câu!"); // Dùng displayName từ ItemScriptableObject
        currentState = FishingState.FightingFish; // Đổi trạng thái sang chiến đấu
        fishingQTE.StartQTE(currentBitingFish);
    }

    private void CancelFishing()
    {
        Debug.Log("Hủy câu!");
        CleanUpAfterFailure(); // Dọn dẹp như thất bại
    }

    // *** HÀM HANDLE SUCCESS GỌI COROUTINE ***
    private void HandleFishingSuccess()
    {
        // Chỉ xử lý nếu đang trong trạng thái chiến đấu
        if (currentState != FishingState.FightingFish)
        {
            Debug.LogWarning("HandleFishingSuccess called but not in FightingFish state.");
            return;
        }
        if (currentBitingFish == null) // Kiểm tra lại cá
        {
            Debug.LogError("HandleFishingSuccess: currentBitingFish is null!");
            CleanUpAfterSuccess(); // Vẫn dọn dẹp
            return;
        }


        Debug.Log($"Bạn đã bắt được: {currentBitingFish.displayName}!"); // Dùng displayName
        bool addedSuccessfully = false;
        try
        {
            addedSuccessfully = inventory.AddItem(currentBitingFish, 1);
            if (!addedSuccessfully) Debug.LogWarning("Inventory full!");
        }
        catch (System.Exception e)
        {
            Debug.LogError($"Không thể thêm cá vào túi đồ! Lỗi: {e.Message}\nStackTrace: {e.StackTrace}");
        }

        // Chơi âm thanh thành công nếu có và nếu thêm thành công (hoặc luôn luôn?)
        if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }


        // HIỂN THỊ UI CÁ BẮT ĐƯỢC
        if (caughtFishPanel != null)
        {
            if (displayFishCoroutine != null) StopCoroutine(displayFishCoroutine);
            // *** SỬA Ở ĐÂY: Truyền con cá vào coroutine ***
            displayFishCoroutine = StartCoroutine(ShowCaughtFishUI(currentBitingFish));
        }

        CleanUpAfterSuccess(); // Gọi hàm dọn dẹp thành công
    }


    private void HandleFishingFailure()
    {
        // Chỉ xử lý nếu đang trong trạng thái chiến đấu
        if (currentState != FishingState.FightingFish)
        {
            Debug.LogWarning("HandleFishingFailure called but not in FightingFish state.");
            return;
        }
        if (currentBitingFish == null) // Kiểm tra lại cá (ít khả năng xảy ra)
        {
            Debug.LogError("HandleFishingFailure: currentBitingFish is null!");
            CleanUpAfterFailure(); // Vẫn dọn dẹp
            return;
        }

        Debug.Log($"Con {currentBitingFish.displayName} đã trốn thoát!"); // Dùng displayName
        // (Thêm âm thanh thất bại ở đây nếu muốn)
        CleanUpAfterFailure(); // Gọi hàm dọn dẹp thất bại
    }

    // --- COROUTINE HIỂN THỊ UI (PHẢI CÓ HÀM NÀY) ---
    private IEnumerator ShowCaughtFishUI(FishData fish) // Nhận tham số FishData
    {
        if (fish == null) yield break; // Thoát nếu không có cá

        // 1. Tạo cân nặng và độ dài ngẫu nhiên
        float randomWeight = Random.Range(fish.min_weight, fish.max_weight);
        float randomLength = Random.Range(fish.min_length, fish.max_length);

        // 2. Cập nhật Text (kiểm tra null trước)
        if (fishNameText != null) fishNameText.text = fish.displayName;
        if (fishingIcon != null) fishingIcon.sprite = fish.icon;
        if (fishWeightText != null) fishWeightText.text = $"Nặng: {randomWeight:F1} kg";
        if (fishLengthText != null) fishLengthText.text = $"Dài: {randomLength:F1} cm";

        // 3. Hiển thị Panel (kiểm tra null trước)
        if (caughtFishPanel != null) caughtFishPanel.SetActive(true);

        // 4. Chờ vài giây
        yield return new WaitForSeconds(caughtFishDisplayTime);

        // 5. Ẩn Panel (kiểm tra null trước)
        if (caughtFishPanel != null) caughtFishPanel.SetActive(false);
        displayFishCoroutine = null; // Reset biến coroutine
    }
    // --- KẾT THÚC COROUTINE ---


    // --- HÀM DỌN DẸP RIÊNG BIỆT ---
    private void CleanUpAfterSuccess() // Sau khi bắt thành công
    {
        currentState = FishingState.Idle; // Quay về trạng thái chờ
                                          // Hủy phao câu/mồi giả
        if (currentBobber != null) Destroy(currentBobber);
        currentBobber = null;
        currentBitingFish = null; // Reset cá
                                  // Dừng các coroutine
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
        // Không cần dừng displayFishCoroutine ở đây, nó sẽ tự kết thúc
    }


    private void CleanUpAfterFailure() // Sau khi thất bại hoặc hủy
    {
        currentState = FishingState.Idle; // Quay về trạng thái chờ
        // Hủy phao câu/mồi giả
        if (currentBobber != null) Destroy(currentBobber);
        currentBobber = null;
        currentBitingFish = null; // Reset cá
        // Dừng các coroutine
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
        // Dừng coroutine hiển thị UI nếu đang chạy (vì thất bại/hủy)
        if (displayFishCoroutine != null)
        {
            StopCoroutine(displayFishCoroutine);
            if (caughtFishPanel != null) caughtFishPanel.SetActive(false); // Ẩn luôn UI
            displayFishCoroutine = null;
        }
    }
    // --- KẾT THÚC HÀM DỌN DẸP ---


    void OnDestroy()
    {
        if (fishingQTE != null)
        {
            fishingQTE.OnQTESuccess -= HandleFishingSuccess;
            fishingQTE.OnQTEFailure -= HandleFishingFailure;
        }
    }
}