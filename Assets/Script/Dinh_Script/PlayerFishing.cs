using Management; // Đảm bảo bạn có namespace này cho Inventory
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerFishing : MonoBehaviour
{
    [Header("Components (Các thành phần)")]
    [Tooltip("Kéo script di chuyển của Player (ví dụ: Player) vào đây")]
    [SerializeField] private Player playerMovement;
    [Tooltip("Kéo InventoryManager của bạn vào đây")]
    [SerializeField] private InventoryManager inventory;
    [Tooltip("Kéo Line Renderer component (trên Player) vào đây")]
    [SerializeField] private LineRenderer lineRenderer;
    private Vector2 lineStartPoint;
    private Vector2 lineEndPoint;
    private float lineCurrentArcHeight;
    private float lineTotalDuration;
    private float lineCastStartTime;

    [Header("Hệ thống QTE")]
    [SerializeField] private FishingQTE fishingQTE;

    [Header("Hệ thống Quăng câu")]
    [SerializeField] private GameObject bobberPrefab;
    [SerializeField] private Transform castPoint;
    [SerializeField] private GameObject castingPanel;
    [SerializeField] private Image castingBar;
    [SerializeField] private GameObject hotBar;
    [Tooltip("Tốc độ di chuyển ngang của phao câu (world units/giây)")]
    [SerializeField] private float bobberTravelSpeed = 5f;
    [SerializeField] private float maxCastDistance = 7f;
    [SerializeField] private float minCastDistance = 2f;
    [SerializeField] private float chargeSpeed = 1f;

    [Header("UI Hiển thị Cá Bắt Được")]
    [SerializeField] private GameObject caughtFishPanel;
    [SerializeField] private Image fishingIcon;
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text fishWeightText;
    [SerializeField] private TMP_Text fishLengthText;
    [SerializeField] private float caughtFishDisplayTime = 3.0f;
    private Coroutine displayFishCoroutine;

    [Header("Isometric Settings")]
    [SerializeField] private float arcHeight = 1.5f;

    [Header("Thời gian chờ (Ngâm phao)")]
    [SerializeField] private float minWaitTime = 2.0f;
    [SerializeField] private float maxWaitTime = 5.0f;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip castSound;
    [SerializeField] private AudioClip successSound;

    // Biến trạng thái
    private float currentCharge = 0f;
    private bool isCharging = false;
    private GameObject currentBobber;
    private FishData currentBitingFish;
    private Coroutine waitingForBiteCoroutine;
    private enum FishingState { Idle, Charging, Casting, BobberWaiting, FightingFish }
    private FishingState currentState = FishingState.Idle;

    void Start()
    {
        if (fishingQTE != null)
        {
            fishingQTE.OnQTESuccess += HandleFishingSuccess;
            fishingQTE.OnQTEFailure += HandleFishingFailure;
        }
        castingPanel.SetActive(false);
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        if (playerMovement == null) playerMovement = GetComponent<Player>();
        if (caughtFishPanel != null) caughtFishPanel.SetActive(false);
        currentState = FishingState.Idle;

        // --- SỬA: Kiểm tra LineRenderer đã gán ---
        // lineRenderer = GetComponent<LineRenderer>(); // Không cần tìm nữa nếu dùng SerializeField
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 2; // Đảm bảo có 2 điểm
            lineRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("Line Renderer chưa được gán trong Inspector!");
        }
        // --- KẾT THÚC SỬA ---
    }

    void Update()
    {
        HandleMovementLock();
        HandleFishingInput(); // Tách riêng logic input
        UpdateFishingLine(); // Cập nhật dây câu
    }

    void HandleFishingInput()
    {
        if (currentState == FishingState.Idle && Input.GetKeyDown(KeyCode.F))
        {
            currentState = FishingState.Charging;
            isCharging = true;
            currentCharge = 0f;
            castingPanel.SetActive(true);
        }

        if (isCharging)
        {
            currentCharge += Time.deltaTime * chargeSpeed;
            castingBar.fillAmount = currentCharge;
            if (currentCharge >= 1f) currentCharge = 1f;

            if (Input.GetKeyUp(KeyCode.F))
            {
                isCharging = false;
                castingPanel.SetActive(false);
                CastBobber();
            }
        }

        if (currentState == FishingState.BobberWaiting && Input.GetKeyDown(KeyCode.F))
        {
            CancelFishing();
        }
    }

    // --- HÀM CẬP NHẬT DÂY CÂU ĐÃ SỬA ĐỂ VẼ CONG ---
    void UpdateFishingLine()
    {
        if (lineRenderer == null) return;

        // Chỉ vẽ dây khi đang bay (Casting), chờ (BobberWaiting), hoặc câu (FightingFish)
        bool shouldShowLine = (currentState == FishingState.Casting ||
                               currentState == FishingState.BobberWaiting ||
                               currentState == FishingState.FightingFish)
                              && currentBobber != null;

        lineRenderer.enabled = shouldShowLine;

        if (shouldShowLine)
        {
            int pointCount = lineRenderer.positionCount; // Lấy số điểm đã đặt (ví dụ: 20)
            Vector3[] points = new Vector3[pointCount];
            points[0] = castPoint.position; // Điểm đầu luôn là cần câu

            // Tính toán thời gian đã trôi qua và tỉ lệ hoàn thành hiện tại
            float timeElapsed = Time.time - lineCastStartTime;
            float currentT = Mathf.Clamp01(timeElapsed / lineTotalDuration); // Tỉ lệ từ 0 đến 1

            // Tính các điểm trung gian dọc theo quỹ đạo cong *tại thời điểm hiện tại*
            for (int i = 1; i < pointCount; i++)
            {
                // Tính tỉ lệ t cho điểm này (chỉ vẽ đường cong đến vị trí hiện tại của phao)
                float tSegment = ((float)i / (pointCount - 1)) * currentT; // Chia đều từ 0 đến currentT

                // Tính vị trí "trên mặt đất" của điểm này
                Vector2 groundPos = Vector2.Lerp(lineStartPoint, lineEndPoint, tSegment);

                // Tính độ cao Y offset bằng công thức Parabol
                // Lưu ý: Dùng lineCurrentArcHeight đã lưu để đảm bảo độ cao đúng
                float yOffset = -4 * lineCurrentArcHeight * (Mathf.Pow(tSegment, 2) - tSegment);

                // Kết hợp vị trí mặt đất và độ cao Y
                // Dùng Z của castPoint để đảm bảo dây không bị lệch Z
                points[i] = new Vector3(groundPos.x, groundPos.y + yOffset, castPoint.position.z);
            }

            // Gán mảng điểm cho LineRenderer
            lineRenderer.SetPositions(points);
        }
    }


    private void HandleMovementLock()
    {
        bool lockMovement = currentState != FishingState.Idle;
        if (playerMovement != null)
        {
            playerMovement.enabled = !lockMovement;
        }
    }

    private void CastBobber()
    {
        // ... (Code tính toán destination, duration giữ nguyên) ...
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Camera.main.WorldToScreenPoint(castPoint.position).z;
        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        Vector2 finalCastDirection = (mouseWorldPos - (Vector2)castPoint.position).normalized;
        float castDistance = Mathf.Lerp(minCastDistance, maxCastDistance, currentCharge);
        Vector2 destination = (Vector2)castPoint.position + (finalCastDirection * castDistance);
        float castDuration = castDistance / (bobberTravelSpeed + 0.01f);

        if (castSound != null && audioSource != null) audioSource.PlayOneShot(castSound);

        GameObject bobberGO = Instantiate(bobberPrefab, castPoint.position, Quaternion.identity);
        Bobber bobberScript = bobberGO.GetComponent<Bobber>();
        bobberScript.playerFishingScript = this;
        bobberScript.StartCast(destination, arcHeight, castDuration);

        // quỹ đạo dây câu
        lineStartPoint = castPoint.position;
        lineEndPoint = destination;
        lineCurrentArcHeight = arcHeight;
        lineTotalDuration = castDuration;
        lineCastStartTime = Time.time;

        currentBobber = bobberGO;
        currentState = FishingState.Casting; // Chuyển sang trạng thái đang bay

        // Không cần bật LineRenderer ở đây, UpdateFishingLine sẽ xử lý
        // if(lineRenderer != null) { ... }
    }

    // ... (OnBobberLandedOnGround, OnBobberLanded, WaitForBite, StartFishingAttempt, OnBite giữ nguyên) ...
    public void OnBobberLandedOnGround()
    {
        if (currentBobber == null) return;
        Debug.Log("Quăng trúng đất!");
        CleanUpAfterFailure();
    }
    public void OnBobberLanded(FishData pickedFish)
    {
        if (currentBobber == null) return;
        Debug.Log("Phao đã chạm nước. Bắt đầu chờ cá!");
        currentBitingFish = pickedFish;
        currentState = FishingState.BobberWaiting;
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = StartCoroutine(WaitForBite());
    }
    private IEnumerator WaitForBite()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log($"Đang ngâm phao, chờ {waitTime} giây...");
        yield return new WaitForSeconds(waitTime);
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
        Debug.Log($"Một con {currentBitingFish.displayName} đã cắn câu!");
        currentState = FishingState.FightingFish;
        fishingQTE.StartQTE(currentBitingFish);
    }

    private void CancelFishing()
    {
        Debug.Log("Hủy câu!");
        CleanUpAfterFailure();
        // Không cần tắt lineRenderer ở đây, CleanUp đã làm
    }

    private void HandleFishingSuccess()
    {
        if (currentState != FishingState.FightingFish || currentBitingFish == null) return;

        Debug.Log($"Bạn đã bắt được: {currentBitingFish.displayName}!");
        bool addedSuccessfully = false;
        try
        {
            addedSuccessfully = inventory.AddItem(currentBitingFish, 1);
            if (!addedSuccessfully) Debug.LogWarning("Inventory full!");
        }
        catch (System.Exception e) { Debug.LogError($"Không thể thêm cá vào túi đồ! Lỗi: {e.Message}"); }

        if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        if (caughtFishPanel != null)
        {
            if (displayFishCoroutine != null) StopCoroutine(displayFishCoroutine);
            displayFishCoroutine = StartCoroutine(ShowCaughtFishUI(currentBitingFish));
        }

        CleanUpAfterSuccess();
        // Không cần tắt lineRenderer ở đây, CleanUp đã làm
    }

    private void HandleFishingFailure()
    {
        if (currentState != FishingState.FightingFish || currentBitingFish == null) return;

        Debug.Log($"Con {currentBitingFish.displayName} đã trốn thoát!");
        CleanUpAfterFailure();
        // Không cần tắt lineRenderer ở đây, CleanUp đã làm
    }

    private IEnumerator ShowCaughtFishUI(FishData fish)
    {
        if (fish == null) yield break;

        // *** KIỂM TRA LẠI TÊN BIẾN TRONG FISHDATA CỦA BẠN ***
        float randomWeight = Random.Range(fish.min_weight, fish.max_weight); // Giả sử là minWeight
        float randomLength = Random.Range(fish.min_length, fish.max_length); // Giả sử là minLength
        // *** KẾT THÚC KIỂM TRA ***

        if (fishNameText != null) fishNameText.text = fish.displayName;
        if (fishingIcon != null) fishingIcon.sprite = fish.icon; // Cập nhật Icon
        if (fishWeightText != null) fishWeightText.text = $"Nặng: {randomWeight:F1} kg";
        if (fishLengthText != null) fishLengthText.text = $"Dài: {randomLength:F1} cm";
        if (caughtFishPanel != null) caughtFishPanel.SetActive(true);
        yield return new WaitForSeconds(caughtFishDisplayTime);
        if (caughtFishPanel != null) caughtFishPanel.SetActive(false);
        displayFishCoroutine = null;
    }


    // --- HÀM DỌN DẸP ĐÃ SỬA ---
    private void CleanUpAfterSuccess()
    {
        currentState = FishingState.Idle;
        if (currentBobber != null) Destroy(currentBobber);
        currentBobber = null;
        currentBitingFish = null;
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
        // Tắt dây câu
        if (lineRenderer != null) lineRenderer.enabled = false;
        hotBar.SetActive(true);
    }

    private void CleanUpAfterFailure()
    {
        currentState = FishingState.Idle;
        if (currentBobber != null) Destroy(currentBobber);
        currentBobber = null;
        currentBitingFish = null;
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
        if (displayFishCoroutine != null)
        {
            StopCoroutine(displayFishCoroutine);
            if (caughtFishPanel != null) caughtFishPanel.SetActive(false);
            displayFishCoroutine = null;
        }
        // Tắt dây câu
        if (lineRenderer != null) lineRenderer.enabled = false;
        hotBar.SetActive(true);
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