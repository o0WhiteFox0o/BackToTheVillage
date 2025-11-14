using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.EventSystems;

// Gắn script này vào Prefab của 1 ô công thức (RecipeSlotPrefab)
public class CraftingSlotUI : MonoBehaviour, IPointerClickHandler
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;

    private CraftingRecipeSO myRecipe;
    private CraftingUI parentUI; // (Để gọi lại Cửa sổ chính)

    // Gán thông tin từ Cửa sổ chính
    public void Setup(CraftingRecipeSO recipe, CraftingUI ui)
    {
        myRecipe = recipe;
        parentUI = ui;

        itemIcon.sprite = recipe.itemToCraft.icon;
        itemName.text = recipe.itemToCraft.displayName.GetLocalizedString();
    }

    // Khi người chơi click vào ô này
    public void OnPointerClick(PointerEventData eventData)
    {
        // Báo cho Cửa sổ chính: "Tôi đã được chọn!"
        parentUI.SelectRecipe(myRecipe);
    }
}