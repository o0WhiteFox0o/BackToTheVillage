using Management;
using System;
using System.Collections.Generic;
using UnityEngine;

public class CraftingManager : MonoBehaviour
{
    public static CraftingManager Instance { get; private set; }

    [Header("Danh sách công thức đã mở khóa")]
    public List<CraftingRecipeSO> unlockedRecipes = new List<CraftingRecipeSO>();

    // Event này sẽ "báo" cho UI biết khi có công thức mới
    public event Action<CraftingRecipeSO> OnRecipeUnlocked;

    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            // DontDestroyOnLoad(this.gameObject);
        }
    }

    // (Hàm này được gọi bởi SkillManager khi lên cấp)
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

        unlockedRecipes.Add(recipe);
        OnRecipeUnlocked?.Invoke(recipe); // Bắn tín hiệu cho UI

        Debug.Log($"[CraftingManager] Công thức {recipe.name} ĐÃ ĐƯỢC THÊM VÀO BỘ NHỚ.");

        if (NotificationManager.Instance != null)
        {
            NotificationManager.Instance.ShowNotification($"Đã học công thức mới: {recipe.name}");
        }
    }


    /// <summary>
    /// Kiểm tra xem người chơi có đủ nguyên liệu để chế tạo không
    /// </summary>
    public bool CanCraft(CraftingRecipeSO recipe)
    {
        if (recipe == null) return false;

        // Mặc định là 1.0 (100% nguyên liệu)
        float costMultiplier = 1.0f;


        foreach (var material in recipe.materials)
        {
            // Tính số lượng thực tế cần
            int requiredAmount = Mathf.CeilToInt(material.quantity * costMultiplier);

            if (InventoryManager.Instance.GetTotalItemQuantity(material.item) < requiredAmount)
            {
                return false; // Thiếu 1 món
            }
        }
        return true; // Đủ tất cả
    }

    /// <summary>
    /// Thực hiện chế tạo: Trừ nguyên liệu và thêm vật phẩm
    /// </summary>
    public void Craft(CraftingRecipeSO recipe)
    {
        if (!CanCraft(recipe))
        {
            Debug.LogWarning($"Không đủ nguyên liệu để chế {recipe.name}");
            return;
        }

        // Lấy hệ số (giống hệt hàm CanCraft)
        float costMultiplier = 1.0f;


        // 1. Trừ nguyên liệu
        foreach (var material in recipe.materials)
        {
            int amountToConsume = Mathf.CeilToInt(material.quantity * costMultiplier);
            InventoryManager.Instance.RemoveItem(material.item, amountToConsume);
        }

        // 2. Thêm vật phẩm thành phẩm
        InventoryManager.Instance.AddItem(recipe.itemToCraft, recipe.quantityCreated);

        Debug.Log($"Đã chế tạo thành công: {recipe.name}!");
    }
}