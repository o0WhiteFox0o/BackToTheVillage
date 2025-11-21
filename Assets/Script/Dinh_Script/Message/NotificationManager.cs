using System.Collections;
using UnityEngine;
using TMPro;

public class NotificationManager : MonoBehaviour
{
    public static NotificationManager Instance { get; private set; }

    [Header("Tham chiếu UI")]
    [Tooltip("Kéo Panel UI (Image/GameObject) vào đây")]
    [SerializeField] private GameObject notificationPanel;
    [Tooltip("Kéo Text (TMP_Text) bên trong Panel vào đây")]
    [SerializeField] private TMP_Text notificationText;
    [SerializeField] private float displayTime = 3f;

    private Coroutine currentNotification;

    void Awake()
    {
        if (Instance != null)
        {
            Debug.LogWarning("[NotificationManager] Bị trùng lặp! Hủy bỏ...");
            Destroy(gameObject);
        }
        else
        {
            Instance = this;
            // (Không bắt buộc, nhưng nên có)
            // DontDestroyOnLoad(this.gameObject);
        }
    }

    void Start()
    {
        // 1. Tự kiểm tra lỗi cài đặt (thiếu UI)
        if (notificationPanel == null || notificationText == null)
        {
            Debug.LogError($"[NotificationManager] LỖI CÀI ĐẶT: 'Notification Panel' hoặc 'Notification Text' chưa được kéo vào Inspector của '{this.gameObject.name}'!");
            this.enabled = false; // Tắt script này đi
            return;
        }

        // 2. Tắt Panel và BÁO CÁO RẰNG NÓ ĐÃ SẴN SÀNG
        notificationPanel.SetActive(false);
        Debug.LogWarning("[NotificationManager] ĐÃ SẴN SÀNG! (Awake + Start đã chạy thành công).");
    }

    public void ShowNotification(string message)
    {
        if (notificationPanel == null || notificationText == null)
        {
            Debug.LogError("[NotificationManager] Không thể hiển thị thông báo vì Panel hoặc Text bị null!");
            return;
        }

        if (currentNotification != null)
        {
            StopCoroutine(currentNotification);
        }
        currentNotification = StartCoroutine(ShowNotificationCoroutine(message));
    }

    private IEnumerator ShowNotificationCoroutine(string message)
    {
        notificationText.text = message;
        notificationPanel.SetActive(true);
        yield return new WaitForSeconds(displayTime);
        notificationPanel.SetActive(false);
        currentNotification = null;
    }
}