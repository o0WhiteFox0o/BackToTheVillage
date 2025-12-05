using TMPro;
using UnityEngine;
using UnityEngine.Localization.Settings;
using UnityEngine.UI;

public class CookingUI : MonoBehaviour
{
    [Header("Components")]
    [SerializeField] private GameObject cookingPanel;
    [SerializeField] private Button cookButton;
    [SerializeField] private TMP_Text cookButtonText;
    [SerializeField] private Button closeButton;

    [Header("Recipe List")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeSlotPrefab;

    [Header("Details")]
    [SerializeField] private Image selectedIcon;
    [SerializeField] private TMP_Text selectedName;
    [SerializeField] private TMP_Text masteryText;
    [SerializeField] private Transform materialsContainer;
    [SerializeField] private GameObject materialSlotPrefab;

    private CookingRecipeSO selectedRecipe;
    private Player playerMovement;
    [SerializeField] private MinigameController controller;

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
            if (Player.Instance != null) Player.Instance.SetInputActive(true);
            if (controller != null) controller.ForceStop();
        });

        // 🔥 Đăng ký lắng nghe sự kiện thay đổi Inventory — cập nhật UI ngay lập tức
        if (Management.InventoryManager.Instance != null)
        {
            Management.InventoryManager.Instance.OnInventoryChanged += RefreshUIInstant;
        }

        cookingPanel.SetActive(false);
    }

    private void OnDestroy()
    {
        if (Management.InventoryManager.Instance != null)
        {
            Management.InventoryManager.Instance.OnInventoryChanged -= RefreshUIInstant;
        }
    }

    private void RefreshUIInstant()
    {
        if (!cookingPanel.activeSelf) return;

        RefreshRecipeList();

        if (selectedRecipe != null)
            UpdateDetailPanel();
    }


    public void OnEnable()
    {
        if (playerMovement != null) playerMovement.SetInputActive(false);
    }
    public void OnDisable()
    {
        if (playerMovement != null) playerMovement.SetInputActive(true);
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

        foreach (var recipe in CookingManager.Instance.unlockedCookingRecipes)
        {
            GameObject slot = Instantiate(recipeSlotPrefab, recipeListContainer);

            slot.GetComponent<CraftingSlotUI>().Setup(recipe, null);

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
            selectedName.text = "";
            masteryText.text = "";
            cookButtonText.text = "";
            cookButton.gameObject.SetActive(false);

            foreach (Transform child in materialsContainer) Destroy(child.gameObject);
            return;
        }

        // Icon + tên
        selectedIcon.gameObject.SetActive(true);
        selectedIcon.sprite = selectedRecipe.itemToCraft.icon;
        selectedName.text = selectedRecipe.itemToCraft.displayName.GetLocalizedString();
        cookButton.gameObject.SetActive(true);

        // Nguyên liệu
        foreach (Transform child in materialsContainer) Destroy(child.gameObject);
        foreach (var mat in selectedRecipe.materials)
        {
            GameObject mSlot = Instantiate(materialSlotPrefab, materialsContainer);
            mSlot.GetComponent<Image>().sprite = mat.item.icon;
            mSlot.GetComponentInChildren<TMP_Text>().text = mat.quantity.ToString();
        }

        // Mastery
        bool mastered = CookingManager.Instance.IsMastered(selectedRecipe);
        int cur = CookingManager.Instance.GetMasteryCount(selectedRecipe);
        int max = selectedRecipe.masteryThreshold;

        if (mastered)
        {
            cookButtonText.text = GetLocalizedString("gMsg.quickcookbutton");
            masteryText.text = GetLocalizedString("gMsg.mastered");
            masteryText.color = Color.yellow;
        }
        else
        {
            masteryText.text = GetLocalizedString("gMsg.mastery") + $" {cur}/{max}";
            masteryText.color = Color.white;
        }

        // Trạng thái nút
        cookButton.interactable = CraftingManager.Instance.CanCraft(selectedRecipe);
    }

    private void OnCookButtonClicked()
    {
        if (selectedRecipe == null) return;

        CookingManager.Instance.RequestCook(selectedRecipe, () =>
        {
            RefreshUIInstant();
        });
    }

    public string GetLocalizedString(string key)
    {
        return LocalizationSettings.StringDatabase.GetLocalizedString("GameplayMessage", key);
    }
}
