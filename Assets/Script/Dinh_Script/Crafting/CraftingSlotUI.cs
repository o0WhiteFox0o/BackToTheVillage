using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CraftingSlotUI : MonoBehaviour
{
    [SerializeField] private Image itemIcon;
    [SerializeField] private TMP_Text itemName;

    // Thêm tham chiếu tới Button để tắt tương tác nếu muốn (tùy chọn)
    // [SerializeField] private Button button; 

    private CraftingRecipeSO myRecipe;
    private CraftingUI parentUI;
    private Button myButton;

    private void Awake()
    {
        myButton = GetComponent<Button>();
        if (myButton != null)
            myButton.onClick.AddListener(OnButtonClicked);
    }

    public void Setup(CraftingRecipeSO recipe, CraftingUI ui)
    {
        myRecipe = recipe;
        parentUI = ui;

        if (itemIcon != null) itemIcon.sprite = recipe.itemToCraft.icon;
        if (itemName != null) itemName.text = recipe.itemToCraft.displayName.GetLocalizedString();


        // 1. Hỏi Manager xem có đủ đồ không
        bool canCraft = CraftingManager.Instance.CanCraft(recipe);

        // 2. Chỉnh màu Icon
        if (itemIcon != null)
        {
            Color c = itemIcon.color;

            if (canCraft)
            {
                // Đủ đồ: Màu rõ (Alpha = 1)
                c.a = 1f;
            }
            else
            {
                // Thiếu đồ: Màu mờ (Alpha = 0.5)
                c.a = 0.5f;
            }

            itemIcon.color = c;
        }

        // (Tùy chọn) Nếu muốn đổi màu Tên vật phẩm luôn cho dễ nhìn
        if (itemName != null)
        {
            itemName.color = canCraft ? Color.white : Color.gray;
        }
        // ==============================================
    }

    private void OnButtonClicked()
    {
        if (parentUI != null) parentUI.SelectRecipe(myRecipe);
    }
}