using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using System.Collections.Generic;

public class MinigameController : MonoBehaviour
{
    [Header("UI References")]
    [SerializeField] private GameObject minigamePanel;
    [SerializeField] private TMP_Text instructionText;
    [SerializeField] private Button actionButton;
    [SerializeField] private TMP_Text timerText;

    [Header("Game Components")]
    [SerializeField] private Slider gameSlider;           // AI (Mục tiêu)
    [SerializeField] private Image targetZoneImage;       // Player (Vùng xanh)
    [SerializeField] private RectTransform handleAreaRect; // Khung giới hạn (Kéo object 'Handle Slide Area' vào đây)

    [Header("Rhythm Components")]
    [SerializeField] private Transform rhythmContainer;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Tracking Physics Settings")]
    [SerializeField] private float timingZoneWidth = 0.25f;  // Độ rộng vùng xanh (0.2 = 20% thanh trượt)
    [SerializeField] private float requiredHoldTime = 3.0f;
    [SerializeField] private float gravity = 2.0f;
    [SerializeField] private float pushForce = 4.5f;
    [SerializeField] private float targetMoveSpeed = 1.0f;

    [Header("Calibration (FIX TRÀN)")]
    [Range(0.1f, 1.5f)]
    [SerializeField] private float visualSizeCorrection = 1.0f; // Chỉnh độ to nhỏ hình ảnh
    [Range(1.0f, 2.0f)]
    [SerializeField] private float hitForgiveness = 1.2f;       // Độ dễ tính khi va chạm

    [Header("Colors")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;
    [SerializeField] private Color normalColor = new Color(1f, 1f, 1f, 0.5f);

    [Header("Time Limit")]
    [SerializeField] private float gameTimeLimit = 15f;
    private float timeRemaining;

    // State
    private CookingRecipeSO currentRecipe;
    private System.Action<bool> onCompleteCallback;
    private bool isPlaying = false;

    // Variables
    private float chopProgress;
    private float playerZonePos;    // 0 -> 1
    private float aiHandlePos;      // 0 -> 1
    private float playerVelocity;
    private float noiseOffset;
    private float currentHoldTime;

    // Rhythm
    private List<KeyCode> rhythmSequence = new List<KeyCode>();
    private List<GameObject> rhythmArrowObjs = new List<GameObject>();
    private int rhythmIndex;

    private void Start()
    {
        if (actionButton != null) actionButton.onClick.AddListener(OnInputTriggered);
        if (minigamePanel != null) minigamePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying) return;

        timeRemaining -= Time.deltaTime;

        // UI Timer
        if (currentRecipe.minigameType == MinigameType.Timing)
        {
            float percent = Mathf.Clamp01(currentHoldTime / requiredHoldTime) * 100f;
            timerText.text = $"{percent:0}%";
        }
        else
        {
            timerText.text = timeRemaining.ToString("0.0");
        }

        if (timeRemaining <= 0f) { EndGame(false); return; }

        // Logic
        switch (currentRecipe.minigameType)
        {
            case MinigameType.Chopping:
                chopProgress -= Time.deltaTime * 0.1f;
                chopProgress = Mathf.Clamp01(chopProgress);
                gameSlider.value = chopProgress;
                break;

            case MinigameType.Timing:
                UpdateTrackingGame();
                break;

            case MinigameType.Rhythm:
                HandleRhythmInput();
                break;
        }
    }

    private void OnInputTriggered()
    {
        if (!isPlaying) return;

        switch (currentRecipe.minigameType)
        {
            case MinigameType.Chopping:
                SimulateButtonPressEffect();
                chopProgress += 0.1f;
                if (chopProgress >= 0.99f) EndGame(true);
                break;

            case MinigameType.Timing:
                SimulateButtonPressEffect();
                playerVelocity += pushForce;
                break;
        }
    }

