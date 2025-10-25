using UnityEngine;
using UnityEngine.UI;
using System.Collections;
using Management; // Đảm bảo bạn có namespace này cho Inventory

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

    [SerializeField] private float maxCastDistance = 7f;
    [SerializeField] private float minCastDistance = 2f;
    [SerializeField] private float chargeSpeed = 1f;

    // --- ĐÃ THÊM LẠI: TỐC ĐỘ BAY CỦA PHAO ---
    [Tooltip("Tốc độ di chuyển ngang của phao câu (world units/giây)")]
    [SerializeField] private float bobberTravelSpeed = 5f;
    // --- KẾT THÚC THÊM ---

    [Header("Isometric Settings")]
    [SerializeField] private float arcHeight = 1.5f;
    // [SerializeField] private float maxCastDuration = 1.0f; // <-- ĐÃ XÓA, KHÔNG CẦN NỮA

    [Header("Thời gian chờ (Ngâm phao)")]
    [SerializeField] private float minWaitTime = 2.0f;
    [SerializeField] private float maxWaitTime = 5.0f;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip castSound;

    // Biến trạng thái
    private float currentCharge = 0f;
    private bool isCharging = false;
    private GameObject currentBobber;
    private FishData currentBitingFish; // Chỉ cần biến này (cho logic FishingZone)
    private Coroutine waitingForBiteCoroutine;

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
    }

    void Update()
    {
        HandleMovementLock();

        // 1. Nhấn GIỮ phím F
        if (Input.GetKeyDown(KeyCode.F) && !isCharging && currentBobber == null && !fishingQTE.IsQTEActive())
        {
            isCharging = true;
            currentCharge = 0f;
            castingPanel.SetActive(true);
        }

        // 2. Khi đang gồng lực
        if (isCharging)
        {
            currentCharge += Time.deltaTime * chargeSpeed;
            castingBar.fillAmount = currentCharge;
            if (currentCharge >= 1f) currentCharge = 1f;

            // 3. Khi THẢ phím F
            if (Input.GetKeyUp(KeyCode.F))
            {
                isCharging = false;
                castingPanel.SetActive(false);
                CastBobber();
            }
        }

        // 4. Nhấn F lần nữa để HỦY CÂU
        if (Input.GetKeyDown(KeyCode.F) && !isCharging && currentBobber != null && !fishingQTE.IsQTEActive())
        {
            CancelFishing();
        }
    }

    private void HandleMovementLock()
    {
        bool isFishing = IsFishing();
        if (playerMovement != null)
        {
            playerMovement.enabled = !isFishing;
        }
    }

    public bool IsFishing()
    {
        return isCharging || currentBobber != null || (fishingQTE != null && fishingQTE.IsQTEActive());
    }

    // *** HÀM QUĂNG PHAO ĐÃ CẬP NHẬT CÁCH TÍNH THỜI GIAN BAY ***
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

        // --- ĐÃ SỬA LẠI LOGIC TÍNH THỜI GIAN BAY ---
        // 4. Tính toán THỜI GIAN BAY dựa trên KHOẢNG CÁCH và TỐC ĐỘ
        // duration = distance / speed
        float castDuration = castDistance / (bobberTravelSpeed + 0.01f); // Thêm 0.01f để tránh chia cho 0
        // --- KẾT THÚC SỬA ---

        if (castSound != null && audioSource != null) audioSource.PlayOneShot(castSound);

        // 5. Tạo phao câu
        GameObject bobberGO = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Bobber bobberScript = bobberGO.GetComponent<Bobber>();
        bobberScript.playerFishingScript = this;

        // 6. Bắt đầu di chuyển (với thời gian bay mới)
        bobberScript.StartCast(destination, arcHeight, castDuration);

        currentBobber = bobberGO;
    }

    // (Hàm này giữ nguyên)
    public void OnBobberLandedOnGround()
    {
        if (currentBobber != null)
        {
            Destroy(currentBobber);
            currentBobber = null;
        }
    }

    // (Hàm này khớp với logic FishingZone)
    public void OnBobberLanded(FishData pickedFish)
    {
        Debug.Log("Phao đã chạm nước. Bắt đầu chờ cá!");
        currentBitingFish = pickedFish; // Lưu con cá đã được chọn
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = StartCoroutine(WaitForBite());
    }

    // (Hàm này giữ nguyên)
    private IEnumerator WaitForBite()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log($"Đang ngâm phao, chờ {waitTime} giây...");
        yield return new WaitForSeconds(waitTime);
        if (currentBobber != null)
        {
            Debug.Log("CÁ CẮN CÂU!");
            StartFishingAttempt();
        }
    }

    // (Hàm này khớp với logic FishingZone)
    private void StartFishingAttempt()
    {
        if (currentBitingFish == null)
        {
            Debug.LogWarning("Không có cá nào trong khu vực này!");
            CancelFishing();
            return;
        }
        OnBite();
    }

    // (Hàm này khớp với logic FishingZone)
    private void OnBite()
    {
        Debug.Log($"Một con {currentBitingFish.fishName} đã cắn câu!");
        fishingQTE.StartQTE(currentBitingFish);
    }

    // (Các hàm còn lại giữ nguyên...)
    private void CancelFishing()
    {
        Debug.Log("Hủy câu!");
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        if (currentBobber != null) Destroy(currentBobber);
        currentBobber = null;
        waitingForBiteCoroutine = null;
    }
    private void HandleFishingSuccess()
    {
        Debug.Log($"Bạn đã bắt được: {currentBitingFish.fishName}!");
        try
        {
            inventory.AddItem(currentBitingFish);
        }
        catch (System.Exception e)
        {
            Debug.LogWarning($"Không thể thêm cá vào túi đồ! Lỗi: {e.Message}");
        }
        CleanUpAfterFishing();
    }


    private void HandleFishingFailure()
    {
        Debug.Log($"Con {currentBitingFish.fishName} đã trốn thoát!");
        CleanUpAfterFishing();
    }


    private void CleanUpAfterFishing()
    {
        if (currentBobber != null) Destroy(currentBobber);
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        currentBobber = null;
        waitingForBiteCoroutine = null;
    }

    
    void OnDestroy()
    {
        if (fishingQTE != null)
        {
            fishingQTE.OnQTESuccess -= HandleFishingSuccess;
            fishingQTE.OnQTEFailure -= HandleFishingFailure;
        }
    }
}