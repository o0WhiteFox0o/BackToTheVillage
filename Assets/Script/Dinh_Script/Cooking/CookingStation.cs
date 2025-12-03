using UnityEngine;

public class CookingStation : MonoBehaviour
{
    [Header("Cấu hình tương tác")]
    [Tooltip("Phím để mở bếp")]
    [SerializeField] private KeyCode interactKey = KeyCode.F;

    [Tooltip("GameObject hiển thị chữ 'Bấm F' (tùy chọn)")]
    [SerializeField] private GameObject visualCue;

    [Header("Kết nối UI")]
    [Tooltip("Kéo script CookingUI (nằm trên Canvas) vào đây")]
    [SerializeField] private CookingUI cookingUI;

    private bool isPlayerInRange = false;

    private void Start()
    {
        // Ẩn gợi ý nút bấm lúc đầu
        if (visualCue != null) visualCue.SetActive(false);

        // Nếu quên chưa kéo UI thì tự tìm (phòng hờ)
        if (cookingUI == null)
        {
            cookingUI = FindObjectOfType<CookingUI>();
        }
    }

    private void Update()
    {
        // Nếu người chơi ở gần VÀ bấm phím tương tác
        if (isPlayerInRange && Input.GetKeyDown(interactKey))
        {
            OpenCookingInterface();
        }
    }
    private void OpenCookingInterface()
    {
        if (cookingUI != null)
        {
            cookingUI.TogglePanel();
        }
        else
        {
            Debug.LogError("Chưa gán CookingUI vào CookingStation!");
        }
    }
    // Dành cho game 2D
    private void OnTriggerEnter2D(Collider2D collision)
    {
        CheckPlayerEnter(collision.gameObject);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        CheckPlayerExit(collision.gameObject);
    }

    // --- LOGIC KIỂM TRA ---

    private void CheckPlayerEnter(GameObject obj)
    {
        // Đảm bảo Player của bạn có Tag là "Player"
        if (obj.CompareTag("Player"))
        {
            isPlayerInRange = true;
            if (visualCue != null) visualCue.SetActive(true);
        }
    }

    private void CheckPlayerExit(GameObject obj)
    {
        if (obj.CompareTag("Player"))
        {
            isPlayerInRange = false;
            if (visualCue != null) visualCue.SetActive(false);

            // Tùy chọn: Tự đóng bảng nếu đi xa quá
            // if (cookingUI != null) cookingUI.ClosePanel(); 
        }
    }

}