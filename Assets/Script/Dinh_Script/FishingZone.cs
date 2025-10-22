using UnityEngine;

// Gắn script này vào các GameObject "Water" Tilemap (hoặc Trigger)
public class FishingZone : MonoBehaviour
{
    // --- MỚI: TẠO MỘT CLASS LỒNG ĐỂ LƯU TỈ LỆ ---
    [System.Serializable]
    public class FishChance
    {
        [Tooltip("Loại cá")]
        public FishData fishData;

        [Tooltip("Tỉ lệ (weight). Số càng cao càng dễ gặp. Ví dụ: Cá chép = 10 (phổ biến), Cá mập = 1 (hiếm)")]
        [Min(1)]
        public int weight;
    }
    // --- KẾT THÚC MỚI ---


    [Header("Cá trong khu vực này")]
    [Tooltip("Danh sách TẤT CẢ các loại cá và tỉ lệ của chúng")]
    // --- ĐÃ THAY ĐỔI: Dùng list FishChance mới ---
    [SerializeField] private FishChance[] availableFish;

    // --- ĐÃ THAY ĐỔI: Hàm này giờ sẽ tự chọn 1 con cá dựa trên tỉ lệ ---
    public FishData PickRandomFish()
    {
        if (availableFish == null || availableFish.Length == 0)
        {
            return null;
        }

        int totalWeight = 0;

        // 1. Tính tổng "tỉ lệ" (weight) của tất cả cá
        foreach (var chance in availableFish)
        {
            totalWeight += chance.weight;
        }

        // 2. "Quay số" ngẫu nhiên từ 0 đến Tổng tỉ lệ
        int randomRoll = Random.Range(0, totalWeight);

        // 3. Tìm xem "số" vừa quay trúng vào con cá nào
        int cumulativeWeight = 0;
        foreach (var chance in availableFish)
        {
            cumulativeWeight += chance.weight;
            if (randomRoll < cumulativeWeight)
            {
                // Trúng con cá này!
                return chance.fishData;
            }
        }

        // Trường hợp dự phòng (không bao giờ nên xảy ra)
        return availableFish[0].fishData;
    }
}