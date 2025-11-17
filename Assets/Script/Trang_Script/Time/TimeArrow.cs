using UnityEngine;
using UnityEngine.UI;

public class TimeArrow : MonoBehaviour
{
    public RectTransform arrow;
    public Text timeText;   // Text UI hi?n th? gi?

    void Update()
    {
        if (DayNight.Instance == null) return;

        float time = DayNight.Instance.timeOfDay;
        float angle = 0f;

        // ---- QUAY M?I TÊN ----
        if (time >= 6f && time < 12f)
        {
            float t = (time - 6f) / 6f;
            angle = Mathf.Lerp(-90f, -40f, t);
        }
        else if (time >= 12f && time < 18f)
        {
            float t = (time - 12f) / 6f;
            angle = Mathf.Lerp(-39f, 40f, t);
        }
        else
        {
            float t = (time >= 18f) ? (time - 18f) / 12f : (time + 6f) / 12f;
            angle = Mathf.Lerp(41f, 90f, t);
        }

        arrow.localRotation = Quaternion.Euler(0, 0, -angle);

        // ---- HI?N TH? GI? ----
        UpdateClock(time);
    }

    void UpdateClock(float time)
    {
        int hour = Mathf.FloorToInt(time);          // L?y gi?
        int minute = Mathf.FloorToInt((time - hour) * 60f); // Tính phút

        // Format ki?u 06 : 12
        timeText.text = $"{hour:00} : {minute:00}";
    }
}
