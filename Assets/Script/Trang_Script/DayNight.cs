using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    [Header("Time Settings")]
    [Range(0, 24)]
    public float timeOfDay = 6f;             
    public float realMinutesPerPeriod = 6f;
    public int currentDay = 1;

    [Header("Lighting Overlay (2D)")]
    public Image overlay;           // L?p ph? màu 2D
    public Color morningColor = new Color(1f, 1f, 1f, 0f);   // G?n nh? trong su?t
    public Color afternoonColor = new Color(1f, 0.75f, 0.5f, 0.1f); // Cam nh?t
    public Color nightColor = new Color(0.1f, 0.2f, 0.4f, 0.4f);    // Xanh ??m t?i

    [Header("Events")]
    public UnityEvent onMorning;
    public UnityEvent onAfternoon;
    public UnityEvent onNight;
    public UnityEvent onNewDay;

    private float dayLengthInMinutes;
    private float timeSpeed;
    private string currentPeriod = "";

    void Start()
    {
        dayLengthInMinutes = realMinutesPerPeriod * 3;
        timeSpeed = 24f / (dayLengthInMinutes * 60f);

        // Khi qua ngày m?i thì t?t c? cây s? l?n thêm 1 ngày
        onNewDay.AddListener(GrowAllCrops);
        onNewDay.AddListener(DrySoil);
    }
    void GrowAllCrops()
    {
        CropBehaviour[] crops = FindObjectsOfType<CropBehaviour>();
        foreach (var crop in crops)
        {
            crop.GrowOneDay();
        }
    }

    void Update()
    {
        timeOfDay += Time.deltaTime * timeSpeed;

        if (timeOfDay >= 24f)
        {
            timeOfDay = 6f; // Reset v? sáng hôm sau
            currentDay++;
            currentPeriod = "";
            onNewDay?.Invoke();
        }

        UpdatePeriod();
        UpdateOverlayColor();
    }

    void UpdatePeriod()
    {
        if (timeOfDay >= 6f && timeOfDay < 14f && currentPeriod != "morning")
        {
            currentPeriod = "morning";
            onMorning?.Invoke();
            //Debug.Log("?? Bu?i sáng");
        }
        else if (timeOfDay >= 14f && timeOfDay < 20f && currentPeriod != "afternoon")
        {
            currentPeriod = "afternoon";
            onAfternoon?.Invoke();
            //Debug.Log("?? Bu?i chi?u");
        }
        else if ((timeOfDay >= 20f || timeOfDay < 6f) && currentPeriod != "night")
        {
            currentPeriod = "night";
            onNight?.Invoke();
            //Debug.Log("?? Bu?i t?i");
        }
    }

    void UpdateOverlayColor()
    {
        // Tính ph?n tr?m th?i gian trong ngày (0–1)
        float t = timeOfDay / 24f;

        // Chuy?n màu d?n gi?a các bu?i
        if (timeOfDay >= 6f && timeOfDay < 14f)
            overlay.color = Color.Lerp(afternoonColor, morningColor, (timeOfDay - 6f) / 8f);
        else if (timeOfDay >= 14f && timeOfDay < 20f)
            overlay.color = Color.Lerp(morningColor, afternoonColor, (timeOfDay - 14f) / 6f);
        else
        {
            float nT = (timeOfDay >= 20f) ? (timeOfDay - 20f) / 10f : (timeOfDay + 4f) / 10f;
            overlay.color = Color.Lerp(afternoonColor, nightColor, nT);
        }
    }
    void DrySoil()
    {
        SoilInteraction soil = FindObjectOfType<SoilInteraction>();
        if (soil != null)
        {
            soil.DryAllWateredTiles();
        }
        else
        {
            Debug.LogWarning("? Không tìm th?y SoilInteraction trong scene!");
        }
    }

}
