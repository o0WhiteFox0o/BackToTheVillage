using UnityEngine;

// Code này tạo một mục menu mới: "Create > Fishing > Fish Data"
[CreateAssetMenu(fileName = "New Fish", menuName = "Fishing/Fish Data")]
public class FishData : ItemScriptableObject
{
    [Header("Thông tin cơ bản")]
    public string fishName;

    [Header("Độ khó QTE (Kéo co)")]
    [Tooltip("Tốc độ thanh trắng chạy qua lại (ping-pong) (1 = chậm, 5 = rất nhanh)")]
    [Range(0.5f, 5f)]
    public float qteBarSpeed = 1f;

    [Tooltip("Kích thước vùng xanh thành công (0.5 = 50% = dễ, 0.1 = 10% = khó)")]
    [Range(0.05f, 0.5f)]
    public float successWindowSize = 0.2f;

    [Tooltip("Tổng thời gian cho phép để bắt cá (giây)")]
    [Range(5f, 50)]
    public float maxGameTime = 10f;

    [Tooltip("Lượng tiến độ TĂNG lên khi nhấn trúng (0.0 - 1.0)")]
    [Range(0.01f, 0.2f)]
    public float progressIncrease = 0.1f;

    [Tooltip("Lượng tiến độ GIẢM xuống khi nhấn hụt (0.0 - 1.0)")]
    [Range(0.01f, 0.2f)]
    public float progressDecrease = 0.05f;
}