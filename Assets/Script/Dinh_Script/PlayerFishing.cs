using Management; // Đảm bảo bạn có namespace này cho Inventory
using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class PlayerFishing : MonoBehaviour
{
    private Bobber currentBobberScript;

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

    [Header("Line Transition")] // Thêm Header mới
    [Tooltip("Thời gian để dây câu duỗi thẳng sau khi chạm nước (giây)")]
    [SerializeField] private float lineStraightenDuration = 0.3f; // Thời gian chuyển đổi
    private bool isLineStraightening = false; // Cờ báo đang chuyển đổi
    private float lineStraightenTimer = 0f;   // Bộ đếm thời gian chuyển đổi
    private float lastSagAmount = 0f;         // Lưu độ võng cuối cùng trước khi chuyển đổi

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
    [SerializeField] public GameObject exclaimation;
    [SerializeField] private GameObject caughtFishPanel;
    [SerializeField] private Image fishingIcon;
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text fishWeightText;
    [SerializeField] private TMP_Text fishLengthText;
    [SerializeField] private float caughtFishDisplayTime = 3.0f;

    [Header("Cá câu được")]
    [SerializeField] private GameObject fishIconPrefab;
    [SerializeField] private float fishPullDuration = 0.5f;
    private Coroutine pullFishCoroutine;

    private float reactionTimer = 1.5f;
    private Coroutine reactionTimerCoroutine;
    private bool canReactToBite = false;

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
    [SerializeField] private AudioClip onBaitSound;

    // Biến trạng thái
    private float currentCharge = 0f;
    private bool isCharging = false;
    private GameObject currentBobber;
    private FishData currentBitingFish;
    private Coroutine waitingForBiteCoroutine;
    private enum FishingState { Idle, Charging, Casting, BobberWaiting, FightingFish , PullingFish}
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
        if(fishIconPrefab == null)Debug.LogError("fishIconPrefab chưa được gán trong Inspector!");
        if (lineRenderer != null)
        {
            lineRenderer.positionCount = 20;
            lineRenderer.enabled = false;
        }
        else
        {
            Debug.LogError("Line Renderer chưa được gán trong Inspector!");
        }
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

        if (currentState == FishingState.BobberWaiting && !canReactToBite && Input.GetKeyDown(KeyCode.F))
        {
            CancelFishing();
        }
    }

    // --- HÀM CẬP NHẬT DÂY CÂU - VỚI CHUYỂN ĐỔI MƯỢT ---
    void UpdateFishingLine()
    {
        // Kiểm tra null
        if (lineRenderer == null || currentBobber == null || currentBobberScript == null)
        {
            if (lineRenderer != null && lineRenderer.enabled) lineRenderer.enabled = false;
            return;
        }

        // Kiểm tra trạng thái hiển thị
        bool shouldShowLine = (currentState == FishingState.Casting ||
                               currentState == FishingState.BobberWaiting ||
                               currentState == FishingState.FightingFish ||
                               currentState == FishingState.PullingFish);

        Vector3 startPoint = castPoint.position;
        Vector3 endPoint = currentBobberScript.SpriteTransform.position;

        // Tắt/Bật renderer
        lineRenderer.enabled = shouldShowLine && Vector3.Distance(startPoint, endPoint) > 0.1f;
        if (!lineRenderer.enabled)
        {
            isLineStraightening = false; // Tắt transition nếu không vẽ dây
            return;
        }

        // --- TÍNH TOÁN ĐỘ VÕNG HIỆN TẠI (currentSagAmount) ---
        float currentSagAmount = 0f; // Mặc định là thẳng (cho các state sau Casting)
        float distance = Vector3.Distance(startPoint, endPoint);

        if (currentState == FishingState.Casting)
        {
            // Tính độ võng đầy đủ khi đang bay
            currentSagAmount = Mathf.Min(distance * 0.1f, 1.0f);
            lastSagAmount = currentSagAmount; // Lưu lại độ võng này
            isLineStraightening = false;    // Đảm bảo cờ transition tắt
        }
        else if (isLineStraightening) // Nếu đang trong quá trình chuyển đổi
        {
            lineStraightenTimer += Time.deltaTime;
            float t = Mathf.Clamp01(lineStraightenTimer / lineStraightenDuration);

            // Nội suy độ võng từ giá trị cuối cùng (lastSagAmount) về 0
            currentSagAmount = Mathf.Lerp(lastSagAmount, 0f, t);

            // Kết thúc chuyển đổi
            if (lineStraightenTimer >= lineStraightenDuration)
            {
                isLineStraightening = false;
                currentSagAmount = 0f; // Đảm bảo về 0
            }
        }
        // Else (BobberWaiting, FightingFish, PullingFish và KHÔNG transitioning): currentSagAmount vẫn là 0

        // --- VẼ DÂY DÙNG currentSagAmount VÀ SÓNG ---
        int pointCount = lineRenderer.positionCount;
        int segments = pointCount - 1;
        Vector3[] points = new Vector3[pointCount];
        points[0] = startPoint;
        points[pointCount - 1] = endPoint;

        // Tính toán chung
        Vector3 dir = (endPoint - startPoint).normalized;
        Vector3 sagDirection = Vector3.Cross(dir, Vector3.up).normalized;
        Vector3 gravityDir = Vector3.up;
        Vector3 finalSagDir = (gravityDir + sagDirection * 0.2f).normalized;
        // Hiệu ứng sóng
        float waveFrequency = 5f;
        float waveAmplitude = Mathf.Min(0.03f, distance * 0.01f);
        float timeOffset = Time.time * 3f;

        for (int i = 1; i < segments; i++)
        {
            float t = i / (float)segments;
            Vector3 basePos = Vector3.Lerp(startPoint, endPoint, t);
            float sagFactor = Mathf.Sin(Mathf.PI * t);

            // Áp dụng độ võng hiện tại (có thể đang giảm dần)
            basePos += finalSagDir * currentSagAmount * sagFactor;

            // Luôn áp dụng hiệu ứng sóng
            basePos += Vector3.up * Mathf.Sin(t * waveFrequency + timeOffset) * waveAmplitude * sagFactor;
            points[i] = basePos;
        }

        lineRenderer.SetPositions(points);
    }
    // --- KẾT THÚC SỬA ---

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
        currentBobberScript = bobberScript;
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
        isLineStraightening = true; // Bật cờ chuyển đổi
        lineStraightenTimer = 0f;   // Reset bộ đếm
    }
    private IEnumerator WaitForBite()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log($"Đang ngâm phao, chờ {waitTime} giây...");
        yield return new WaitForSeconds(waitTime);

        // Chỉ xử lý nếu vẫn đang chờ và phao còn đó
        if (currentState == FishingState.BobberWaiting && currentBobber != null)
        {
            Debug.Log("CÁ CẮN CÂU! Chờ người chơi nhấn F...");

            // --- HIỆN DẤU CHẤM THAN & CHƠI ÂM THANH ---
            if (exclaimation != null) exclaimation.SetActive(true);
            if (onBaitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(onBaitSound);
            }
            // --- KẾT THÚC ---

            // --- BẮT ĐẦU COROUTINE CHỜ PHẢN ỨNG ---
            canReactToBite = true; // Cho phép nhấn F
            // Dừng coroutine cũ nếu còn chạy (ít khả năng)
            if (reactionTimerCoroutine != null) StopCoroutine(reactionTimerCoroutine);
            reactionTimerCoroutine = StartCoroutine(ReactionTimerCoroutine());
            // --- KẾT THÚC ---

            // KHÔNG gọi StartFishingAttempt hay delay ở đây nữa
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

    public void HandleFishingSuccess()
    {
        if (currentState != FishingState.FightingFish || currentBitingFish == null) return;
        FishData caughtFish = currentBitingFish;
        
        currentState = FishingState.PullingFish;

        if(pullFishCoroutine != null) StopCoroutine(pullFishCoroutine);
        pullFishCoroutine = StartCoroutine(AnimateReelInAndPullFish(caughtFish, currentBobber));

        Debug.Log($"Bạn đã bắt được: {currentBitingFish.displayName}!");
        if (successSound != null && audioSource != null)
        {
            audioSource.PlayOneShot(successSound);
        }

        if (caughtFishPanel != null)
        {
            if (displayFishCoroutine != null) StopCoroutine(displayFishCoroutine);
            displayFishCoroutine = StartCoroutine(ShowCaughtFishUI(currentBitingFish));
        }
    }

    private void HandleFishingFailure()
    {
        if (currentState != FishingState.FightingFish || currentBitingFish == null) return;

        Debug.Log($"Con {currentBitingFish.displayName} đã trốn thoát!");
        CleanUpAfterFailure();
        // Không cần tắt lineRenderer ở đây, CleanUp đã làm
    }

    private IEnumerator AnimateReelInAndPullFish(FishData fish, GameObject bobberToReel)
    {
        if (bobberToReel == null || fishIconPrefab == null || castPoint == null)
        {
            Debug.LogError("Thiếu đối tượng để thực hiện AnimateReelInAndPullFish.");
            TryAddItemAndCleanup(fish); // Dọn dẹp nếu lỗi
            yield break;
        }

        Vector3 startPos = bobberToReel.transform.position; // Vị trí bắt đầu (của cả phao và cá)
        Vector3 endPos = castPoint.position;           // Vị trí đích

        // --- Tạo icon cá NGAY TỪ ĐẦU ---
        GameObject fishIconGO = Instantiate(fishIconPrefab, startPos, Quaternion.identity);
        SpriteRenderer fishSpriteRenderer = fishIconGO.GetComponent<SpriteRenderer>();
        if (fishSpriteRenderer != null)
        {
            fishSpriteRenderer.sprite = fish.icon;
        }
        else
        {
            Debug.LogWarning("fishIconPrefab không có SpriteRenderer!");
        }
        // --- Kết thúc tạo icon ---

        // Tắt script Bobber để nó không tự di chuyển
        Bobber bobberScript = bobberToReel.GetComponent<Bobber>();
        if (bobberScript != null) bobberScript.enabled = false;

        // Tắt dây câu đi vì phao và cá đang bay về rồi
        if (lineRenderer != null) lineRenderer.enabled = false;


        Debug.Log("Reeling in bobber AND pulling fish icon...");
        float timeElapsed = 0f;
        while (timeElapsed < fishPullDuration) // Dùng toàn bộ thời gian fishPullDuration
        {
            // Kiểm tra xem các object còn tồn tại không
            if (bobberToReel == null || fishIconGO == null) yield break;

            float t = timeElapsed / fishPullDuration;
            // Di chuyển CẢ PHAO và ICON CÁ cùng lúc
            bobberToReel.transform.position = Vector3.Lerp(startPos, endPos, t);
            fishIconGO.transform.position = Vector3.Lerp(startPos, endPos, t); // Bay cùng tốc độ

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // --- Dọn dẹp cuối animation ---
        if (bobberToReel != null) Destroy(bobberToReel); // Hủy phao câu
        currentBobber = null;
        if (fishIconGO != null) Destroy(fishIconGO); // Hủy icon cá
        pullFishCoroutine = null;

        // Thêm cá vào inventory và reset state
        TryAddItemAndCleanup(fish);
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
    private IEnumerator ReactionTimerCoroutine()
    {
        float timer = 0f;
        bool reactedInTime = false;

        // Chờ input F hoặc hết giờ
        while (timer < reactionTimer)
        {
            // Kiểm tra nếu người chơi nhấn F VÀ đang trong trạng thái chờ phản ứng
            if (canReactToBite && Input.GetKeyDown(KeyCode.F))
            {
                Debug.Log("Người chơi nhấn F kịp thời!");
                reactedInTime = true;
                break; // Thoát vòng lặp
            }
            timer += Time.deltaTime;
            yield return null; // Chờ frame tiếp theo
        }

        // Tắt cờ cho phép phản ứng và ẩn dấu chấm than
        canReactToBite = false;
        if (exclaimation != null) exclaimation.SetActive(false);
        reactionTimerCoroutine = null; // Reset biến coroutine

        // Xử lý kết quả
        if (reactedInTime)
        {
            // Nếu nhấn F kịp -> Bắt đầu QTE
            // Kiểm tra lại state và bobber phòng trường hợp bị hủy
            if (currentState == FishingState.BobberWaiting && currentBobber != null)
            {
                StartFishingAttempt();
            }
        }
        else
        {
            // Nếu hết giờ -> Cá chạy thoát
            Debug.Log("Quá chậm! Cá chạy mất rồi.");
            // Kiểm tra lại state và bobber phòng trường hợp bị hủy
            if (currentState == FishingState.BobberWaiting && currentBobber != null)
            {
                CleanUpAfterFailure(); // Gọi hàm dọn dẹp
            }
        }
    }

    // --- HÀM DỌN DẸP ---
    private void TryAddItemAndCleanup(FishData fish)
    {
        // ... (Thêm item) ...
        currentState = FishingState.Idle;
        currentBitingFish = null;
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
        // --- DỪNG COROUTINE PHẢN ỨNG ---
        if (reactionTimerCoroutine != null) StopCoroutine(reactionTimerCoroutine);
        reactionTimerCoroutine = null;
        canReactToBite = false;
        if (exclaimation != null) exclaimation.SetActive(false);
        if (hotBar != null) hotBar.SetActive(true);
    }

    private void CleanUpAfterFailure()
    {
        currentState = FishingState.Idle;
        if (currentBobber != null) Destroy(currentBobber);
        currentBobber = null;
        currentBitingFish = null;
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
        // --- DỪNG COROUTINE PHẢN ỨNG ---
        if (reactionTimerCoroutine != null) StopCoroutine(reactionTimerCoroutine);
        reactionTimerCoroutine = null;
        canReactToBite = false;
        if (exclaimation != null) exclaimation.SetActive(false);
        // --- KẾT THÚC DỪNG ---
        if (displayFishCoroutine != null) { /*...*/ }
        if (lineRenderer != null) lineRenderer.enabled = false;
        if (hotBar != null) hotBar.SetActive(true);
    }

    // Cũng nên thêm vào CancelFishing để đảm bảo
    private void CancelFishing()
    {
        Debug.Log("Hủy câu!");
        // --- DỪNG COROUTINE PHẢN ỨNG ---
        if (reactionTimerCoroutine != null) StopCoroutine(reactionTimerCoroutine);
        reactionTimerCoroutine = null;
        canReactToBite = false;
        if (exclaimation != null) exclaimation.SetActive(false);
        // --- KẾT THÚC DỪNG ---
        CleanUpAfterFailure();
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