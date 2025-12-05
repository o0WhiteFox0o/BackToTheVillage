using UnityEngine;
using UnityEngine.UI;
using TMPro;

// Gắn script này vào GameObject CraftingPanel (Cửa sổ chế tạo chính)
public class CraftingUI : MonoBehaviour
{
    [Header("Components Chính")]
    [SerializeField] private GameObject craftingPanel;
    [SerializeField] private Button craftButton;
    [SerializeField] private Button closeButton; // Nút đóng UI

    [Header("Danh sách Công thức (Bên trái)")]
    [SerializeField] private Transform recipeListContainer;
    [SerializeField] private GameObject recipeSlotPrefab; // Prefab của 1 ô công thức

    [Header("Chi tiết Công thức (Bên phải)")]
    [SerializeField] private Image selectedItemIcon;
    [SerializeField] private TMP_Text selectedItemName;
    [SerializeField] private TMP_Text selectedItemDesc;
    [SerializeField] private Transform materialsContainer;
    [SerializeField] private GameObject materialSlotPrefab; // Prefab của 1 ô nguyên liệu

    private CraftingRecipeSO selectedRecipe;

    void Start()
    {
        craftButton.onClick.AddListener(OnCraftButtonPressed);
        closeButton.onClick.AddListener(ClosePanel);

        // Lắng nghe Manager
        CraftingManager.Instance.OnRecipeUnlocked += OnNewRecipeUnlocked;

        craftingPanel.SetActive(false); // Ẩn lúc bắt đầu

        if(Management.InventoryManager.Instance != null)
        {
            Management.InventoryManager.Instance.OnInventoryChanged += OnInventoryUpdated;
        }
    }
    private void OnDestroy()
    {
        if (Management.InventoryManager.Instance != null)
        {
            Management.InventoryManager.Instance.OnInventoryChanged -= OnInventoryUpdated;
        }
    }

    public void TogglePanel()
    {
        bool isActive = !craftingPanel.activeSelf;
        craftingPanel.SetActive(isActive);

        if (isActive)
        {
            RefreshRecipeList(); // Tải danh sách công thức khi mở
            SelectRecipe(null); // Xóa chi tiết công thức cũ
        }
    }

    private void ClosePanel()
    {
        craftingPanel.SetActive(false);
    }

    // Tải lại toàn bộ danh sách công thức đã mở khóa
    public void RefreshRecipeList()
    {
        // Xóa danh sách cũ
        foreach (Transform child in recipeListContainer)
        {
            Destroy(child.gameObject);
        }

        // Tạo danh sách mới
        foreach (var recipe in CraftingManager.Instance.unlockedRecipes)
        {
            GameObject slotGO = Instantiate(recipeSlotPrefab, recipeListContainer);
            // Gắn script cho ô prefab (xem File 3)
            slotGO.GetComponent<CraftingSlotUI>().Setup(recipe, this);
        }
    }

    // (Hàm này được gọi bởi CraftingSlotUI khi bạn click vào 1 công thức)
    public void SelectRecipe(CraftingRecipeSO recipe)
    {
        selectedRecipe = recipe;
        UpdateDetailPanel();
    }

    // Cập nhật thông tin ở Panel bên phải
    private void UpdateDetailPanel()
    {
        if (selectedRecipe == null)
        {
            // Nếu không chọn gì, ẩn hết
            selectedItemIcon.gameObject.SetActive(false);
            selectedItemName.text = "";
            selectedItemDesc.text = "...";
            craftButton.gameObject.SetActive(false);
            foreach (Transform child in materialsContainer) Destroy(child.gameObject);
            return;
        }

        // Hiển thị thông tin
        selectedItemIcon.gameObject.SetActive(true);
        selectedItemIcon.sprite = selectedRecipe.itemToCraft.icon;
        selectedItemName.text = selectedRecipe.itemToCraft.displayName.GetLocalizedString(); 
        selectedItemDesc.text = selectedRecipe.itemToCraft.itemDescription.GetLocalizedString(); 
        craftButton.gameObject.SetActive(true);

        // Hiển thị nguyên liệu
        foreach (Transform child in materialsContainer) Destroy(child.gameObject);
        foreach (var material in selectedRecipe.materials)
        {
            GameObject matSlotGO = Instantiate(materialSlotPrefab, materialsContainer);
            // (Giả sử Prefab nguyên liệu có Image và Text)
            matSlotGO.GetComponent<Image>().sprite = material.item.icon;
            matSlotGO.GetComponentInChildren<TMP_Text>().text = material.quantity.ToString();
        }

        // Kiểm tra xem có chế được không để bật/tắt nút
        craftButton.interactable = CraftingManager.Instance.CanCraft(selectedRecipe);
    }

    // Bấm nút "Chế tạo"
    private void OnCraftButtonPressed()
    {
        if (selectedRecipe == null) return;

        // Gọi "Bộ não" để chế
        CraftingManager.Instance.Craft(selectedRecipe);

        // Cập nhật lại nút
        UpdateDetailPanel();
        RefreshRecipeList();
    }

    // Khi học công thức mới lúc đang mở cửa sổ
    private void OnNewRecipeUnlocked(CraftingRecipeSO recipe)
    {
        if (craftingPanel.activeSelf)
        {
            RefreshRecipeList();
        }
    }

    private void OnInventoryUpdated()
    {
        // Chỉ cập nhật nếu bảng đang mở (để tiết kiệm hiệu năng)
        if (craftingPanel.activeSelf)
        {
            // Vẽ lại danh sách bên trái (để cập nhật mờ/rõ)
            RefreshRecipeList();

            if (selectedRecipe != null)
            {
                UpdateDetailPanel();
            }
        }
    }
}