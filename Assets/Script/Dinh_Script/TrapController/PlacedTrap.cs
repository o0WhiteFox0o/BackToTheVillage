using System.Collections;
using UnityEngine;
using Management;

public class PlacedTrap : MonoBehaviour
{
    public enum TrapState
    {
        Empty,
        Baited,
        ReadyToCollect
    }

    [Header("Trạng thái")]
    [SerializeField] private TrapState currentState = TrapState.Empty;
    public TrapState CurrentState { get { return currentState; } }


    [Header("Cấu hình")]
    [Tooltip("Vật phẩm bẫy (chính nó) để trả về inventory khi nhặt")]
    public ItemScriptableObject trapItemSO;

    [Tooltip("Thời gian (GIỜ TRONG GAME) CƠ BẢN để bắt được cá")]
    public float baseTimeToCatch = 8f;


    [Header("Hiển thị Icon Cá")]
    [Tooltip("Sprite Renderer dùng để hiển thị icon của con cá đã bắt")]
    public SpriteRenderer fishIconRenderer;
    [Tooltip("Sprite Renderer dùng để hiển thị background của icon cá")]
    public SpriteRenderer fishIconBackgroundRenderer;


    [Header("Hình ảnh (Sprite)")]
    public Sprite emptySprite;
    public Sprite baitedSprite;
    public Sprite readySprite;

    // Tham chiếu nội bộ
    private FishingZone associatedZone;
    private FishData caughtFish;
    private SpriteRenderer spriteRenderer;
    private BaitSO currentBait;

    private bool isTimerRunning = false;
    private float targetTimeOfDay;
    private int targetDay;


    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        if (trapItemSO == null)
        {
            Debug.LogError("LỖI: PlacedTrap Prefab chưa được gán 'Trap Item SO'!", this.gameObject);
        }
        if (fishIconRenderer == null || fishIconBackgroundRenderer == null)
        {
            Debug.LogWarning("Prefab bẫy chưa gán 'Fish Icon/Background Renderer'!", this.gameObject);
        }
        SetState(TrapState.Empty);
    }


    void Update()
    {
        // // Chỉ chạy logic khi timer đang bật
        // if (!isTimerRunning) return;

        // // An toàn nếu DayNight chưa được load
        // if (DayNight.Instance == null) return;

        // int currentDay = DayNight.Instance.currentDay;
        // float currentTime = DayNight.Instance.timeOfDay;

        // // 1. Kiểm tra xem đã qua ngày target chưa
        // if (currentDay > targetDay)
        // {
        //     OnTimerComplete();
        // }
        // // 2. Nếu cùng ngày, kiểm tra giờ
        // else if (currentDay == targetDay)
        // {
        //     if (currentTime >= targetTimeOfDay)
        //     {
        //         OnTimerComplete();
        //     }
        // }
    }


    public void Initialize(FishingZone zone)
    {
        associatedZone = zone;
    }


    private void SetState(TrapState newState)
    {
        currentState = newState;

        // Dừng timer nếu bẫy rỗng hoặc sẵn sàng
        if (newState == TrapState.Empty || newState == TrapState.ReadyToCollect)
        {
            isTimerRunning = false;
        }

        switch (currentState)
        {
            case TrapState.Empty:
                if (spriteRenderer != null && emptySprite != null)
                    spriteRenderer.sprite = emptySprite;

                // Sửa lỗi copy-paste: Ẩn cả 2
                if (fishIconRenderer != null) fishIconRenderer.enabled = false;
                if (fishIconBackgroundRenderer != null) fishIconBackgroundRenderer.enabled = false;

                currentBait = null;
                caughtFish = null;
                break;

            case TrapState.Baited:
                if (spriteRenderer != null && baitedSprite != null)
                    spriteRenderer.sprite = baitedSprite;

                // Sửa lỗi copy-paste: Ẩn cả 2
                if (fishIconRenderer != null) fishIconRenderer.enabled = false;
                if (fishIconBackgroundRenderer != null) fishIconBackgroundRenderer.enabled = false;
                break;

            case TrapState.ReadyToCollect:
                // Sửa lỗi: Thêm dòng gán sprite bẫy đầy
                if (spriteRenderer != null && readySprite != null)
                    spriteRenderer.sprite = readySprite;

                if (fishIconRenderer != null && caughtFish != null && caughtFish.icon != null)
                {
                    fishIconRenderer.sprite = caughtFish.icon;
                    fishIconRenderer.enabled = true;
                    if (fishIconBackgroundRenderer != null)
                    {
                        fishIconBackgroundRenderer.enabled = true;
                    }
                }
                else
                {
                    if (fishIconRenderer != null) fishIconRenderer.enabled = false;
                    if (fishIconBackgroundRenderer != null) fishIconBackgroundRenderer.enabled = false;
                }
                break;
        }
    }


    public bool TryAddBait(BaitSO baitItem, InventoryManager invManager)
    {
        if (currentState != TrapState.Empty)
        {
            return false;
        }

        if (invManager.RemoveItem(baitItem, 1))
        {
            currentBait = baitItem;
            SetState(TrapState.Baited);
            StartCatchTimer(); // Bắt đầu timer mới
            return true;
        }
        return false;
    }


    private void StartCatchTimer()
    {
        // if (currentBait == null) return;
        // if (DayNight.Instance == null)
        // {
        //     Debug.LogError("Không tìm thấy DayNight.Instance! Bẫy sẽ không hoạt động.");
        //     return;
        // }

        // // 1. Tính toán thời gian chờ (bằng giờ trong game)
        // float hoursToWait = baseTimeToCatch * (1.0f - currentBait.biteTimeMultiplier);
        // hoursToWait = Mathf.Max(0.1f, hoursToWait); // Chờ ít nhất 0.1 giờ

        // // 2. Lấy thời gian hiện tại
        // float currentTime = DayNight.Instance.timeOfDay;
        // int currentDay = DayNight.Instance.currentDay;

        // // 3. Tính toán ngày/giờ mục tiêu
        // targetDay = currentDay;
        // targetTimeOfDay = currentTime + hoursToWait;

        // // 4. Xử lý nếu giờ mục tiêu vượt qua 24h
        // // (Giả sử DayNight reset về 6h, không phải 0h)
        // while (targetTimeOfDay >= 24f)
        // {
        //     targetTimeOfDay = 6f + (targetTimeOfDay - 24f); // Bắt đầu ngày mới lúc 6h + thời gian dư
        //     targetDay++;
        // }

        // // 5. Bật timer
        // isTimerRunning = true;
        // //Debug.Log($"Bẫy sẽ sẵn sàng vào Ngày {targetDay} lúc {targetTimeOfDay}:00");
    }


    private void OnTimerComplete()
    {
        isTimerRunning = false; // Dừng timer

        if (associatedZone != null)
        {
            caughtFish = associatedZone.PickRandomFish();

            if (caughtFish != null)
            {
                SetState(TrapState.ReadyToCollect);
            }
            else
            {
                SetState(TrapState.Empty); // Mất mồi, không được cá
            }
        }
    }


    public void TryCollect(InventoryManager invManager)
    {
        if (currentState != TrapState.ReadyToCollect)
        {
            return;
        }

        if (caughtFish == null)
        {
            SetState(TrapState.Empty);
            return;
        }

        if (invManager.AddItem(caughtFish, 1))
        {
            SetState(TrapState.Empty);
        }
        else
        {
            Debug.Log("Túi đồ đầy!");
        }
    }
}