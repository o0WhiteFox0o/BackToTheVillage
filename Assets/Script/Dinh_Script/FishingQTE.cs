using System;
using UnityEngine;
using UnityEngine.UI;

public class FishingQTE : MonoBehaviour
{
    [Header("--- UI Components ---")]
    [SerializeField] private GameObject qtePanel;

    [Tooltip("Thanh xoay tròn (kim chỉ)")]
    [SerializeField] private Image timerBar;

    [Tooltip("Vùng màu xanh (vùng thắng)")]
    [SerializeField] private Image successZoneImage;

    [Header("--- Fill Bars ---")]
    [Tooltip("Thanh thời gian tổng (Nằm ngang). Nhớ set Image Type = Filled")]
    [SerializeField] private Image gameTimerImage;

    [Tooltip("Thanh tiến độ câu (Dọc). Nhớ set Image Type = Filled")]
    [SerializeField] private Image progressBarImage;

    [Header("--- Settings ---")]
    [SerializeField] private KeyCode qteKey = KeyCode.F;
    [SerializeField][Range(0.1f, 1.0f)] private float startProgress = 0.3f; // Bắt đầu ở 30%

    [Header("--- Visual Smoothing ---")]
    [SerializeField] private float progressLerpSpeed = 10f; // Tốc độ làm mượt thanh tiến độ
    private float visualProgress; // Biến ảo để hiển thị cho mượt

    // --- Biến Logic ---
    private bool isQTEActive = false;
    private float currentFill;      // Vị trí kim xoay (0-1)
    private float currentProgress;  // Tiến độ thật (0-1)
    private float currentGameTime;

    // --- Số liệu từ FishData ---
    private float qteBarSpeed;
    private float successWindowSize;
    private float maxGameTime;
    private float progressIncrease;
    private float progressDecrease;

    // --- Vùng Xanh ---
    private float successMin;
    private float successMax;

    // --- Events ---
    public event Action OnQTESuccess;
    public event Action OnQTEFailure;

    // --- Audio ---
    [Header("--- Audio ---")]
    [SerializeField] private AudioClip qteBackgroundMusic;
    [SerializeField] private AudioSource qteAudioSource;

    void Start()
    {
        qtePanel.SetActive(false);
    }

    public void StartQTE(FishData fishData, QTEBuff buff)
    {
        // 1. Tính toán chỉ số (Logic giữ nguyên)
        float baseSpeed = fishData.qteBarSpeed;
        this.qteBarSpeed = baseSpeed * (1.0f - buff.barSpeedMod);
        if (this.qteBarSpeed <= 0.01f) this.qteBarSpeed = 0.01f;

        float baseWindow = fishData.successWindowSize;
        this.successWindowSize = baseWindow * (1.0f + buff.successWindowMod);
        if (this.successWindowSize > 1.0f) this.successWindowSize = 1.0f;

        float baseProgress = fishData.progressIncrease;
        this.progressIncrease = baseProgress * (1.0f + buff.progressIncreaseMod);

        this.maxGameTime = fishData.maxGameTime;
        this.progressDecrease = fishData.progressDecrease;

        Debug.Log($"[FishingQTE] Start: Speed={qteBarSpeed}, Zone={successWindowSize}, Inc={progressIncrease}");

        // 2. Reset trạng thái
        currentProgress = startProgress;
        visualProgress = startProgress; // Đặt visual bằng real ngay lập tức để không bị giật lúc đầu
        currentGameTime = maxGameTime;
        currentFill = 0f;

        // 3. Setup hiển thị ban đầu
        if (gameTimerImage != null) gameTimerImage.fillAmount = 1f;
        if (progressBarImage != null) progressBarImage.fillAmount = startProgress;

        RandomizeSuccessZone();

        // 4. Kích hoạt
        isQTEActive = true;
        qtePanel.SetActive(true);

        if (qteAudioSource != null && qteBackgroundMusic != null)
        {
            qteAudioSource.clip = qteBackgroundMusic;
            qteAudioSource.Play();
        }
    }

    void Update()
    {
        if (!isQTEActive) return;

        float dt = Time.deltaTime;

        // 1. Xử lý thời gian tổng (Timer)
        currentGameTime -= dt;

        // Cập nhật UI Timer (Mượt mà nhờ fillAmount)
        if (gameTimerImage != null)
        {
            gameTimerImage.fillAmount = currentGameTime / maxGameTime;
        }

        if (currentGameTime <= 0f)
        {
            FailQTE();
            return;
        }

        // 2. Xử lý kim xoay (Thanh trắng)
        UpdateFillBar(dt);

        // 3. Xử lý Input
        if (Input.GetKeyDown(qteKey))
        {
            HandleHit();
        }

        // 4. Cập nhật Thanh Tiến Độ (Progress Bar) - LÀM MƯỢT
        // Lerp từ giá trị hiển thị cũ -> giá trị thực tế mới
        visualProgress = Mathf.Lerp(visualProgress, currentProgress, dt * progressLerpSpeed);

        if (progressBarImage != null)
        {
            progressBarImage.fillAmount = visualProgress;
        }

        // Cập nhật hình ảnh kim xoay
        if (timerBar != null)
        {
            timerBar.rectTransform.localRotation = Quaternion.Euler(0, 0, -currentFill * 360f);
        }
    }

    private void UpdateFillBar(float dt)
    {
        currentFill += qteBarSpeed * dt;
        if (currentFill >= 1f)
        {
            currentFill -= 1f;
        }
    }

    private void HandleHit()
    {
        // Logic kiểm tra trúng/trượt
        if (currentFill >= successMin && currentFill <= successMax)
        {
            currentProgress += progressIncrease; // Trúng
            RandomizeSuccessZone();
        }
        else
        {
            currentProgress -= progressDecrease; // Trượt
        }

        currentProgress = Mathf.Clamp01(currentProgress);

        // Kiểm tra Thắng/Thua ngay lập tức (Logic Game)
        if (currentProgress >= 1f)
        {
            progressBarImage.fillAmount = 1f; // Fill đầy ngay cho đẹp
            SuccessQTE();
        }
        else if (currentProgress <= 0f)
        {
            progressBarImage.fillAmount = 0f; // Về 0 ngay cho đẹp
            FailQTE();
        }
    }

    private void UpdateSuccessZoneVisuals()
    {
        if (successZoneImage != null)
        {
            successZoneImage.rectTransform.localRotation = Quaternion.Euler(0, 0, -successMin * 360f);
            successZoneImage.fillAmount = successWindowSize;
        }
    }

    private void RandomizeSuccessZone()
    {
        successMin = UnityEngine.Random.Range(0f, 1f - successWindowSize);
        successMax = successMin + successWindowSize;
        UpdateSuccessZoneVisuals();
    }

    private void SuccessQTE()
    {
        Debug.Log("Câu thành công!");
        OnQTESuccess?.Invoke();
        StopQTE();
    }

    private void FailQTE()
    {
        Debug.Log("Thất bại!");
        OnQTEFailure?.Invoke();
        StopQTE();
    }

    private void StopQTE()
    {
        isQTEActive = false;
        qtePanel.SetActive(false);
        if (qteAudioSource != null) qteAudioSource.Stop();
    }

    public bool IsQTEActive() => isQTEActive;
}

// Struct Buff giữ nguyên
public struct QTEBuff
{
    public float barSpeedMod;
    public float successWindowMod;
    public float progressIncreaseMod;
    public static QTEBuff Default => new QTEBuff { barSpeedMod = 0f, successWindowMod = 0f, progressIncreaseMod = 0f };
}