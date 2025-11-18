using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class FerryTransition : MonoBehaviour
{
    [Header("Scene kế tiếp")]
    public string nextScene = "IslandScene"; // 🏝 Scene bến đến
    public float travelTime = 5f; // ⏳ Thời gian di chuyển trên phà

    [Header("Hiệu ứng UI")]
    public TMP_Text travelText;
    public Image fadePanel;

    private bool isTravelling = false;

    void Start()
    {
        if (fadePanel != null)
        {
            Color c = fadePanel.color;
            c.a = 1f; // Bắt đầu tối
            fadePanel.color = c;
            StartCoroutine(FadeIn());
        }

        if (travelText != null)
        {
            travelText.text = "Phà đang di chuyển";
            StartCoroutine(AnimateDots());
        }

        StartCoroutine(TravelAndLoad());
    }

    IEnumerator AnimateDots()
    {
        int dotCount = 0;
        string baseText = "Phà đang di chuyển";
        while (isTravelling == false)
        {
            dotCount = (dotCount + 1) % 4;
            travelText.text = baseText + new string('.', dotCount);
            yield return new WaitForSeconds(0.5f);
        }
    }

    IEnumerator TravelAndLoad()
    {
        isTravelling = false;

        // ⛴ Chờ "phà chạy"
        yield return new WaitForSeconds(travelTime);

        // 🔄 Fade out và chuyển scene
        if (fadePanel != null)
            yield return StartCoroutine(FadeOut());

        SceneManager.LoadScene(nextScene);
    }

    IEnumerator FadeIn()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(1f, 0f, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 0f;
        fadePanel.color = c;
    }

    IEnumerator FadeOut()
    {
        float duration = 1.5f;
        float elapsed = 0f;
        Color c = fadePanel.color;

        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            c.a = Mathf.Lerp(0f, 1f, elapsed / duration);
            fadePanel.color = c;
            yield return null;
        }

        c.a = 1f;
        fadePanel.color = c;
    }
}