    // ======================================================================
    // LOGIC TIMING / TRACKING (FIXED OVERFLOW)
    // ======================================================================
    private void SetupTiming()
    {
        gameSlider.gameObject.SetActive(true);
        targetZoneImage.gameObject.SetActive(true);

        playerZonePos = 0.5f;
        aiHandlePos = 0.5f;
        playerVelocity = 0f;
        currentHoldTime = 0f;
        noiseOffset = Random.Range(0f, 100f);

        instructionText.text = "Giữ Vùng Xanh trùm lên Handle!";
        targetZoneImage.color = normalColor;

        UpdateGreenZoneVisual(playerZonePos);
    }

    private void UpdateTrackingGame()
    {
        // 1. AI MOVEMENT (Handle)
        float noiseVal = Mathf.PerlinNoise(Time.time * targetMoveSpeed, noiseOffset);

        // Quan trọng: Giới hạn AI chạy thụt vào trong một chút (0.05 -> 0.95)
        // Để tránh AI chạy sát mép quá khiến người chơi khó bắt
        aiHandlePos = Mathf.Lerp(0.05f, 0.95f, noiseVal);
        gameSlider.value = aiHandlePos;

        // 2. PLAYER PHYSICS (Green Zone)
        playerVelocity -= gravity * Time.deltaTime;
        playerZonePos += playerVelocity * Time.deltaTime;

        if (playerZonePos <= 0f) { playerZonePos = 0f; playerVelocity = 0f; }
        else if (playerZonePos >= 1f) { playerZonePos = 1f; playerVelocity = 0f; }

        UpdateGreenZoneVisual(playerZonePos);

        // 3. COLLISION CHECK
        float dist = Mathf.Abs(playerZonePos - aiHandlePos);
        float logicHalfWidth = (timingZoneWidth / 2f);

        if (dist <= logicHalfWidth * hitForgiveness)
        {
            currentHoldTime += Time.deltaTime;
            targetZoneImage.color = successColor;
            if (currentHoldTime >= requiredHoldTime) EndGame(true);
        }
        else
        {
            targetZoneImage.color = normalColor;
        }
    }

    // --- HÀM QUAN TRỌNG NHẤT ĐỂ FIX TRÀN VIỀN ---
    private void UpdateGreenZoneVisual(float pos01)
    {
        if (targetZoneImage == null || handleAreaRect == null) return;

        RectTransform targetRect = targetZoneImage.rectTransform;

        // Reset Anchor về giữa
        targetRect.anchorMin = new Vector2(0.5f, 0.5f);
        targetRect.anchorMax = new Vector2(0.5f, 0.5f);
        targetRect.pivot = new Vector2(0.5f, 0.5f);

        float containerWidth = handleAreaRect.rect.width;

        // Tính chiều rộng thực tế của Vùng Xanh
        float currentZoneWidth = containerWidth * timingZoneWidth * visualSizeCorrection;
        targetRect.sizeDelta = new Vector2(currentZoneWidth, targetRect.sizeDelta.y);

        // --- CÔNG THỨC KẸP BIÊN (PADDING) ---
        // Thay vì cho chạy hết containerWidth, ta trừ đi chiều rộng của chính nó (currentZoneWidth)
        // safeDistance là quãng đường mà TÂM vùng xanh được phép đi để MÉP không lòi ra ngoài
        float safeDistance = containerWidth - currentZoneWidth;

        // Tránh lỗi nếu lỡ chỉnh vùng xanh to hơn khung
        if (safeDistance < 0) safeDistance = 0;

        // Tính vị trí X: (pos - 0.5) * safeDistance
        float posX = (pos01 - 0.5f) * safeDistance;

        targetRect.anchoredPosition = new Vector2(posX, 0);
    }

