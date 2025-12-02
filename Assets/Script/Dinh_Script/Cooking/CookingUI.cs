using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject cookingPanel;
    [SerializeField] private Button cookButton; // Nút này sẽ đổi text
    [SerializeField] private TMP_Text cookButtonText; // Text của nút: "Cook" hoặc "Quick Cook"
    [SerializeField] private Button closeButton;

    [Header("Recipe List")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeSlotPrefab;

    [Header("Details")]
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TMP_Text selectedName;
    [SerializeField] private TMP_Text masteryText; // Hiển thị: "Thành thạo: 3/10"
    [SerializeField] private Transform materialsContainer;
    [SerializeField] private GameObject materialSlotPrefab;

    private CookingRecipeSO selectedRecipe;
    private Player playerMovement;

    private void Awake()
    {
        playerMovement = GetComponent<Player>();
    }
    private void Start()
    {
        cookButton.onClick.AddListener(OnCookButtonClicked);
        closeButton.onClick.AddListener(() =>
        {
            cookingPanel.SetActive(false);

            if (Player.Instance != null)
                Player.Instance.SetInputActive(true);
        });
        cookingPanel.SetActive(false);
    }

    public void OnEnable()
    {
        if (playerMovement != null) {
            playerMovement.SetInputActive(false);
        }
    }
    public void OnDisable()
    {
        if (playerMovement != null)
        {
            playerMovement.SetInputActive(true);
        }
    }

    public void TogglePanel()
    {
        bool isActive = !cookingPanel.activeSelf;
        cookingPanel.SetActive(isActive);
        if (isActive)
        {
            RefreshRecipeList();
            SelectRecipe(null);
        }
        Player.Instance.SetInputActive(!isActive);
    }


    public void RefreshRecipeList()
    {
        foreach (Transform child in recipeListContainer) Destroy(child.gameObject);

        // Chỉ lấy những recipe là CookingRecipeSO
        foreach (var recipe in CookingManager.Instance.unlockedCookingRecipes)
        {
            GameObject slot = Instantiate(recipeSlotPrefab, recipeListContainer);
            // Tận dụng lại CraftingSlotUI hoặc tạo CookingSlotUI riêng nếu muốn hiển thị khác biệt
            // Ở đây ta ép kiểu (cast) về CraftingRecipeSO để dùng lại slot cũ cho tiện
            slot.GetComponent<CraftingSlotUI>().Setup(recipe, null);

            // Lưu ý: CraftingSlotUI cần sửa lại chút ở hàm OnButtonClicked để gọi về CookingUI
            // Hoặc đơn giản là gán sự kiện click thủ công ở đây:
            slot.GetComponent<Button>().onClick.RemoveAllListeners();
            slot.GetComponent<Button>().onClick.AddListener(() => SelectRecipe(recipe));
        }
    }

    public void SelectRecipe(CookingRecipeSO recipe)
    {
        selectedRecipe = recipe;
        UpdateDetailPanel();
    }

    private void UpdateDetailPanel()
    {
        if (selectedRecipe == null)
        {
            selectedIcon.gameObject.SetActive(false);
            masteryText.text = "";
            cookButton.gameObject.SetActive(false);
            foreach (Transform child in materialsContainer) Destroy(child.gameObject);
            return;
        }

        // Hiện thông tin cơ bản
        selectedIcon.gameObject.SetActive(true);
        selectedIcon.sprite = selectedRecipe.itemToCraft.icon;
        selectedName.text = selectedRecipe.itemToCraft.displayName.GetLocalizedString();
        cookButton.gameObject.SetActive(true);

        // Hiện nguyên liệu
        foreach (Transform child in materialsContainer) Destroy(child.gameObject);
        foreach (var mat in selectedRecipe.materials)
        {
            GameObject mSlot = Instantiate(materialSlotPrefab, materialsContainer);
            mSlot.GetComponent<Image>().sprite = mat.item.icon;
            mSlot.GetComponentInChildren<TMP_Text>().text = mat.quantity.ToString();
        }

        // --- LOGIC MASTERY ---
        bool isMastered = CookingManager.Instance.IsMastered(selectedRecipe);
        int currentMastery = CookingManager.Instance.GetMasteryCount(selectedRecipe);
        int maxMastery = selectedRecipe.masteryThreshold;

        if (isMastered)
        {
            cookButtonText.text = "Nấu Nhanh (Auto)";
            masteryText.text = $"Thành thạo: TỐI ĐA";
            masteryText.color = Color.yellow;
        }
        else
        {
            cookButtonText.text = "Nấu";
            masteryText.text = $"Thành thạo: {currentMastery}/{maxMastery}";
            masteryText.color = Color.white;
        }

        // Kiểm tra đủ nguyên liệu để active nút
        cookButton.interactable = CraftingManager.Instance.CanCraft(selectedRecipe);
    }

    private void OnCookButtonClicked()
    {
        if (selectedRecipe == null) return;

        CookingManager.Instance.RequestCook(selectedRecipe, () =>
        {
            // Callback sau khi nấu xong (hoặc thất bại)
            UpdateDetailPanel();
            RefreshRecipeList(); // Để cập nhật lại màu sắc nếu hết nguyên liệu
        });
    }
}