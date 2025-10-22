using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class PlayerFishing : MonoBehaviour
{
    [Header("Components (Các thành phần)")]
    [SerializeField] private Player playerMovement;
    [SerializeField] private SpriteRenderer playerSprite;

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
    private FishData currentBitingFish; // <-- Chỉ cần biến này
    // private FishData[] currentFishList; // <-- ĐÃ XÓA
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
        if (playerSprite == null)
        {
            playerSprite = GetComponent<SpriteRenderer>();
            if (playerSprite == null) playerSprite = GetComponentInChildren<SpriteRenderer>();
        }
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

    private void CastBobber()
    {
        Vector2 baseIsometricForward = new Vector2(1, 0.5f).normalized;
        Vector2 finalCastDirection;

        if (playerSprite != null && playerSprite.flipX)
        {
            finalCastDirection = new Vector2(-baseIsometricForward.x, baseIsometricForward.y);
        }
        else
        {
            finalCastDirection = baseIsometricForward;
        }

        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, currentCharge);
        Vector2 destination = (Vector2)castPoint.position + (finalCastDirection * castDistance);

        if (castSound != null && audioSource != null) audioSource.PlayOneShot(castSound);

        GameObject bobberGO = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Bobber bobberScript = bobberGO.GetComponent<Bobber>();
        bobberScript.playerFishingScript = this;

        float castDuration = Mathf.Lerp(0.1f, maxCastDuration, currentCharge);

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

    // --- SỬA HÀM NÀY: Giờ nó nhận vào 1 con cá (FishData) ---
    public void OnBobberLanded(FishData pickedFish)
    {
        Debug.Log("Phao đã chạm nước. Bắt đầu chờ cá!");

        // --- MỚI: Lưu lại con cá đã được chọn ---
        currentBitingFish = pickedFish;
        // --- KẾT THÚC MỚI ---

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

    // --- SỬA HÀM NÀY: Dùng con cá đã lưu ---
    private void StartFishingAttempt()
    {
        // Kiểm tra xem Bobber có gửi cho chúng ta con cá nào không
        if (currentBitingFish == null)
        {
            Debug.LogWarning("Không có cá nào trong khu vực này!");
            CancelFishing(); // Tự động hủy câu nếu không có cá
            return;
        }
        OnBite();
    }

    // --- SỬA HÀM NÀY: Dùng con cá đã lưu ---
    private void OnBite()
    {
        // Không cần Random ở đây nữa! Cá đã được chọn
        Debug.Log($"Một con {currentBitingFish.fishName} đã cắn câu!");
        fishingQTE.StartQTE(currentBitingFish);
    }

    // (Các hàm còn lại giữ nguyên...)
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