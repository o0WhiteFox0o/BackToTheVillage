using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerFishing : MonoBehaviour
{
    [Header("Components (Các thành phần)")]
    [Tooltip("Kéo script di chuyển của Player (ví dụ: PlayerMovement) vào đây")]
    [SerializeField] private Player playerMovement; // <-- THAY TÊN NÀY NẾU CẦN

    [Tooltip("Kéo SpriteRenderer của Player vào đây để script biết hướng quay mặt")]
    [SerializeField] private SpriteRenderer playerSprite;

    [Header("Hệ thống QTE")]
    [SerializeField] private FishingQTE fishingQTE;
    [SerializeField] private FishData[] fishInThisArea;

    [Header("Hệ thống Quăng câu")]
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private GameObject castingPanel;
    [SerializeField] private Image castingBar;

    [Tooltip("Khoảng cách quăng xa tối đa (world units)")]
    [SerializeField] private float maxCastDistance = 7f;
    [Tooltip("Khoảng cách quăng gần tối thiểu (world units)")]
    [SerializeField] private float minCastDistance = 2f;
    [SerializeField] private float chargeSpeed = 1f;

    [Header("Isometric Settings")]
    [SerializeField] private float arcHeight = 1.5f;
    [SerializeField] private float maxCastDuration = 1.0f;

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
    private FishData currentBitingFish;
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

        // Tự động tìm SpriteRenderer nếu chưa gán
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
            if (playerSprite == null) playerSprite = GetComponentInChildren<SpriteRenderer>();
        }

        // --- MỚI: Tự động tìm script di chuyển nếu chưa gán ---
        if (playerMovement == null)
        {
            playerMovement = GetComponent<Player>(); // <-- THAY TÊN NÀY NẾU CẦN
        }
        // --- KẾT THÚC MỚI ---
    }

    void Update()
    {
        // --- MỚI: LUÔN CHẠY HÀM KHÓA DI CHUYỂN ---
        HandleMovementLock();
        // --- KẾT THÚC MỚI ---

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
                CastBobber(); // Gọi trực tiếp
            }
        }

        // 4. Nhấn F lần nữa để HỦY CÂU
        if (Input.GetKeyDown(KeyCode.F) && !isCharging && currentBobber != null && !fishingQTE.IsQTEActive())
        {
            CancelFishing();
        }
    }

    // --- HÀM MỚI: XỬ LÝ KHÓA DI CHUYỂN ---
    private void HandleMovementLock()
    {
        bool isFishing = IsFishing(); // Kiểm tra trạng thái

        if (playerMovement != null)
        {
            // Nếu đang câu (true) -> tắt di chuyển (enabled = false)
            // Nếu không câu (false) -> bật di chuyển (enabled = true)
            playerMovement.enabled = !isFishing;
        }
    }

    // --- HÀM MỚI: KIỂM TRA TRẠNG THÁI CÂU ---
    public bool IsFishing()
    {
        // Trả về true nếu đang gồng, đang chờ cá, hoặc đang chơi QTE
        return isCharging || currentBobber != null || (fishingQTE != null && fishingQTE.IsQTEActive());
    }
    // --- KẾT THÚC HÀM MỚI ---


    // *** HÀM QUĂNG PHAO ĐÃ CẬP NHẬT HƯỚNG QUĂNG ***
    private void CastBobber()
    {
        // --- MỚI: XÁC ĐỊNH HƯỚNG QUĂNG ---
        Vector2 baseIsometricForward = new Vector2(1, 0.5f).normalized; // Hướng "lên-phải" (mặc định)
        Vector2 finalCastDirection;

        if (playerSprite != null && playerSprite.flipX)
        {
            // Player đang quay BÊN TRÁI
            finalCastDirection = new Vector2(-baseIsometricForward.x, baseIsometricForward.y); // (thành "lên-trái")
        }
        else
        {
            // Player đang quay BÊN PHẢI (mặc định)
            finalCastDirection = baseIsometricForward;
        }
        // --- KẾT THÚC MỚI ---

        // 1. Tính toán KHOẢNG CÁCH
        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, currentCharge);

        // 2. Tính ĐIỂM ĐẾN (Destination)
        Vector2 destination = (Vector2)castPoint.position + (finalCastDirection * castDistance); // <-- Dùng hướng mới

        if (castSound != null && audioSource != null) audioSource.PlayOneShot(castSound);

        // 3. Tạo phao câu
        GameObject bobberGO = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Bobber bobberScript = bobberGO.GetComponent<Bobber>();
        bobberScript.playerFishingScript = this;

        // 4. Tính toán thời gian bay
        float castDuration = Mathf.Lerp(0.1f, maxCastDuration, currentCharge);

        // 5. Bắt đầu di chuyển
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

    // (Hàm này giữ nguyên)
    public void OnBobberLanded()
    {
        Debug.Log("Phao đã chạm nước. Bắt đầu chờ cá!");
        if (waitingForBiteCoroutine != null)
        {
            StopCoroutine(waitingForBiteCoroutine);
        }
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

    // (Các hàm còn lại giữ nguyên...)
    private void StartFishingAttempt()
    {
        if (fishInThisArea.Length == 0)
        {
            Debug.LogWarning("Không có cá nào trong khu vực này!");
            return;
        }
        OnBite();
    }
    private void OnBite()
    {
        currentBitingFish = fishInThisArea[Random.Range(0, fishInThisArea.Length)];
        Debug.Log($"Một con {currentBitingFish.fishName} đã cắn câu!");
        fishingQTE.StartQTE(currentBitingFish);
    }
    private void CancelFishing()
    {
        Debug.Log("Hủy câu!");
        if (waitingForBiteCoroutine != null)
        {
            StopCoroutine(waitingForBiteCoroutine);
            waitingForBiteCoroutine = null;
        }
        if (currentBobber != null)
        {
            Destroy(currentBobber);
            currentBobber = null;
        }
    }
    private void HandleFishingSuccess()
    {
        Debug.Log($"Bạn đã bắt được: {currentBitingFish.fishName}!");
        CleanUpAfterFishing();
    }
    private void HandleFishingFailure()
    {
        Debug.Log($"Con {currentBitingFish.fishName} đã trốn thoát!");
        CleanUpAfterFishing();
    }
    private void CleanUpAfterFishing()
    {
        if (currentBobber != null)
        {
            Destroy(currentBobber);
            currentBobber = null;
        }
        if (waitingForBiteCoroutine != null)
        {
            StopCoroutine(waitingForBiteCoroutine);
            waitingForBiteCoroutine = null;
        }
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