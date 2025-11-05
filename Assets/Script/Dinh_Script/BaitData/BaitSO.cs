using UnityEngine;


[CreateAssetMenu(fileName = "New Bait", menuName = "Scriptable Object/Item/Bait")]
public class BaitSO : ItemScriptableObject
{
    // --- THAY THẾ DÒNG 'public BaitEffect effect;' BẰNG CÁC DÒNG NÀY ---
    [Header("Bait Stats (Chỉ số mồi)")]

    [Tooltip("Hệ số nhân thời gian chờ. (0 = không đổi, 0.2 = nhanh hơn 20%)")]
    public float biteTimeMultiplier = 0f;
    [Tooltip("Tốc độ thanh quay khi cá cắn câu. (0 = không đổi, 0.2 = chậm hơn 20%)")]
    public float fishingBarSpeedModifier = 0f;
    [Tooltip("Tăng độ rông vùng thành công. (1.0 = không đổi, 0.2 = rộng hơn hơn 20%)")]
    public float successWindowSizeModifier = 0f;
    [Tooltip("Tăng tiến độ khi cá cắn câu. (0 = không đổi, 0.2 = nhanh hơn 20%)")]
    public float progressIncreaseModifier = 0f;

    //Tạm thời chưa dùng đến
    //[Tooltip("Tỷ lệ cộng thêm để câu được rương báu. (0.05 = 5%)")]
    //public float bonusTreasureChance = 0.0f;
    //[Tooltip("Mồi này có cho phép câu cá bất kỳ (bỏ qua mùa/thời tiết) không?")]
    //public bool allowAnyFish = false;

    // (Bạn có thể thêm bất kỳ chỉ số nào khác bạn muốn ở đây)
    // Ví dụ: public float bonusRareFishChance = 0.0f;

    private void OnValidate()
    {
        itemType = ItemType.Bait;
        stackable = true;
    }
}