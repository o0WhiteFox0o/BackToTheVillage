using Management;
using System.Collections.Generic;
using UnityEngine;

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance { get; private set; }

    [Header("References")]
    [SerializeField] private MinigameController minigameController;

    [Header("Data")]
    // Lưu số lần đã nấu thành công mỗi món: Dictionary<Tên món, Số lần>
    // Trong thực tế bạn nên lưu Dictionary<int ID, int count> và Save vào file JSON
    public Dictionary<string, int> masteryTracker = new Dictionary<string, int>();

    public List<CookingRecipeSO> unlockedCookingRecipes = new List<CookingRecipeSO>();

    private void Awake()
    {
        if (Instance != null && Instance != this) Destroy(gameObject);
        else Instance = this;
    }

    public void UnlockRecipe(CookingRecipeSO recipe)
    {
        if (!unlockedCookingRecipes.Contains(recipe))
            unlockedCookingRecipes.Add(recipe);
    }

    // Kiểm tra đã đủ trình độ nấu nhanh chưa
    public bool IsMastered(CookingRecipeSO recipe)
    {
        if (masteryTracker.ContainsKey(recipe.name))
        {
            return masteryTracker[recipe.name] >= recipe.masteryThreshold;
        }
        return false;
    }

    public int GetMasteryCount(CookingRecipeSO recipe)
    {
        return masteryTracker.ContainsKey(recipe.name) ? masteryTracker[recipe.name] : 0;
    }

    // Hàm gọi từ UI
    public void RequestCook(CookingRecipeSO recipe, System.Action onCookFinishedUIUpdate)
    {
        // 1. Kiểm tra nguyên liệu (Dùng logic của CraftingManager hoặc InventoryManager trực tiếp)
        if (!CraftingManager.Instance.CanCraft(recipe))
        {
            Debug.Log("Không đủ nguyên liệu!");
            return;
        }

        // 2. Trừ nguyên liệu NGAY LẬP TỨC (để tránh exploit)
        ConsumeIngredients(recipe);

        // 3. Kiểm tra Mastery
        if (IsMastered(recipe))
        {
            // Nấu nhanh
            CompleteCooking(recipe, true);
            onCookFinishedUIUpdate?.Invoke();
        }
        else
        {
            // Chơi Minigame
            minigameController.StartMinigame(recipe, (isSuccess) =>
            {
                if (isSuccess)
                {
                    CompleteCooking(recipe, false);
                }
                else
                {
                    // Thua game: Mất nguyên liệu và nhận món thất bại (hoặc không gì cả)
                    HandleCookingFailure(recipe);
                }
                onCookFinishedUIUpdate?.Invoke();
            });
        }
    }

    private void ConsumeIngredients(CookingRecipeSO recipe)
    {
        foreach (var material in recipe.materials)
        {
            InventoryManager.Instance.RemoveItem(material.item, material.quantity);
        }
    }

    private void CompleteCooking(CookingRecipeSO recipe, bool isQuickCook)
    {
        // Thêm món ăn vào túi
        InventoryManager.Instance.AddItem(recipe.itemToCraft, recipe.quantityCreated);

        // Tăng điểm Mastery
        if (!masteryTracker.ContainsKey(recipe.name)) masteryTracker[recipe.name] = 0;
        masteryTracker[recipe.name]++;

        string method = isQuickCook ? "Nấu nhanh" : "Nấu thủ công";
        Debug.Log($"Đã nấu xong {recipe.name} ({method}). Mastery: {masteryTracker[recipe.name]}/{recipe.masteryThreshold}");
        if (NotificationManager.Instance != null)
            NotificationManager.Instance.ShowNotification($" {recipe.name}");
    }

    private void HandleCookingFailure(CookingRecipeSO recipe)
    {
        Debug.Log("Nấu ăn thất bại! Trả lại nguyên liệu cho người chơi.");

        // Hoàn nguyên liệu
        foreach (var material in recipe.materials)
        {
            InventoryManager.Instance.AddItem(material.item, material.quantity);
        }

        if (NotificationManager.Instance != null)
            NotificationManager.Instance.ShowNotification("Nấu ăn thất bại! Đã hoàn trả nguyên liệu.");
    }

}