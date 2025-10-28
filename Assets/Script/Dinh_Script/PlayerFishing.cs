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

    [Header("Cá câu được")]
    [SerializeField] private GameObject fishIconPrefab;
    [SerializeField] private float fishPullDuration = 0.5f;
    private Coroutine pullFishCoroutine;

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

        if (currentState == FishingState.BobberWaiting && Input.GetKeyDown(KeyCode.F))
        {
            CancelFishing();
        }
    }

    // --- HÀM CẬP NHẬT DÂY CÂU - VẼ VÕNG MỀM MẠI ---
    void UpdateFishingLine()
    {
        // Kiểm tra lineRenderer và script phao câu
        if (lineRenderer == null || currentBobber == null || currentBobberScript == null) // Thêm kiểm tra currentBobberScript
        {
            if (lineRenderer != null && lineRenderer.enabled)
                lineRenderer.enabled = false;
            return;
        }

        // Kiểm tra trạng thái
        bool shouldShowLine = (currentState == FishingState.Casting ||
                               currentState == FishingState.BobberWaiting ||
                               currentState == FishingState.FightingFish ||
                               currentState == FishingState.PullingFish); // PullingFish có thể không cần dây nếu phao bị hủy sớm

        Vector3 startPoint = castPoint.position;
        // Lấy vị trí từ SpriteTransform của phao
        Vector3 endPoint = currentBobberScript.SpriteTransform.position;

        // Tắt renderer nếu không cần hoặc quá gần
        lineRenderer.enabled = shouldShowLine && Vector3.Distance(startPoint, endPoint) > 0.1f;
        if (!lineRenderer.enabled) return; // Thoát nếu không cần vẽ

        int pointCount = lineRenderer.positionCount; // Lấy số điểm đã đặt (ví dụ: 21)
        int segments = pointCount - 1; // Số đoạn = số điểm - 1

        // Tính hướng và khoảng cách
        Vector3 dir = endPoint - startPoint;
        float distance = dir.magnitude;
        dir.Normalize();

        // Tạo vector "võng" (kết hợp trọng lực và hướng vuông góc)
        Vector3 sagDirection = Vector3.Cross(dir, Vector3.up).normalized; // Hướng vuông góc
        Vector3 gravityDir = Vector3.down;
        Vector3 finalSagDir = (gravityDir + sagDirection * 0.2f).normalized; // Kết hợp (điều chỉnh 0.2f nếu muốn lệch nhiều/ít)

        // Độ võng tỷ lệ với khoảng cách, có giới hạn
        float sagAmount = Mathf.Min(distance * 0.1f, 1.0f); // Điều chỉnh 0.1f và 1.0f để thay đổi độ võng

        // Hiệu ứng sóng nhỏ (tùy chọn)
        float waveFrequency = 5f; // Tần số sóng
        float waveAmplitude = Mathf.Min(0.05f, distance * 0.01f); // Biên độ sóng
        float timeOffset = Time.time * 3f; // Tốc độ sóng

        // Tính toán các điểm
        for (int i = 0; i <= segments; i++)
        {
            float t = i / (float)segments; // Tỉ lệ dọc theo đường dây (0 đến 1)

            // Vị trí cơ bản trên đường thẳng
            Vector3 basePos = Vector3.Lerp(startPoint, endPoint, t);

            // Thêm độ võng (dùng sin để tạo đường cong mềm)
            float sagFactor = Mathf.Sin(Mathf.PI * t); // = 0 ở đầu/cuối, = 1 ở giữa
            basePos += finalSagDir * sagAmount * sagFactor;

            // Thêm hiệu ứng sóng nhỏ (tùy chọn)
            basePos += Vector3.up * Mathf.Sin(t * waveFrequency + timeOffset) * waveAmplitude * sagFactor; // Sóng mạnh hơn ở giữa

            lineRenderer.SetPosition(i, basePos);
        }
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
    }
    private IEnumerator WaitForBite()
    {
        float waitTime = Random.Range(minWaitTime, maxWaitTime);
        Debug.Log($"Đang ngâm phao, chờ {waitTime} giây...");
        yield return new WaitForSeconds(waitTime);
        if (currentState == FishingState.BobberWaiting && currentBobber != null)
        {
            Debug.Log("CÁ CẮN CÂU!");
            float biteSoundDuration = 0f;
            if (onBaitSound != null && audioSource != null)
            {
                audioSource.PlayOneShot(onBaitSound);
                biteSoundDuration = onBaitSound.length;
            }
            float delayDuration = Mathf.Max(0f,biteSoundDuration);
            Debug.Log($"Chờ thêm {delayDuration} giây trước khi bắt đầu QTE...");
            yield return new WaitForSeconds(delayDuration);
            if (currentState == FishingState.BobberWaiting && currentBobber != null)
            { 
                StartFishingAttempt();
            }
                
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

    // --- HÀM DỌN DẸP ---
    private void TryAddItemAndCleanup(FishData fish)
    {
        bool addedSuccessfully = false;
        try
        {
            if (inventory != null && fish != null)
            {
                addedSuccessfully = inventory.AddItem(fish, 1);
                if (!addedSuccessfully) Debug.LogWarning("Inventory full!");
            }
            else
            {
                Debug.LogError("Inventory or FishData is null during TryAddItemAndCleanup!");
            }
        }
        catch (System.Exception e) { Debug.LogError($"Lỗi thêm cá sau khi kéo: {e.Message}"); }

        // Reset trạng thái về Idle
        currentState = FishingState.Idle;
        currentBitingFish = null; // Đã xử lý xong con cá này
        if (waitingForBiteCoroutine != null) StopCoroutine(waitingForBiteCoroutine);
        waitingForBiteCoroutine = null;
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