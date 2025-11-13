using UnityEngine;
using System.Collections.Generic;

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager Instance { get; private set; }

    // Thay vì lưu kết quả, ta lưu DANH SÁCH CÁC NGHỀ ĐÃ HỌC
    [Header("Danh sách các nghề đang kích hoạt")]
    public List<PerkSO> activeProfessions = new List<PerkSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    /// <summary>
    /// Hàm này được gọi khi người chơi chọn nghề mới
    /// </summary>
    public void ApplyProfessionBonus(PerkSO profession)
    {
        if (profession == null) return;

        // KIỂM TRA TRÙNG LẶP: Nếu đã có nghề này rồi thì không thêm nữa
        if (!activeProfessions.Contains(profession))
        {
            activeProfessions.Add(profession);
            Debug.Log($"[Stats] Đã kích hoạt nghề: {profession.perkName}");
        }
        else
        {
            Debug.LogWarning($"[Stats] Nghề {profession.perkName} đã tồn tại, không thêm lại.");
        }
    }

    /// <summary>
    /// HÀM QUAN TRỌNG: Tính toán giá trị chỉ số (Tự động cộng dồn)
    /// </summary>
    public float GetStatValue(StatType type, float defaultValue = 1.0f)
    {
        // 1. Bắt đầu với giá trị mặc định (thường là 1.0)
        float totalValue = defaultValue;

        // 2. Duyệt qua TẤT CẢ nghề đã học
        foreach (var profession in activeProfessions)
        {
            if (profession == null) continue;

            // Tìm xem nghề này có buff cho chỉ số 'type' không
            foreach (var modifier in profession.modifiers)
            {
                if (modifier.statType == type)
                {
                    // Cộng dồn giá trị (Ví dụ: +0.1 rồi +0.2)
                    totalValue += modifier.valueToAdd;
                }
            }
        }

        // 3. Trả về tổng cuối cùng
        return totalValue;
    }

    // Hàm tính giá (Giữ nguyên, nhưng giờ nó gọi hàm GetStatValue mới ở trên)
    public int GetActualItemPrice(ItemScriptableObject item)
    {
        if (item == null) return 0;

        float multiplier = 1.0f;

        if (item is FishData)
        {
            // Mặc định là 1.0 (100%). 
            // Nếu có Fisher (+0.1) -> 1.1
            // Nếu có thêm Angler (+0.2) -> 1.3
            multiplier = GetStatValue(StatType.FishingPriceBonus, 1.0f);
        }

        // Làm tròn số
        return Mathf.RoundToInt(item.sellPrice * multiplier);
    }
}