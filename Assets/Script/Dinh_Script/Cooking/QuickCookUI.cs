using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Management; // Để gọi InventoryManager

public class QuickCookUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject panel; // Cái Panel cha
    [SerializeField] private Image resultIcon;
    [SerializeField] private TMP_Text resultName;
    [SerializeField] private Slider amountSlider;
    [SerializeField] private TMP_Text amountText; // Hiển thị "x1", "x5"...
    [SerializeField] private Button confirmButton;
    [SerializeField] private Button closeButton;

    private CookingRecipeSO currentRecipe;

    private void Start()
    {
        // Gắn sự kiện cho các nút và slider
        amountSlider.onValueChanged.AddListener(OnSliderValueChanged);
        confirmButton.onClick.AddListener(OnConfirmPressed);
        closeButton.onClick.AddListener(ClosePanel);

        panel.SetActive(false); // Ẩn mặc định
    }

    public void Show(CookingRecipeSO recipe)
    {
        currentRecipe = recipe;
        panel.SetActive(true);

        // Hiển thị thông tin
        resultIcon.sprite = recipe.resultItem.icon;
        resultName.text = recipe.dishName.GetLocalizedString();

        // 1. Tính toán số lượng tối đa có thể nấu
        int maxCraftable = CalculateMaxAmount();

        // 2. Setup Slider
        if (maxCraftable > 0)
        {
            amountSlider.interactable = true;
            amountSlider.minValue = 1;
            amountSlider.maxValue = maxCraftable;
            amountSlider.value = 1; // Mặc định là 1

            confirmButton.interactable = true;
            UpdateAmountText(1);
        }
        else
        {
            // Trường hợp lỗi (thực ra nút QuickCook bên ngoài đã chặn rồi, nhưng cứ check cho chắc)
            amountSlider.interactable = false;
            amountSlider.value = 0;
            confirmButton.interactable = false;
            amountText.text = "Thiếu nguyên liệu";
        }
    }

    private int CalculateMaxAmount()
    {
        int maxAmount = 9999; // Giả định ban đầu là vô hạn

        // Duyệt qua từng nguyên liệu để tìm "nút thắt cổ chai"
        foreach (var ing in currentRecipe.ingredients)
        {
            // Lấy số lượng đang có trong túi
            int currentStock = InventoryManager.Instance.GetTotalItemQuantity(ing.item);

            // Tính xem với nguyên liệu này thì nấu được bao nhiêu món
            // Ví dụ: Có 10 trứng, công thức cần 2 trứng -> nấu được 5
            int possibleAmount = currentStock / ing.quantity;

            // Lấy số nhỏ nhất
            if (possibleAmount < maxAmount)
            {
                maxAmount = possibleAmount;
            }
        }

        return maxAmount;
    }

    private void OnSliderValueChanged(float value)
    {
        UpdateAmountText((int)value);
    }

    private void UpdateAmountText(int value)
    {
        amountText.text = $"x{value}";
    }

    private void OnConfirmPressed()
    {
        int amount = (int)amountSlider.value;

        // Gọi Manager để trừ đồ và thêm món ăn
        CookingManager.Instance.ProcessQuickCook(currentRecipe, amount);

        ClosePanel();
    }

    public void ClosePanel()
    {
        panel.SetActive(false);
    }
}