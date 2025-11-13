using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("Danh sách công thức đã mở khóa")]
    public List<CraftingRecipeSO> unlockedRecipes = new List<CraftingRecipeSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // (Không bắt buộc, nhưng nên có)
            // DontDestroyOnLoad(this.gameObject); 
        }
    }

    // === HÀM UNLOCKRECIPE ĐÃ ĐƯỢC "TINH CHỈNH" ===
    // (Lưu ý: Tên hàm là UnLockRecipe với 'L' hoa, khớp với code của bạn)
    public void UnLockRecipe(CraftingRecipeSO recipe)
    {
        if (recipe == null)
        {
            Debug.LogError("[CraftingManager] Nhận được một công thức NULL!");
            return;
        }

        if (unlockedRecipes.Contains(recipe))
        {
            Debug.LogWarning($"[CraftingManager] Đã cố mở khóa {recipe.name} nhưng đã có rồi.");
            return;
        }

        // 1. Thêm công thức
        unlockedRecipes.Add(recipe);
        Debug.Log($"[CraftingManager] Công thức {recipe.name} ĐÃ ĐƯỢC THÊM VÀO BỘ NHỚ.");

        // 2. "Tinh chỉnh" nằm ở đây:
        //    Chúng ta sẽ kiểm tra NotificationManager một cách "ồn ào"

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance?.ShowNotification($"Đã học công thức mới: {recipe.name}");
        }
    }
}