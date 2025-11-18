using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    public static DayNight Instance { get; private set; }

    [Header("Time Settings")]
    [Range(0, 24)]
    public float timeOfDay = 6f;             
    public float realMinutesPerPeriod = 6f;
    public int currentDay = 1;

    [Header("Lighting Overlay (2D)")]
    public Image overlay;           // L?p ph? màu 2D
    public Color morningColor = new Color(1f, 0.95f, 0.8f, 0.05f);  // sáng vàng nh?
    public Color afternoonColor = new Color(1f, 0.9f, 0.6f, 0.1f);  // vàng cam tr?a
    public Color nightColor = new Color(0.05f, 0.1f, 0.2f, 0.4f);  // t?i nh?ng không ?en hoàn toàn

    [Header("Events")]
    public UnityEvent onMorning;
    public UnityEvent onAfternoon;
    public UnityEvent onNight;
    public UnityEvent onNewDay;

    private float dayLengthInMinutes;
    private float timeSpeed;
    private string currentPeriod = "";

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
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
        // Sáng: 6h ? 12h
        if (timeOfDay >= 6f && timeOfDay < 12f && currentPeriod != "morning")
        {
            currentPeriod = "morning";
            onMorning?.Invoke();
        }
        // Tr?a/Chi?u: 12h ? 18h
        else if (timeOfDay >= 12f && timeOfDay < 18f && currentPeriod != "afternoon")
        {
            currentPeriod = "afternoon";
            onAfternoon?.Invoke();
        }
        // T?i: 18h ? 6h hôm sau
        else if ((timeOfDay >= 18f || timeOfDay < 6f) && currentPeriod != "night")
        {
            currentPeriod = "night";
            onNight?.Invoke();
        }
    }


    void UpdateOverlayColor()
    {
        // SÁNG: 6h ? 12h (morning ? afternoon)
        if (timeOfDay >= 6f && timeOfDay < 12f)
        {
            float t = (timeOfDay - 6f) / 6f;
            overlay.color = Color.Lerp(morningColor, afternoonColor, t);
        }
        // TR?A/CHI?U: 12h ? 18h (afternoon ? night)
        else if (timeOfDay >= 12f && timeOfDay < 24f)
        {
            float t = (timeOfDay - 13f) / 6f;
            overlay.color = Color.Lerp(afternoonColor, nightColor, t);
        }
        // T?I: 18h ? 6h sáng hôm sau (night ? morning)
        else
        {
            float t;

            // 18h ? 24h
            if (timeOfDay < 6f)
                t = (timeOfDay - 18f) / 12f;
            else
                // 0h ? 6h
                t = (timeOfDay + 6f) / 12f;

            overlay.color = Color.Lerp(nightColor, morningColor, t);
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
