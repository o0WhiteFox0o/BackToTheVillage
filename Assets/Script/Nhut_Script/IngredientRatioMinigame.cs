using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class IngredientRatioMinigame : MonoBehaviour
{
    [Header("UI References")]
    public Transform rowParent;              // Nơi chứa các dòng nguyên liệu
    public GameObject rowPrefab;            // Prefab gồm: Name, Percent, -, +
    public Button btnCook;                  // Nút "Nấu!"

    [Header("Data")]
    private Dictionary<string, int> ratios = new Dictionary<string, int>();
    private Dictionary<string, int> targetRatios = new Dictionary<string, int>();

    private List<RowItem> rowItems = new List<RowItem>();

    public System.Action<bool> onFinish;

    // Struct giúp quản lý 1 dòng nguyên liệu
    private class RowItem
    {
        public string ingredient;
        public TextMeshProUGUI txtName;
        public TextMeshProUGUI txtPercent;
        public Button btnMinus;
        public Button btnPlus;
    }

    // ---------------------------------------------------
    // Gọi khi bắt đầu mở minigame
    // ---------------------------------------------------
    public void StartMinigame(Dictionary<string, int> correctRatios)
    {
        gameObject.SetActive(true);

        // Reset
        ClearRows();
        rowItems.Clear();
        ratios.Clear();
        targetRatios.Clear();

        // Gán target ratio
        foreach (var kv in correctRatios)
        {
            targetRatios[kv.Key] = kv.Value;
            ratios[kv.Key] = kv.Value; // Cho người chơi bắt đầu từ tỉ lệ đúng
        }

        // Tạo dòng UI
        foreach (var ing in ratios.Keys)
        {
            CreateRow(ing);
        }

        RefreshUI();

        btnCook.onClick.RemoveAllListeners();
        btnCook.onClick.AddListener(ValidateResult);
    }

    // ---------------------------------------------------
    // Tạo 1 dòng UI
    // ---------------------------------------------------
    void CreateRow(string ingredientName)
    {
        GameObject row = Instantiate(rowPrefab, rowParent);
        RowItem item = new RowItem();

        item.ingredient = ingredientName;
        item.txtName = row.transform.Find("TxtIngredientName").GetComponent<TextMeshProUGUI>();
        item.txtPercent = row.transform.Find("TxtPercent").GetComponent<TextMeshProUGUI>();
        item.btnMinus = row.transform.Find("BtnMinus").GetComponent<Button>();
        item.btnPlus = row.transform.Find("BtnPlus").GetComponent<Button>();

        item.txtName.text = ingredientName;

        // Gán sự kiện nút
        item.btnMinus.onClick.AddListener(() => ChangeValue(ingredientName, -10));
        item.btnPlus.onClick.AddListener(() => ChangeValue(ingredientName, +10));

        rowItems.Add(item);
    }

    // ---------------------------------------------------
    // Tăng/giảm giá trị nguyên liệu
    // ---------------------------------------------------
    void ChangeValue(string key, int amount)
    {
        int total = GetTotal();

        // Nếu tăng nhưng tổng đã 100%
        if (amount > 0 && total >= 100)
            return;

        // Nếu giảm nhưng nguyên liệu đang từ 0
        if (amount < 0 && ratios[key] <= 0)
            return;

        // Giới hạn max 100%
        int newValue = ratios[key] + amount;
        if (newValue < 0 || newValue > 100)
            return;

        ratios[key] = newValue;

        // Nếu tổng vượt 100, tự động giảm các nguyên liệu khác
        NormalizeTo100();

        RefreshUI();
    }

    // ---------------------------------------------------
    // Giảm các nguyên liệu khác để đảm bảo tổng = 100%
    // ---------------------------------------------------
    void NormalizeTo100()
    {
        int total = GetTotal();
        if (total <= 100) return;

        int excess = total - 100;

        foreach (var key in ratios.Keys)
        {
            if (excess <= 0) break;

            if (ratios[key] > 0)
            {
                int reduce = Mathf.Min(10, excess);
                ratios[key] -= reduce;
                excess -= reduce;
            }
        }
    }

    // ---------------------------------------------------
    // Cập nhật UI
    // ---------------------------------------------------
    void RefreshUI()
    {
        foreach (var item in rowItems)
        {
            item.txtPercent.text = ratios[item.ingredient] + "%";
        }
    }

    // ---------------------------------------------------
    // Kiểm tra kết quả
    // ---------------------------------------------------
    void ValidateResult()
    {
        foreach (var key in targetRatios.Keys)
        {
            if (ratios[key] != targetRatios[key])
            {
                Debug.Log("❌ Sai tỷ lệ nguyên liệu!");

                // Gợi ý thông minh
                string hint = GenerateHint();
                Debug.Log("💡 Gợi ý: " + hint);

                onFinish?.Invoke(false);
                return;
            }
        }

        Debug.Log("🔥 Chính xác! Tỷ lệ hoàn hảo!");
        onFinish?.Invoke(true);
    }

    // ---------------------------------------------------
    // Gợi ý thông minh cho người chơi
    // ---------------------------------------------------
    string GenerateHint()
    {
        foreach (var key in targetRatios.Keys)
        {
            int cur = ratios[key];
            int want = targetRatios[key];

            if (cur < want)
                return $"{key} đang quá ít, hãy tăng thêm!";

            if (cur > want)
                return $"{key} đang quá nhiều, hãy giảm xuống!";
        }

        return "Hãy cân chỉnh lại một chút!";
    }

    // ---------------------------------------------------
    int GetTotal()
    {
        int total = 0;
        foreach (var val in ratios.Values)
            total += val;
        return total;
    }

    // ---------------------------------------------------
    void ClearRows()
    {
        foreach (Transform t in rowParent)
            Destroy(t.gameObject);
    }
}
