using UnityEngine;
using System.Collections;
// using Unity.VisualScripting; // Namespace này thường không cần thiết trừ khi dùng Visual Scripting Nodes

public class Bobber : MonoBehaviour
{
    public PlayerFishing playerFishingScript;

    // --- THÊM BIẾN SPRITE VÀ RENDERER ---
    [Header("Sprites & Renderer")]
    [Tooltip("Sprite phao câu khi đang bay")]
    [SerializeField] private Sprite flyingSprite;
    [Tooltip("Sprite phao câu khi đã ở dưới nước")]
    [SerializeField] private Sprite floatingSprite;
    [Tooltip("Kéo Sprite Renderer của child 'Sprite' vào đây")]
    [SerializeField] private SpriteRenderer bobberSpriteRenderer;
    // --- KẾT THÚC THÊM ---

    [Header("Visual Effects")] // Đổi tên Header cho rõ
    [Tooltip("Kéo Prefab hiệu ứng 'Splash' vào đây")]
    [SerializeField] private GameObject splashEffectPrefab;
    [Tooltip("Kéo child 'Shadow' vào đây")] // Thêm Tooltip
    [SerializeField] private GameObject shadow; // Giữ lại để ẩn khi chạm nước
    // [SerializeField] private GameObject hotBar; // <-- BỎ BIẾN NÀY, Bobber không nên điều khiển Hotbar

    [Header("Âm thanh")]
    [Tooltip("Kéo AudioSource component (gắn trên Bobber hoặc Player) vào đây nếu muốn dùng PlayOneShot")]
    [SerializeField] private AudioSource audioSource; // Dùng component này nếu có
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioClip thudSound;

    [Header("Cấu trúc Prefab")]
    [Tooltip("Kéo child 'Sprite' (chứa hình ảnh phao câu) vào đây")]
    [SerializeField] private Transform spriteTransform;
    public Transform SpriteTransform => spriteTransform;

    private Collider2D col;
    private Coroutine moveCoroutine;

    void Start()
    {
        Rigidbody2D rb = GetComponent<Rigidbody2D>();
        if (rb != null)
        {
            rb.isKinematic = true;
            rb.velocity = Vector2.zero;
        }

        col = GetComponent<Collider2D>();
        if (col != null) col.enabled = false;

        // --- KIỂM TRA SPRITE TRANSFORM VÀ RENDERER ---
        if (spriteTransform == null)
        {
            Debug.LogError("Chưa kéo child 'Sprite' vào script Bobber!");
        }
        // Tự động tìm SpriteRenderer nếu chưa gán
        if (bobberSpriteRenderer == null && spriteTransform != null)
        {
            bobberSpriteRenderer = spriteTransform.GetComponent<SpriteRenderer>();
        }
        if (bobberSpriteRenderer == null)
        {
            Debug.LogError("Không tìm thấy hoặc chưa gán Sprite Renderer trên child 'Sprite'!");
        }
        else if (flyingSprite != null)
        {
            // Đặt sprite ban đầu là sprite bay
            bobberSpriteRenderer.sprite = flyingSprite;
        }
        else
        {
            Debug.LogWarning("Chưa gán Flying Sprite cho Bobber!");
        }
        // Tự tìm AudioSource nếu chưa gán (tùy chọn)
        if (audioSource == null) audioSource = GetComponent<AudioSource>();
        // --- KẾT THÚC KIỂM TRA ---

        // Đảm bảo shadow hiện khi bắt đầu
        if (shadow != null) shadow.SetActive(true);
    }

