using UnityEngine;
using System.Collections;

public class Bobber : MonoBehaviour
{
    public PlayerFishing playerFishingScript;

    [Tooltip("Kéo Prefab hiệu ứng 'Splash' vào đây")]
    [SerializeField] private GameObject splashEffectPrefab;

    [Header("Âm thanh")]
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioClip thudSound;

    [Header("Cấu trúc Prefab")]
    [Tooltip("Kéo child 'Sprite' (chứa hình ảnh phao câu) vào đây")]
    [SerializeField] private Transform spriteTransform;

    private Collider2D col;
    private Coroutine moveCoroutine;

    void Start()
    {
        // Chúng ta sẽ dùng Rigidbody Kinematic
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        col = GetComponent<Collider2D>();
        // Tắt collider đi trong suốt quá trình bay
        if (col != null) col.enabled = false;

        if (spriteTransform == null)
        {
            Debug.LogError("Chưa kéo child 'Sprite' vào script Bobber!");
        }
    }

    // Hàm này được PlayerFishing.cs gọi
    public void StartCast(Vector2 destination, float height, float duration)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        moveCoroutine = StartCoroutine(MoveToTarget(destination, height, duration));
    }

    // Coroutine tự động di chuyển phao (cả vòng cung VÀ vị trí)
    private IEnumerator MoveToTarget(Vector2 destination, float height, float duration)
    {
        Vector2 startPos = transform.position;
        Vector3 spriteStartPos = spriteTransform.localPosition;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration; // % hoàn thành (0.0 -> 1.0)

            // 1. Di chuyển object cha (trên mặt đất)
            transform.position = Vector2.Lerp(startPos, destination, t);

            // 2. Di chuyển object con (vòng cung)
            // Dùng công thức Parabol (y = -4h * (t^2 - t))
            float yOffset = -4 * height * (Mathf.Pow(t, 2) - t);
            spriteTransform.localPosition = new Vector3(spriteStartPos.x, yOffset, spriteStartPos.z);

            timeElapsed += Time.deltaTime;
            yield return null;
        }

        // Kết thúc bay, snap về vị trí cuối cùng
        transform.position = destination;
        spriteTransform.localPosition = spriteStartPos;

        // Bây giờ MỚI kiểm tra xem hạ cánh ở đâu
        CheckLandSpot();
    }

    // *** HÀM MỚI: KIỂM TRA ĐIỂM HẠ CÁNH ***
    private void CheckLandSpot()
    {
        // Bật collider lên 1 frame chỉ để kiểm tra
        if (col != null) col.enabled = true;

        // Kiểm tra xem phao đang đè lên cái gì
        // (Chúng ta cần LayerMask cho "Water" và "Ground")
        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);

        bool inWater = false;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Water"))
            {
                inWater = true;
                break; // Ưu tiên Nước
            }
        }

        // Tắt collider đi (nếu cần)
        // if (col != null) col.enabled = false; 

        // Xử lý kết quả
        if (inWater)
        {
            HandleHitWater();
        }
        else
        {
            HandleHitGround();
        }
    }

    private void HandleHitWater()
    {
        Debug.Log("Hạ cánh trên NƯỚC");
        // Dừng sprite lại (nếu nó vẫn đang chạy animation do trễ frame)
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        spriteTransform.localPosition = Vector3.zero;

        if (splashEffectPrefab != null) Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        if (splashSound != null) AudioSource.PlayClipAtPoint(splashSound, transform.position);
        if (playerFishingScript != null) playerFishingScript.OnBobberLanded();
    }

    private void HandleHitGround()
    {
        Debug.Log("Hạ cánh trên ĐẤT!");
        if (thudSound != null) AudioSource.PlayClipAtPoint(thudSound, transform.position);
        if (playerFishingScript != null) playerFishingScript.OnBobberLandedOnGround();
    }
}