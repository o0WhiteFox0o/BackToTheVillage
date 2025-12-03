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
   // INPUT DUY NHẤT

    [Header("Slider Components")]
    [SerializeField] private Slider gameSlider;
    [SerializeField] private Image targetZoneImage;
    [SerializeField] private RectTransform handleAreaRect;

    [Header("Rhythm Components")]
    [SerializeField] private Transform rhythmContainer;
    [SerializeField] private GameObject arrowPrefab;

    [Header("Timing Settings")]
    [SerializeField] private float timingZoneWidth = 0.2f;

    [Header("Colors")]
    [SerializeField] private Color successColor = Color.green;
    [SerializeField] private Color failColor = Color.red;

    // Internal State
    private CookingRecipeSO currentRecipe;
    private System.Action<bool> onCompleteCallback;
    private bool isPlaying = false;

    // Chopping
    private float chopProgress;
    private float chopDecayRate = 0.1f;
    private float chopAddAmount = 0.1f;

    // Timing
    private float timingCurrentVal;
    private float timingSpeed = 1.5f;
    private float targetMin, targetMax;
    private bool timingMovingRight = true;

    // Rhythm
    private List<KeyCode> rhythmSequence = new List<KeyCode>();
    private List<GameObject> rhythmArrowObjs = new List<GameObject>();
    private int rhythmIndex;

    private void Start()
    {
        // TẤT CẢ INPUT BUTTON ĐỀU CHẠY VỀ 1 CHỖ
        actionButton.onClick.AddListener(() => OnInputTriggered());
        minigamePanel.SetActive(false);
    }

    private void Update()
    {
        if (!isPlaying) return;

        // ---- LOGIC UPDATE ----
        switch (currentRecipe.minigameType)
        {
            case MinigameType.Chopping:
                chopProgress -= Time.deltaTime * chopDecayRate;
                chopProgress = Mathf.Clamp01(chopProgress);
                gameSlider.value = chopProgress;
                break;

            case MinigameType.Timing:
                if (timingMovingRight)
                {
                    timingCurrentVal += Time.deltaTime * timingSpeed;
                    if (timingCurrentVal >= 1f) timingMovingRight = false;
                }
                else
                {
                    timingCurrentVal -= Time.deltaTime * timingSpeed;
                    if (timingCurrentVal <= 0f) timingMovingRight = true;
                }
                gameSlider.value = timingCurrentVal;
                break;

            case MinigameType.Rhythm:
                HandleRhythmInput();   // Rhythm chỉ dùng arrow keys
                break;
        }
    }

    // ======================================================================  
    // 1 INPUT HANDLER DUY NHẤT  
    // ======================================================================
    private void OnInputTriggered()
    {
        if (!isPlaying) return;

        switch (currentRecipe.minigameType)
        {
            case MinigameType.Chopping:
                SimulateButtonPressEffect();
                chopProgress += chopAddAmount;
                chopProgress = Mathf.Clamp01(chopProgress);
                if (chopProgress >= 0.99f) EndGame(true);
                break;

            case MinigameType.Timing:
                SimulateButtonPressEffect();
                if (timingCurrentVal >= targetMin && timingCurrentVal <= targetMax)
                    EndGame(true);
                else
                    EndGame(false);
                break;

            case MinigameType.Rhythm:
                // Rhythm KHÔNG sử dụng input này
                break;
        }
    }

    // ======================================================================  
    // RHYTHM — dùng arrow key riêng nhưng vẫn là input pipeline duy nhất  
    // ======================================================================
    private void HandleRhythmInput()
    {
        // Chỉ bắt arrow key, tránh Input.anyKeyDown bị loạn
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

            if (rhythmIndex >= rhythmSequence.Count)
            {
                EndGame(true);
            }
        }
        else
        {
            rhythmArrowObjs[rhythmIndex].GetComponent<Image>().color = failColor;
            EndGame(false);
        }
    }

    // ======================================================================
    // INIT GAME
    // ======================================================================
    public void StartMinigame(CookingRecipeSO recipe, System.Action<bool> callback)
    {
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
                SetupChopping();
                break;

            case MinigameType.Timing:
                instructionText.text = "Canh đúng vùng xanh!";
                SetupTiming();
                break;

            case MinigameType.Rhythm:
                instructionText.text = "Bấm theo mũi tên!";
                SetupRhythm();
                break;
        }
    }

    private void SetupChopping()
    {
        gameSlider.gameObject.SetActive(true);
        chopProgress = 0;
        gameSlider.value = 0;
    }

    private void SetupTiming()
    {
        gameSlider.gameObject.SetActive(true);
        targetZoneImage.gameObject.SetActive(true);

        float rangeSize = timingZoneWidth; 

        // tránh tràn ra ngoài
        float maxStart = 1f - rangeSize;
        targetMin = Random.Range(0f, maxStart);
        targetMax = targetMin + rangeSize;

        RectTransform target = targetZoneImage.rectTransform;
        target.anchorMin = target.anchorMax = target.pivot = new Vector2(0.5f, 0.5f);

        float total = handleAreaRect.rect.width;
        float zoneWidth = total * rangeSize;

        // vị trí chuẩn
        float centerOffset = (targetMin + rangeSize * 0.5f) - 0.5f;
        float posX = centerOffset * total;

        target.sizeDelta = new Vector2(zoneWidth, target.sizeDelta.y);
        target.anchoredPosition = new Vector2(posX, 0);

        timingCurrentVal = 0;
        timingMovingRight = true;
    }


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

    // ======================================================================
    private void EndGame(bool success)
    {
        isPlaying = false;
        instructionText.text = success ? "THÀNH CÔNG!" : "THẤT BẠI!";
        actionButton.interactable = false;

        StartCoroutine(CloseDelay(success));
    }

    private IEnumerator CloseDelay(bool success)
    {
        yield return new WaitForSeconds(0.8f);
        minigamePanel.SetActive(false);
        onCompleteCallback?.Invoke(success);
    }

    private void SimulateButtonPressEffect()
    {
        StartCoroutine(AnimateButton());
    }

    private IEnumerator AnimateButton()
    {
        actionButton.transform.localScale = new Vector3(0.9f, 0.9f, 1f);
        yield return new WaitForSeconds(0.1f);
        actionButton.transform.localScale = Vector3.one;
    }

}