    public void StartCast(Vector2 destination, float height, float duration)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        // Đảm bảo shadow hiện và sprite là flying khi bắt đầu quăng
        if (shadow != null) shadow.SetActive(true);
        if (bobberSpriteRenderer != null && flyingSprite != null)
        {
            bobberSpriteRenderer.sprite = flyingSprite;
        }
        moveCoroutine = StartCoroutine(MoveToTarget(destination, height, duration));
    }

    private IEnumerator MoveToTarget(Vector2 destination, float height, float duration)
    {
        Vector2 startPos = transform.position;
        Vector3 spriteStartPos = spriteTransform.localPosition;
        float timeElapsed = 0f;

        while (timeElapsed < duration)
        {
            float t = timeElapsed / duration;
            transform.position = Vector2.Lerp(startPos, destination, t);
            float yOffset = -4 * height * (Mathf.Pow(t, 2) - t);
            spriteTransform.localPosition = new Vector3(spriteStartPos.x, yOffset, spriteStartPos.z);
            timeElapsed += Time.deltaTime;
            yield return null;
        }

        transform.position = destination;
        spriteTransform.localPosition = spriteStartPos; // Reset vị trí local sprite về 0

        CheckLandSpot();
    }

    private void CheckLandSpot()
    {
        if (col != null) col.enabled = true;

        Collider2D[] hits = Physics2D.OverlapCircleAll(transform.position, 0.1f);
        bool inWater = false;
        FishingZone currentZone = null;

        foreach (Collider2D hit in hits)
        {
            if (hit.CompareTag("Water"))
            {
                inWater = true;
                currentZone = hit.GetComponent<FishingZone>();
                break;
            }
        }

        if (inWater && currentZone != null)
        {
            FishData pickedFish = currentZone.PickRandomFish();
            if (pickedFish != null)
            {
                HandleHitWater(pickedFish);
            }
            else
            {
                HandleHitGround(); // Nước nhưng không có cá = đất
            }
        }
        else
        {
            HandleHitGround();
        }
    }

    // --- SỬA HÀM HANDLEHITWATER ---
    private void HandleHitWater(FishData pickedFish)
    {
        Debug.Log("Hạ cánh trên NƯỚC");
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        spriteTransform.localPosition = Vector3.zero; // Reset vị trí local

        // Tạo hiệu ứng splash
        if (splashEffectPrefab != null) Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);

        // Chơi âm thanh splash (Ưu tiên dùng AudioSource nếu có)
        if (audioSource != null && splashSound != null)
        {
            audioSource.PlayOneShot(splashSound);
        }
        else if (splashSound != null) // Dự phòng dùng PlayClipAtPoint
        {
            AudioSource.PlayClipAtPoint(splashSound, transform.position);
        }

        // Đổi sang sprite nổi trên nước
        if (bobberSpriteRenderer != null && floatingSprite != null)
        {
            bobberSpriteRenderer.sprite = floatingSprite;
        }
        else if (floatingSprite == null)
        {
            Debug.LogWarning("Chưa gán Floating Sprite cho Bobber!");
        }

        // Ẩn bóng đi
        if (shadow != null) shadow.SetActive(false);

        // Gửi thông tin về Player
        if (playerFishingScript != null) playerFishingScript.OnBobberLanded(pickedFish);

        // hotBar.SetActive(false); // <-- BỎ DÒNG NÀY
    }
    // --- KẾT THÚC SỬA ---


    // --- SỬA NHẸ HÀM HANDLEHITGROUND ---
    private void HandleHitGround()
    {
        Debug.Log("Hạ cánh trên ĐẤT!");
        if (moveCoroutine != null) StopCoroutine(moveCoroutine); // Dừng coroutine nếu chạm đất sớm
        spriteTransform.localPosition = Vector3.zero; // Reset vị trí local

        // Đảm bảo sprite là sprite bay (hoặc sprite chạm đất nếu có)
        if (bobberSpriteRenderer != null && flyingSprite != null)
        {
            bobberSpriteRenderer.sprite = flyingSprite;
        }
        // Giữ lại bóng
        if (shadow != null) shadow.SetActive(true);


        // Chơi âm thanh chạm đất
        if (thudSound != null) AudioSource.PlayClipAtPoint(thudSound, transform.position);

        // Báo cho Player
        if (playerFishingScript != null) playerFishingScript.OnBobberLandedOnGround();
    }
    // --- KẾT THÚC SỬA ---
}