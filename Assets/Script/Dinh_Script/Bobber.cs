using UnityEngine;
using System.Collections;
using Unity.VisualScripting;

public class Bobber : MonoBehaviour
{
    public PlayerFishing playerFishingScript;

    [Tooltip("Kéo Prefab hiệu ứng 'Splash' vào đây")]
    [SerializeField] private GameObject splashEffectPrefab;
    [SerializeField]private GameObject shadow;

    [Header("Âm thanh")]
    [SerializeField] private AudioSource audioSource;
    [SerializeField] private AudioClip splashSound;
    [SerializeField] private AudioClip thudSound;

    [Header("Cấu trúc Prefab")]
    [Tooltip("Kéo child 'Sprite' (chứa hình ảnh phao câu) vào đây")]
    [SerializeField] private Transform spriteTransform;

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

        if (spriteTransform == null)
        {
            Debug.LogError("Chưa kéo child 'Sprite' vào script Bobber!");
        }
    }

    public void StartCast(Vector2 destination, float height, float duration)
    {
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
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
        spriteTransform.localPosition = spriteStartPos;

        CheckLandSpot();
    }

    // --- HÀM CHECKLANDSPOT ĐÃ CẬP NHẬT ---
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
            // --- ĐÃ THAY ĐỔI: Yêu cầu FishingZone chọn 1 con cá ---
            FishData pickedFish = currentZone.PickRandomFish();

            // Nếu khu vực có cá (pickedFish != null)
            if (pickedFish != null)
            {
                HandleHitWater(pickedFish);
            }
            else
            {
                // Nếu khu vực là nước nhưng không có cá, coi như là đất
                HandleHitGround();
            }
        }
        else
        {
            HandleHitGround();
        }
    }

    // --- SỬA HÀM NÀY: Giờ nó nhận vào 1 con cá (FishData) ---
    private void HandleHitWater(FishData pickedFish)
    {
        Debug.Log("Hạ cánh trên NƯỚC");
        if (moveCoroutine != null) StopCoroutine(moveCoroutine);
        spriteTransform.localPosition = Vector3.zero;

        if (splashEffectPrefab != null) Instantiate(splashEffectPrefab, transform.position, Quaternion.identity);
        if (splashSound != null && audioSource != null) audioSource.PlayOneShot(splashSound);
        if (splashSound != null) AudioSource.PlayClipAtPoint(splashSound, transform.position);

        // --- Gửi con cá đã chọn về cho Player ---
        if (playerFishingScript != null) playerFishingScript.OnBobberLanded(pickedFish);
        shadow.SetActive(false);
    }

    private void HandleHitGround()
    {
        Debug.Log("Hạ cánh trên ĐẤT!");
        if (thudSound != null) AudioSource.PlayClipAtPoint(thudSound, transform.position);
        if (playerFishingScript != null) playerFishingScript.OnBobberLandedOnGround();
    }
}