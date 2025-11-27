using UnityEngine;
using UnityEngine.UI;

public class ChoppingMinigame : BaseCookingMinigame
{
    [SerializeField] private Slider slider;
    [SerializeField] private float speedBase = 2f;
    [SerializeField] private RectTransform safeZone; // Vùng xanh

    private float sliderVal;
    private int direction = 1;
    private int chopsNeeded;
    private int currentChops;

    public override void StartMinigame(float difficulty, float timeLimit)
    {
        base.StartMinigame(difficulty, timeLimit);
        currentChops = 0;
        chopsNeeded = Mathf.CeilToInt(3 + difficulty);
        slider.value = 0;
    }

    void Update()
    {
        if (!isPlaying) return;

        // Thanh trượt chạy qua lại
        sliderVal += Time.deltaTime * speedBase * direction;
        if (sliderVal >= 1f || sliderVal <= 0f) direction *= -1;
        slider.value = sliderVal;

        if (Input.GetKeyDown(KeyCode.Space))
        {
            // Logic đơn giản: Check giá trị slider (0.4 - 0.6 là vùng giữa)
            // Bạn có thể làm phức tạp hơn bằng cách so sánh RectTransform
            if (sliderVal > 0.4f && sliderVal < 0.6f) 
            {
                currentChops++;
                Debug.Log($"Chặt! {currentChops}/{chopsNeeded}");
                if (currentChops >= chopsNeeded) EndGame(true);
            }
        }
    }
}