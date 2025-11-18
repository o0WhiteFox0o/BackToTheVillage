using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using System.Collections;
using TMPro;

public class FerryToNextScene : MonoBehaviour
{
    [Header("Thiết lập Scene")]
    public string sceneToLoad = "NextScene";
    public float delayBeforeLoad = 1.5f;

    [Header("Tương tác")]
    public KeyCode interactKey = KeyCode.E;
    private bool isPlayerNear = false;
    private bool isTransitioning = false;

    [Header("UI đối thoại (kéo thả trong Canvas)")]
    public GameObject dialogPanel;
    public Button yesButton;
    public Button noButton;

    [Header("Hiệu ứng Fade màn hình")]
    public Image fadePanel;
    public float fadeDuration = 1.5f;

    [Header("Âm thanh (AudioSource)")]
    public AudioSource audioSource;        // Gắn vào đối tượng có script này
    public AudioClip ferrySound;           // Tiếng phà khởi hành
    public AudioClip confirmSound;         // Khi bấm Yes
    public AudioClip cancelSound;          // Khi bấm No
    public AudioClip waterLoopSound;       // 🔹 Âm thanh nền nước/động cơ phà

    [Header("Hiển thị Loading (Text TMP)")]
    public TMP_Text loadingText;

    private bool isLoading = false;
    private AudioSource waterAudioSource; // 🔹 Tạo audio riêng cho tiếng nền

    void Start()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 0f;
            fadePanel.color = c;
        }

        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(false);
            Color c = loadingText.color;
            c.a = 0f;
            loadingText.color = c;
        }

        if (yesButton != null)
            yesButton.onClick.AddListener(OnYesClicked);

        if (noButton != null)
            noButton.onClick.AddListener(OnNoClicked);
    }

    void Update()
    {
        if (isPlayerNear && Input.GetKeyDown(interactKey) && !isTransitioning)
        {
            ShowDialog();
        }
    }

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
            isPlayerNear = true;
    }

    private void OnTriggerExit2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            isPlayerNear = false;
            if (dialogPanel != null)
                dialogPanel.SetActive(false);
        }
    }

    void ShowDialog()
    {
        if (dialogPanel != null)
            dialogPanel.SetActive(true);
    }

    void OnYesClicked()
    {
        if (audioSource != null && confirmSound != null)
            audioSource.PlayOneShot(confirmSound);

        if (dialogPanel != null)
            dialogPanel.SetActive(false);

        StartCoroutine(LoadSceneAfterDelay());
    }

    void OnNoClicked()
    {
        if (audioSource != null && cancelSound != null)
            audioSource.PlayOneShot(cancelSound);

        if (dialogPanel != null)
            dialogPanel.SetActive(false);
    }

    private IEnumerator LoadSceneAfterDelay()
    {
        isTransitioning = true;

        // 🔊 Phát tiếng phà khởi hành
        if (audioSource != null && ferrySound != null)
            audioSource.PlayOneShot(ferrySound);

        // 🔹 Hiện chữ “Đang lên phà...”
        if (loadingText != null)
        {
            loadingText.gameObject.SetActive(true);
            isLoading = true;
            StartCoroutine(AnimateLoadingText());
            StartCoroutine(FadeLoadingText());
        }

        // 🔊 Âm thanh nền (nước hoặc động cơ phà)
        if (waterLoopSound != null)
        {
            waterAudioSource = gameObject.AddComponent<AudioSource>();
            waterAudioSource.clip = waterLoopSound;
            waterAudioSource.loop = true;
            waterAudioSource.volume = 0.4f;
            waterAudioSource.Play();
        }

        // 🔥 Hiệu ứng fade màn hình
        if (fadePanel != null)
            yield return StartCoroutine(SmoothFade(0f, 1f));

        yield return new WaitForSeconds(delayBeforeLoad);

        isLoading = false;

        // 🔇 Tắt âm thanh nền
        if (waterAudioSource != null)
            waterAudioSource.Stop();

        SceneManager.LoadScene(sceneToLoad);
    }

    // 🔸 Chạy dấu ba chấm
    private IEnumerator AnimateLoadingText()
    {
        string baseText = "Đang lên phà";
        int dotCount = 0;

        while (isLoading)
        {
            dotCount = (dotCount + 1) % 4; // từ 0 -> 3
            loadingText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.4f);
        }
    }

    // 🔸 Hiệu ứng fade chữ (mờ dần - sáng dần)
    private IEnumerator FadeLoadingText()
    {
        Color c = loadingText.color;
        float speed = 2f;

        while (isLoading)
        {
            // Fade In
            while (c.a < 1f && isLoading)
            {
                c.a += Time.deltaTime * speed;
                loadingText.color = c;
                yield return null;
            }

            // Fade Out
            while (c.a > 0.3f && isLoading)
            {
                c.a -= Time.deltaTime * speed;
                loadingText.color = c;
                yield return null;
            }
        }
    }

    // 🔸 Fade màn hình
    private IEnumerator SmoothFade(float startAlpha, float endAlpha)
    {
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < fadeDuration)
        {
            elapsed += Time.deltaTime;
            float t = Mathf.SmoothStep(0f, 1f, elapsed / fadeDuration);
            c.a = Mathf.Lerp(startAlpha, endAlpha, t);
            fadePanel.color = c;
            yield return null;
        }

        c.a = endAlpha;
        fadePanel.color = c;
    }
}
