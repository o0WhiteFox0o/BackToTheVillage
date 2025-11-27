using TMPro;
using UnityEngine;
using UnityEngine.UI;
using static CookingPanManager;

public class CookingRecipeButton : MonoBehaviour
{
    [SerializeField] private Image icon;
    [SerializeField] private Button selectButton; // Nút vô hình bao trùm cả prefab

    private CookingRecipeSO myRecipe;

    // Hàm này được gọi khi tạo danh sách
    public void Setup(CookingRecipeSO recipe)
    {
        myRecipe = recipe;

        icon.sprite = recipe.resultItem.icon;

        // Khi bấm vào nút này -> Gọi hàm SelectRecipe trong Manager
        selectButton.onClick.RemoveAllListeners();
        selectButton.onClick.AddListener(() => CookingManager.Instance.SelectRecipe(myRecipe));
    }
}