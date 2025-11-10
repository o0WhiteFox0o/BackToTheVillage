using System.Collections;
using UnityEngine;
using Management; // Thêm namespace của InventoryManager

public class PlacedTrap : MonoBehaviour
{
    public enum TrapState
    {
        Empty,          // Mới đặt, rỗng
        Baited,         // Đã có mồi, đang chờ
        ReadyToCollect  // Đã bắt được cá
    }

    [Header("Trạng thái")]
    [SerializeField] private TrapState currentState = TrapState.Empty;

    [Header("Cấu hình")]
    [Tooltip("Thời gian (giây) CƠ BẢN để bắt được cá (chưa tính mồi)")]
    public float baseTimeToCatch = 60f;

    [Header("Hình ảnh (Sprite)")]
    [Tooltip("Hình ảnh bẫy rỗng (Trạng thái Empty)")]
    public Sprite emptySprite;
    [Tooltip("Hình ảnh bẫy đã có mồi (Trạng thái Baited)")]
    public Sprite baitedSprite; // <-- SPRITE MỚI
    [Tooltip("Hình ảnh bẫy đầy (Trạng thái ReadyToCollect)")]
    public Sprite readySprite;

    // Tham chiếu nội bộ
    private FishingZone associatedZone;
    private FishData caughtFish;
    private SpriteRenderer spriteRenderer;
    private BaitSO currentBait;

    void Awake()
    {
        spriteRenderer = GetComponent<SpriteRenderer>();
        SetState(TrapState.Empty); // Bắt đầu với trạng thái rỗng
    }

    /// <summary>
    /// Được gọi bởi TrapInteraction khi bẫy được ĐẶT XUỐNG
    /// </summary>
    public void Initialize(FishingZone zone)
    {
        associatedZone = zone;
    }

    /// <summary>
    /// Cập nhật trạng thái và hình ảnh của bẫy
    /// </summary>
    private void SetState(TrapState newState)
    {
        currentState = newState;
        switch (currentState)
        {
            case TrapState.Empty:
                if (spriteRenderer != null && emptySprite != null)
                    spriteRenderer.sprite = emptySprite;
                currentBait = null;
                caughtFish = null;
                break;
            case TrapState.Baited:
                if (spriteRenderer != null && baitedSprite != null)
                    spriteRenderer.sprite = baitedSprite;
                break;
            case TrapState.ReadyToCollect:
                if (spriteRenderer != null && readySprite != null)
                    spriteRenderer.sprite = readySprite;
                break;
        }
    }

    /// <summary>
    /// Thử thêm mồi vào bẫy (được gọi bởi TrapInteraction)
    /// </summary>
    public bool TryAddBait(BaitSO baitItem, InventoryManager invManager)
    {
        // Chỉ thêm mồi nếu bẫy đang rỗng
        if (currentState != TrapState.Empty)
        {
            Debug.Log("Bẫy không rỗng, không thể thêm mồi.");
            return false;
        }

        // Thử trừ 1 mồi từ túi đồ
        if (invManager.RemoveItem(baitItem, 1)) //
        {
            Debug.Log($"Đã thêm mồi: {baitItem.displayName}");
            currentBait = baitItem;
            SetState(TrapState.Baited);
            StartCatchTimer(); // Bắt đầu đếm giờ
            return true;
        }

        Debug.Log("Không thể thêm mồi (lỗi không rõ).");
        return false;
    }

    private void StartCatchTimer()
    {
        if (currentBait == null)
        {
            Debug.LogError("Lỗi: StartCatchTimer được gọi khi không có mồi!");
            return;
        }

        // Tính toán thời gian bắt cá dựa trên mồi
        // (1.0 - 0.2) = 0.8 -> Nhanh hơn 20%
        float multiplier = 1.0f - currentBait.biteTimeMultiplier; //
        float finalTimeToCatch = baseTimeToCatch * multiplier;
        finalTimeToCatch = Mathf.Max(1f, finalTimeToCatch); // Đảm bảo không ít hơn 1 giây

        Debug.Log($"Bẫy sẽ sẵn sàng trong {finalTimeToCatch} giây.");
        StartCoroutine(CatchFishCoroutine(finalTimeToCatch));
    }

    private IEnumerator CatchFishCoroutine(float waitTime)
    {
        yield return new WaitForSeconds(waitTime);

        if (associatedZone != null)
        {
            caughtFish = associatedZone.PickRandomFish(); //

            if (caughtFish != null)
            {
                SetState(TrapState.ReadyToCollect);
                Debug.Log("Bẫy đã bắt được cá!");
            }
            else
            {
                Debug.Log("Bẫy không bắt được gì, reset về rỗng.");
                SetState(TrapState.Empty); // Mất mồi, không được cá
            }
        }
    }

    /// <summary>
    /// Thử thu hoạch cá từ bẫy (được gọi bởi TrapInteraction)
    /// </summary>
    public void TryCollect(InventoryManager invManager)
    {
        // Chỉ thu hoạch khi đã sẵn sàng
        if (currentState != TrapState.ReadyToCollect)
        {
            Debug.Log("Bẫy chưa sẵn sàng.");
            return;
        }

        if (caughtFish == null)
        {
            Debug.LogError("Trạng thái sẵn sàng nhưng không có cá? Reset.");
            SetState(TrapState.Empty);
            return;
        }

        // Thử thêm cá vào túi đồ
        if (invManager.AddItem(caughtFish, 1)) //
        {
            Debug.Log($"Đã thu hoạch: {caughtFish.displayName}");
            // Quan trọng: Sau khi thu hoạch, bẫy trở về rỗng (Empty)
            SetState(TrapState.Empty);
        }
        else
        {
            Debug.Log("Túi đồ đầy, không thể thu hoạch!");
        }
    }
}