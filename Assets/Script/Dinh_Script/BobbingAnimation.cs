using UnityEngine;

public class BobbingAnimation : MonoBehaviour
{
    [Tooltip("Tốc độ nhấp nhô")]
    public float bobbingSpeed = 5f;

    [Tooltip("Độ cao nhấp nhô (tính bằng mét)")]
    public float bobbingHeight = 0.1f;

    // Vị trí ban đầu của sprite (thường là 0,0,0)
    private Vector3 startPos;

    void Awake()
    {
        // Lưu lại vị trí ban đầu (local)
        startPos = transform.localPosition;
    }

    void Update()
    {
        // Dùng Mathf.Sin để tạo một sóng hình sin mượt mà theo thời gian
        // Nó sẽ di chuyển từ -1 đến 1, rồi ta nhân với chiều cao
        float newY = startPos.y + Mathf.Sin(Time.time * bobbingSpeed) * bobbingHeight;

        // Áp dụng vị trí Y mới cho localPosition
        transform.localPosition = new Vector3(startPos.x, newY, startPos.z);
    }
}