    // ======================================================================
    // RHYTHM & HELPER
    // ======================================================================
    private void HandleRhythmInput()
    {
        KeyCode pressed = KeyCode.None;
        if (Input.GetKeyDown(KeyCode.UpArrow)) pressed = KeyCode.UpArrow;
        else if (Input.GetKeyDown(KeyCode.DownArrow)) pressed = KeyCode.DownArrow;
        else if (Input.GetKeyDown(KeyCode.LeftArrow)) pressed = KeyCode.LeftArrow;
        else if (Input.GetKeyDown(KeyCode.RightArrow)) pressed = KeyCode.RightArrow;

        if (pressed == KeyCode.None) return;

        if (pressed == rhythmSequence[rhythmIndex])
        {
            rhythmArrowObjs[rhythmIndex].GetComponent<Image>().color = successColor;
            rhythmIndex++;
            if (rhythmIndex >= rhythmSequence.Count) EndGame(true);
        }
        else
        {
            rhythmArrowObjs[rhythmIndex].GetComponent<Image>().color = failColor;
            EndGame(false);
        }
    }

    public void StartMinigame(CookingRecipeSO recipe, System.Action<bool> callback)
    {
        timeRemaining = gameTimeLimit;
        timerText.gameObject.SetActive(true);

        currentRecipe = recipe;
        onCompleteCallback = callback;
        isPlaying = true;

        minigamePanel.SetActive(true);
        actionButton.interactable = true;

        gameSlider.gameObject.SetActive(false);
        targetZoneImage.gameObject.SetActive(false);
        rhythmContainer.gameObject.SetActive(false);
        actionButton.gameObject.SetActive(true);

        switch (recipe.minigameType)
        {
            case MinigameType.Chopping:
                instructionText.text = "Bấm LIÊN TỤC!";
                gameSlider.gameObject.SetActive(true);
                gameSlider.value = 0; chopProgress = 0;
                break;
            case MinigameType.Timing:
                SetupTiming();
                break;
            case MinigameType.Rhythm:
                instructionText.text = "Bấm theo mũi tên!";
                SetupRhythm();
                break;
        }
    }

    public void EndGame(bool success)
    {
        isPlaying = false;
        instructionText.text = success ? "Hoàn thành!" : "Thất bại!";
        actionButton.interactable = false;
        StartCoroutine(CloseDelay(success));
        timerText.gameObject.SetActive(false);
    }

    private IEnumerator CloseDelay(bool success)
    {
        yield return new WaitForSeconds(0.8f);
        minigamePanel.SetActive(false);
        onCompleteCallback?.Invoke(success);
    }

    private void SimulateButtonPressEffect()
    {
        if (actionButton != null) StartCoroutine(AnimateButton());
    }

    private IEnumerator AnimateButton()
    {
        actionButton.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        yield return new WaitForSeconds(0.1f);
        actionButton.transform.localScale = Vector3.one;
    }

    public void ForceStop()
    {
        StopAllCoroutines();
        isPlaying = false;
        minigamePanel.SetActive(false);
        if (onCompleteCallback != null) onCompleteCallback.Invoke(false);
        onCompleteCallback = null;
    }

    // ======================================================================
    // LOGIC SETUP RHYTHM
    // ======================================================================
    private void SetupRhythm()
    {
        rhythmContainer.gameObject.SetActive(true);
        actionButton.gameObject.SetActive(false);
        foreach (Transform c in rhythmContainer) Destroy(c.gameObject);
        rhythmSequence.Clear();
        rhythmArrowObjs.Clear();
        rhythmIndex = 0;

        for (int i = 0; i < 5; i++)
        {
            int r = Random.Range(0, 4);
            KeyCode k = KeyCode.RightArrow;
            float rot = 0;
            switch (r)
            {
                case 0: k = KeyCode.RightArrow; rot = 0; break;
                case 1: k = KeyCode.LeftArrow; rot = 180; break;
                case 2: k = KeyCode.UpArrow; rot = 90; break;
                case 3: k = KeyCode.DownArrow; rot = -90; break;
            }
            rhythmSequence.Add(k);
            GameObject arrow = Instantiate(arrowPrefab, rhythmContainer);
            arrow.GetComponent<RectTransform>().localRotation = Quaternion.Euler(0, 0, rot);
            rhythmArrowObjs.Add(arrow);
        }
    }
}