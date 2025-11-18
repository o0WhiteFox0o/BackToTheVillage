using UnityEngine;
using UnityEngine.UI;

public class FireCookingMinigame : MonoBehaviour
{
    public RectTransform fireCursor;
    public RectTransform fireSweetSpot;
    public Button fireButton;
    private float cursorSpeed;
    private float sweetSpotWidth;
    private float direction = 1;
    private bool isPlaying = false;

    public System.Action<bool> onFinish; // 📢 callback báo kết quả

    public enum DishType { Easy, Medium, Hard }

    public void SetupDishDifficulty(DishType difficulty)
    {
        switch (difficulty)
        {
            case DishType.Easy:
                cursorSpeed = 1.5f;
                sweetSpotWidth = 0.3f;
                break;
            case DishType.Medium:
                cursorSpeed = 2.2f;
                sweetSpotWidth = 0.2f;
                break;
            case DishType.Hard:
                cursorSpeed = 3.0f;
                sweetSpotWidth = 0.1f;
                break;
        }

        float parentWidth = fireSweetSpot.parent.GetComponent<RectTransform>().rect.width;
        fireSweetSpot.sizeDelta = new Vector2(parentWidth * sweetSpotWidth, fireSweetSpot.sizeDelta.y);

        fireCursor.anchoredPosition = new Vector2(-200, fireCursor.anchoredPosition.y);
        fireButton.onClick.RemoveAllListeners();
        fireButton.onClick.AddListener(CheckFire);
        isPlaying = true;
    }

    void Update()
    {
        if (!isPlaying) return;

        float move = cursorSpeed * direction * Time.deltaTime * 100f;
        fireCursor.anchoredPosition += new Vector2(move, 0);

        if (fireCursor.anchoredPosition.x >= 200 || fireCursor.anchoredPosition.x <= -200)
            direction *= -1;
    }

    void CheckFire()
    {
        float cursorX = fireCursor.anchoredPosition.x;
        float sweetX = fireSweetSpot.anchoredPosition.x;
        float halfWidth = fireSweetSpot.sizeDelta.x / 2;

        bool success = cursorX >= sweetX - halfWidth && cursorX <= sweetX + halfWidth;
        isPlaying = false;

        Debug.Log(success ? "🔥 Thành công canh lửa!" : "💥 Canh lửa thất bại!");
        onFinish?.Invoke(success);
    }
}
