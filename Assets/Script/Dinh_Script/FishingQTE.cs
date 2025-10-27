using System; // Cần thiết để dùng 'Action' (events)
using UnityEngine;
using UnityEngine.UI;

public class FishingQTE : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject qtePanel;
    [SerializeField] private Image timerBar;         // Thanh xoay (TimerBar.png)
    [SerializeField] private Image successZoneImage; // Vùng xanh (SuccessZone.png)
    [SerializeField] private Image gameTimerBar;

    // Thanh thời gian tổng
    [Header("Game Timer Mask (Horizontal)")]
    [Tooltip("Kéo object 'Image' CON NẰM TRONG MASK của Timer vào đây")]
    [SerializeField] private RectTransform gameTimerFillRect;
    [Tooltip("Vị trí Y của thanh fill Timer khi 0% (ví dụ: -100)")]
    [SerializeField] private float timerEmptyXPos = -100f;
    [Tooltip("Vị trí Y của thanh fill Timer khi 100% (ví dụ: 0)")]
    [SerializeField] private float timerFullXPos = 0f;
    //Thanh tiến độ
    [Header("Progress Bar Mask (Vertical)")]
    [Tooltip("Kéo object 'Image' CON NẰM TRONG MASK vào đây")]
    [SerializeField] private RectTransform progressBarFillRect;
    [Tooltip("Vị trí Y của thanh fill khi 0% (ví dụ: -100)")]
    [SerializeField] private float barEmptyYPos = -100f;
    [Tooltip("Vị trí Y của thanh fill khi 100% (ví dụ: 0)")]
    [SerializeField] private float barFullYPos = 0f;

    [Header("Settings")]
    [SerializeField] private KeyCode qteKey = KeyCode.F;
    // --- ĐÃ THAY ĐỔI DÒNG NÀY ---
    [SerializeField][Range(0.1f, 1.0f)] private float startProgress = 0.333f; // Bắt đầu ở 1/3
    // --- KẾT THÚC THAY ĐỔI ---

    [Header("Visual Setting")]
    [SerializeField] private float progressLerpSpeed = 5f;
    private float visualProgress; // Tiến độ "ảo" để làm mượt

    // Biến trạng thái
    private bool isQTEActive = false;
    private float currentFill;      // Vị trí thanh trắng (0-1)
    private float currentProgress;  // Tiến độ "thật" (0-1)
    private float currentGameTime;

    // Biến logic (lấy từ FishData)
    private float qteBarSpeed;
    private float successWindowSize;
    private float maxGameTime;
    private float progressIncrease;
    private float progressDecrease;

    // Biến logic cho vùng xanh
    private float successMin;
    private float successMax;

    // Events: Báo cho script khác biết kết quả
    public event Action OnQTESuccess;
    public event Action OnQTEFailure;

    // Audio 
    [Header("Audio")] // Đổi tên Header cho rõ
    [Tooltip("Âm thanh nền chạy trong lúc QTE diễn ra")]
    [SerializeField] private AudioClip qteBackgroundMusic; // Đổi tên biến cho rõ
    [Tooltip("Kéo AudioSource component (gắn trên cùng object này) vào đây")]
    [SerializeField] private AudioSource qteAudioSource;

    void Start()
    {
        qtePanel.SetActive(false); // Ẩn khi bắt đầu
    }

    public void StartQTE(FishData fishData)
    {
        // 1. Tải độ khó từ FishData
        this.qteBarSpeed = fishData.qteBarSpeed;
        this.successWindowSize = fishData.successWindowSize;
        this.maxGameTime = fishData.maxGameTime;
        this.progressIncrease = fishData.progressIncrease;
        this.progressDecrease = fishData.progressDecrease;

        // 2. Reset trạng thái
        currentProgress = startProgress; // Sẽ lấy giá trị 0.333f mới
        visualProgress = startProgress;
        currentGameTime = maxGameTime;
        currentFill = 0f;

        // 3. Random VỊ TRÍ VÙNG XANH (1 LẦN)
        RandomizeSuccessZone();

        // 4. Kích hoạt QTE
        isQTEActive = true;
        qtePanel.SetActive(true);

        // 5. Audio
        if (qteAudioSource != null && qteBackgroundMusic != null)
        {
            qteAudioSource.clip = qteBackgroundMusic;
            qteAudioSource.Play();
        }
    }

    void Update()
    {
        if (!isQTEActive) return;

        // 1. XỬ LÝ THỜI GIAN TỔNG (Đếm ngược)
        currentGameTime -= Time.deltaTime;
        if (currentGameTime <= 0f)
        {
            FailQTE(); // Hết giờ -> Thua
            return;
        }

        // 2. XỬ LÝ THANH TRẮNG (xoay 1 vòng)
        UpdateFillBar();

        // 3. XỬ LÝ INPUT (Nhấn phím)
        if (Input.GetKeyDown(qteKey))
        {
            HandleHit();
        }

        // 4. CẬP NHẬT TẤT CẢ UI

        // Làm mượt tiến độ "ảo" (visual) chạy theo tiến độ "thật" (current)
        visualProgress = Mathf.Lerp(visualProgress, currentProgress, Time.deltaTime * progressLerpSpeed);

        // Cập nhật thanh bar bằng cách di chuyển vị trí Y của nó
        float newY = Mathf.Lerp(barEmptyYPos, barFullYPos, visualProgress);
        progressBarFillRect.anchoredPosition = new Vector2(progressBarFillRect.anchoredPosition.x, newY);

        // Cập nhật thanh Game Timer (dùng Mask)
        float timerPercent = currentGameTime / maxGameTime;
        float newTimerX = Mathf.Lerp(timerEmptyXPos, timerFullXPos, timerPercent);
        gameTimerFillRect.anchoredPosition = new Vector2(newTimerX, gameTimerFillRect.anchoredPosition.y);

        // Cập nhật thanh xoay
        timerBar.rectTransform.localRotation = Quaternion.Euler(0, 0, -currentFill * 360f);
    }

    private void UpdateFillBar()
    {
        // Logic cho thanh trắng chạy 1 vòng tròn 0 -> 1 -> 0 -> 1...
        float moveDelta = qteBarSpeed * Time.deltaTime;
        currentFill += moveDelta;
        if (currentFill >= 1f)
        {
            currentFill -= 1f; // Quay lại 0
        }
    }

    private void HandleHit()
    {
        // Logic "Kéo co"
        if (currentFill >= successMin && currentFill <= successMax)
        {
            // Trúng -> Tăng tiến độ
            currentProgress += progressIncrease;
            // Random vị trí mới
            RandomizeSuccessZone();
        }
        else
        {
            // Hụt -> Giảm tiến độ
            currentProgress -= progressDecrease;
        }

        // Giữ tiến độ trong khoảng 0 - 1
        currentProgress = Mathf.Clamp01(currentProgress);

        // Kiểm tra thắng / thua
        if (currentProgress >= 1f)
        {
            SuccessQTE(); // Thắng
        }
        else if (currentProgress <= 0f) // Logic "Thua khi về 0" của bạn
        {
            FailQTE(); // Thua
        }
    }

    // Hàm này đặt vị trí và kích thước vùng xanh
    private void UpdateSuccessZoneVisuals()
    {
        successZoneImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -successMin * 360f);
        successZoneImage.fillAmount = successWindowSize;
    }

    // Hàm này random vị trí mới
    private void RandomizeSuccessZone()
    {
        successMin = UnityEngine.Random.Range(0f, 1f - successWindowSize);
        successMax = successMin + successWindowSize;
        UpdateSuccessZoneVisuals();
    }

    private void SuccessQTE()
    {
        Debug.Log("Câu thành công!");
        OnQTESuccess?.Invoke(); // Bắn sự kiện "Thành công"
        StopQTE();
        qteAudioSource.Stop();
    }

    // Hàm này để PlayerFishing kiểm tra
    public bool IsQTEActive()
    {
        return isQTEActive;
    }

    private void FailQTE()
    {
        Debug.Log("Cá chạy mất rồi!");
        OnQTEFailure?.Invoke(); // Bắn sự kiện "Thất bại"
        StopQTE();
        qteAudioSource.Stop();
    }

    private void StopQTE()
    {
        isQTEActive = false;
        qtePanel.SetActive(false);
        qteAudioSource.Stop();
    }
}