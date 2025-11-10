using UnityEngine;
using UnityEngine.Events;
using UnityEngine.UI;

public class DayNight : MonoBehaviour
{
    // === THÊM SINGLETON ===
    public static DayNight Instance { get; private set; }
    // === KẾT THÚC THÊM ===

    [Header("Time Settings")]
    [Range(0, 24)]
    public float timeOfDay = 6f;
    public float realMinutesPerPeriod = 6f;
    public int currentDay = 1;

    [Header("Lighting Overlay (2D)")]
    public Image overlay;
    public Color morningColor = new Color(1f, 1f, 1f, 0f);
    public Color afternoonColor = new Color(1f, 0.75f, 0.5f, 0.1f);
    public Color nightColor = new Color(0.1f, 0.2f, 0.4f, 0.4f);

    [Header("Events")]
    public UnityEvent onMorning;
    public UnityEvent onAfternoon;
    public UnityEvent onNight;
    public UnityEvent onNewDay;

    private float dayLengthInMinutes;
    private float timeSpeed;
    private string currentPeriod = "";

    // === THÊM HÀM AWAKE ===
    void Awake()
    {
        // Khởi tạo Singleton
        if (Instance != null && Instance != this)
        {
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
        }
    }
    // === KẾT THÚC THÊM ===

    void Start()
    {
        dayLengthInMinutes = realMinutesPerPeriod * 3;
        // Sửa lại công thức tính timeSpeed cho đúng với 18 giờ (từ 6h đến 24h)
        timeSpeed = 18f / (dayLengthInMinutes * 60f);

        onNewDay.AddListener(GrowAllCrops);
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
            timeOfDay = 6f; // Reset về sáng hôm sau
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
        }
        else if (timeOfDay >= 14f && timeOfDay < 20f && currentPeriod != "afternoon")
        {
            currentPeriod = "afternoon";
            onAfternoon?.Invoke();
        }
        else if ((timeOfDay >= 20f || timeOfDay < 6f) && currentPeriod != "night")
        {
            // Cập nhật lại điều kiện logic cho ban đêm (vì timeOfDay không bao giờ < 6)
            currentPeriod = "night";
            onNight?.Invoke();
        }
    }

    void UpdateOverlayColor()
    {
        // (Giữ nguyên hàm này)
        if (timeOfDay >= 6f && timeOfDay < 14f)
            overlay.color = Color.Lerp(afternoonColor, morningColor, (timeOfDay - 6f) / 8f);
        else if (timeOfDay >= 14f && timeOfDay < 20f)
            overlay.color = Color.Lerp(morningColor, afternoonColor, (timeOfDay - 14f) / 6f);
        else
        {
            // Sửa lại logic Lerp ban đêm
            float nT = (timeOfDay - 20f) / 4f; // 20h -> 24h (4 tiếng)
            overlay.color = Color.Lerp(afternoonColor, nightColor, nT);
        }
    }
}