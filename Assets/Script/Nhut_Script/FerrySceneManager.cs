using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FerrySceneManager : MonoBehaviour
{
    [Header("Scene kế tiếp")]
    public string nextScene = "IslandScene";
    public float totalTravelTime = 10f;

    [Header("Hiệu ứng Fade")]
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Hiển thị chữ")]
    public TMP_Text travelText;

    [Header("Âm thanh")]
    public AudioSource engineAudio;
    public AudioSource waterAudio;
    public AudioSource bellAudio;

    [Header("Hiệu ứng Background")]
    public Renderer waterRenderer;
    public float scrollSpeed = 0.05f;

    [Header("Hiệu ứng Rung Camera")]
    public Transform cameraTransform;
    public float shakeIntensity = 0.05f;
    public float shakeSpeed = 5f;

    private bool isTravelling = true;
    private Vector3 originalCamPos;
    private Coroutine currentAnim; // 🟢 Thêm biến này

    void Start()
    {
        if (cameraTransform != null)
            originalCamPos = cameraTransform.localPosition;

        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f;
            fadePanel.color = c;
            StartCoroutine(FadeIn());
        }

        if (engineAudio != null) engineAudio.Play();
        if (waterAudio != null) waterAudio.Play();

        StartCoroutine(TravelRoutine());
    }

    void Update()
    {
        if (waterRenderer != null)
        {
            float offset = Time.time * scrollSpeed;
            waterRenderer.material.mainTextureOffset = new Vector2(offset, 0);
        }

        if (cameraTransform != null)
        {
            float shakeX = Mathf.Sin(Time.time * shakeSpeed) * shakeIntensity;
            float shakeY = Mathf.Cos(Time.time * shakeSpeed * 1.3f) * shakeIntensity;
            cameraTransform.localPosition = originalCamPos + new Vector3(shakeX, shakeY, 0);
        }
    }

    IEnumerator TravelRoutine()
    {
        // Giai đoạn 1: Đang di chuyển
        StartNewAnimation("Phà đang di chuyển");
        yield return new WaitForSeconds(totalTravelTime * 0.6f);

        // Giai đoạn 2: Sắp đến
        if (bellAudio != null) bellAudio.Play();
        StartNewAnimation("Phà sắp cập bến");
        yield return new WaitForSeconds(totalTravelTime * 0.3f);

        // Giai đoạn 3: Đang xuống phà
        StartNewAnimation("Đang xuống phà");
        if (fadePanel != null)
            yield return StartCoroutine(FadeOut());

        isTravelling = false;
        SceneManager.LoadScene(nextScene);
    }

    // 🟢 Hàm khởi chạy animation mới, tự dừng cái cũ
    void StartNewAnimation(string text)
    {
        if (currentAnim != null)
            StopCoroutine(currentAnim);
        currentAnim = StartCoroutine(AnimateDots(text));
    }

    IEnumerator AnimateDots(string baseText)
    {
        int dotCount = 0;
        float dotSpeed = 0.4f;

        while (isTravelling)
        {
            dotCount = (dotCount + 1) % 4;
            if (travelText != null)
                travelText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(dotSpeed);
        }
    }

    IEnumerator FadeIn()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 0f;
        fadePanel.color = c;
    }

    IEnumerator FadeOut()
    {
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / fadeDuration);
            fadePanel.color = c;
            yield return null;
        }
        c.a = 1f;
        fadePanel.color = c;
    }
}
