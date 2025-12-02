using UnityEngine;

// Enum các loại minigame
public enum MinigameType
{
    Chopping, // Băm chặt (Spam nút)
    Timing,   // Canh thời gian (Thanh trượt)
    Rhythm    // Nhịp điệu (Bấm đúng thứ tự)
}

[CreateAssetMenu(fileName = "New Cooking Recipe", menuName = "Cooking/Cooking Recipe")]
public class CookingRecipeSO : CraftingRecipeSO
{
    [Header("Cooking Specifics")]
    public MinigameType minigameType;

    [Tooltip("Số lần nấu thành công để mở khóa Nấu Nhanh")]
    public int masteryThreshold = 5;

    // Override để phân loại item nếu cần
    private void OnValidate()
    {
        // Có thể tạo thêm ItemType.CookingRecipe nếu muốn
        itemType = ItemType.CraftingRecipe;
        stackable = false;
        canSell = false;
    }
}