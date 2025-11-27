using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Management;
using TMPro; // Nhớ thêm thư viện này cho TextMeshPro

public class CookingManager : MonoBehaviour
{
    public static CookingManager Instance { get; private set; }

    [Header("Database")]
    public List<CookingRecipeSO> allRecipes;

    // Lưu trữ
    private List<CookingRecipeSO> discoveredList = new List<CookingRecipeSO>();
    private Dictionary<CookingRecipeSO, int> masteryCount = new Dictionary<CookingRecipeSO, int>();

    [Header("Minigames")]
    [SerializeField] private ChoppingMinigame choppingGame;
    // [SerializeField] private FryingMinigame fryingGame;

    [Header("UI Panels - Main")]
    [SerializeField] private GameObject mainPanel;
    [SerializeField] private QuickCookUI quickCookUI;
    [SerializeField] private NewDishPopupUI newDishUI;

    [Header("UI: Danh Sách (Left Panel)")]
    [SerializeField] private Transform recipeContainer;
    [SerializeField] private GameObject recipeButtonPrefab;

    [Header("UI: Chi Tiết (Right Panel)")]
    [SerializeField] private Image detailIcon;
    [SerializeField] private TMP_Text detailName;
    [SerializeField] private TMP_Text detailDesc;
    [SerializeField] private TMP_Text detailMastery;
    [SerializeField] private Button cookButton;      // Nút Nấu (Màu xanh lá)
    [SerializeField] private Button quickCookButton; // Nút Nấu Nhanh (Màu xanh dương)

    // Biến lưu món đang được chọn hiện tại
    private CookingRecipeSO selectedRecipe;

    private void Awake()
    {
        Instance = this;
        mainPanel.SetActive(false);
    }

    // --- MỞ BẢNG NẤU ĂN ---
    public void OpenCooking(CookingToolType tool)
    {
        mainPanel.SetActive(true);

        // 1. Xóa danh sách cũ
        foreach (Transform child in recipeContainer) Destroy(child.gameObject);

        // 2. Tạo nút mới (Chỉ để chọn, không có logic nấu ở đây)
        foreach (var recipe in allRecipes)
        {
            if (recipe.requiredTool == tool)
            {
                GameObject btn = Instantiate(recipeButtonPrefab, recipeContainer);
                // Gọi hàm Setup mới (xem phần 2 bên dưới)
                btn.GetComponent<CookingRecipeButton>().Setup(recipe);
            }
        }

        // 3. Reset vùng chi tiết (Chưa chọn gì)
        ResetDetailView();
    }

    private void ResetDetailView()
    {
        selectedRecipe = null;
        detailIcon.gameObject.SetActive(false);
        detailName.text = "Chọn một công thức...";
        detailDesc.text = "";
        detailMastery.text = "";

        // Tắt các nút nấu
        cookButton.interactable = false;
        quickCookButton.gameObject.SetActive(false);
    }

    public void CloseMainPanel() => mainPanel.SetActive(false);

    // --- HÀM CHỌN MÓN (Được gọi khi bấm vào nút trong danh sách) ---
    public void SelectRecipe(CookingRecipeSO recipe)
    {
        selectedRecipe = recipe;

        // 1. Cập nhật thông tin hiển thị
        detailIcon.gameObject.SetActive(true);
        detailIcon.sprite = recipe.resultItem.icon;
        detailName.text = recipe.dishName.GetLocalizedString();
        detailDesc.text = recipe.resultItem.itemDescription.GetLocalizedString(); 

        // 2. Setup nút Nấu (Minigame)
        cookButton.interactable = true;
        cookButton.onClick.RemoveAllListeners(); // Xóa sự kiện cũ
        cookButton.onClick.AddListener(() => StartMinigameCooking(recipe));

        // 3. Setup nút Nấu Nhanh (Quick Cook)
        int currentCount = GetCookCount(recipe);
        bool isUnlocked = currentCount >= recipe.masteryThreshold;

        quickCookButton.gameObject.SetActive(isUnlocked); // Chỉ hiện khi mở khóa

        if (isUnlocked)
        {
            quickCookButton.onClick.RemoveAllListeners();
            quickCookButton.onClick.AddListener(() => OpenQuickCook(recipe));
            detailMastery.text = "<color=green>Đã thành thạo!</color>";
        }
        else
        {
            detailMastery.text = $"Thành thạo: {currentCount}/{recipe.masteryThreshold}";
        }
    }

    // --- CÁC HÀM LOGIC CŨ (GIỮ NGUYÊN) ---

    public void StartMinigameCooking(CookingRecipeSO recipe)
    {
        if (!CheckIngredients(recipe, 1))
        {
            Debug.Log("Thiếu nguyên liệu!");
            return;
        }

        mainPanel.SetActive(false);

        BaseCookingMinigame game = null;
        switch (recipe.minigameType)
        {
            case CookingMinigameType.Chopping: game = choppingGame; break;
        }

        if (game != null)
        {
            game.OnWin = () => OnCookSuccess(recipe);
            game.OnLose = () => {
                Debug.Log("Thất bại!");
                mainPanel.SetActive(true);
            };
            game.StartMinigame(recipe.difficulty, recipe.timeLimit);
        }
    }

    private void OnCookSuccess(CookingRecipeSO recipe)
    {
        ConsumeIngredients(recipe, 1);
        InventoryManager.Instance.AddItem(recipe.resultItem, 1);

        if (!masteryCount.ContainsKey(recipe)) masteryCount[recipe] = 0;
        masteryCount[recipe]++;

        if (!discoveredList.Contains(recipe))
        {
            discoveredList.Add(recipe);
            newDishUI.Show(recipe);
        }
        else
        {
            // Nếu không phải món mới, mở lại bảng chính để nấu tiếp
            mainPanel.SetActive(true);
            // Chọn lại món vừa nấu để cập nhật số lượng Mastery
            SelectRecipe(recipe);
        }
    }

    public void OpenQuickCook(CookingRecipeSO recipe) => quickCookUI.Show(recipe);

    public void ProcessQuickCook(CookingRecipeSO recipe, int amount)
    {
        ConsumeIngredients(recipe, amount);
        InventoryManager.Instance.AddItem(recipe.resultItem, amount);

        // Sau khi nấu nhanh xong, cập nhật lại giao diện
        mainPanel.SetActive(true);
        SelectRecipe(recipe);
    }

    public int GetCookCount(CookingRecipeSO r) => masteryCount.ContainsKey(r) ? masteryCount[r] : 0;

    private bool CheckIngredients(CookingRecipeSO r, int multiplier)
    {
        foreach (var ing in r.ingredients)
        {
            if (InventoryManager.Instance.GetTotalItemQuantity(ing.item) < ing.quantity * multiplier)
                return false;
        }
        return true;
    }

    private void ConsumeIngredients(CookingRecipeSO r, int multiplier)
    {
        foreach (var ing in r.ingredients)
        {
            InventoryManager.Instance.RemoveItem(ing.item, ing.quantity * multiplier);
        }
    }
}