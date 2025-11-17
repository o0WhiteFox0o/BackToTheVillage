using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class CookingPanManager : MonoBehaviour
{
    [Header("UI References")]
    public Transform addedIngredientParent;
    public GameObject ingredientIconPrefab;
    public Image resultImage;
    public GameObject resultPanel;
    public TextMeshProUGUI resultText;
    public Slider cookingSlider;

    [Header("Minigame")]
    public FireCookingMinigame fireMinigame;

    [Header("Data")]
    public List<string> currentIngredients = new List<string>();

    [System.Serializable]
    public class Recipe
    {
        public string dishName;
        public Sprite dishIcon;
        public List<string> ingredients;
        public float cookTime = 5f;
        public FireCookingMinigame.DishType difficulty;
    }

    public Recipe[] recipes;

    // -----------------------------
    // Thêm nguyên liệu
    // -----------------------------
    public void AddIngredient(IngredientListManager.IngredientData ing)
    {
        if (ingredientIconPrefab == null || addedIngredientParent == null)
        {
            Debug.LogWarning("Thiếu prefab hoặc vùng chứa icon nguyên liệu!");
            return;
        }

        // Tạo icon
        GameObject icon = Instantiate(ingredientIconPrefab, addedIngredientParent);
        Image iconImg = icon.GetComponent<Image>();
        iconImg.sprite = ing.icon;

        RectTransform iconRect = icon.GetComponent<RectTransform>();

        // Bắt đầu ở vị trí cao hơn chảo (rơi xuống)
        iconRect.anchoredPosition = new Vector2(Random.Range(-200f, 200f), 500f);

        // Tạo vị trí mục tiêu (ở giữa chảo, lệch nhẹ ngẫu nhiên)
        Vector2 targetPos = new Vector2(
            Random.Range(-30f, 30f),
            Random.Range(-15f, 15f)
        );

        // Thêm hiệu ứng rơi (tween)
        StartCoroutine(DropToPan(iconRect, targetPos));

        currentIngredients.Add(ing.name);
        Debug.Log($"🧂 Đã thêm nguyên liệu: {ing.name}");
    }

    IEnumerator DropToPan(RectTransform iconRect, Vector2 target)
    {
        float duration = 0.6f;
        float elapsed = 0f;
        Vector2 startPos = iconRect.anchoredPosition;

        // Hiệu ứng rơi (EaseOutBounce)
        while (elapsed < duration)
        {
            elapsed += Time.deltaTime;
            float t = elapsed / duration;
            t = Mathf.Sin(t * Mathf.PI * 0.5f); // Ease-out

            iconRect.anchoredPosition = Vector2.Lerp(startPos, target, t);
            yield return null;
        }

        iconRect.anchoredPosition = target;
    }

    // -----------------------------
    // Bắt đầu nấu
    // -----------------------------
    public void StartCooking()
    {
        if (currentIngredients.Count == 0)
        {
            Debug.LogWarning("Chưa chọn nguyên liệu nào!");
            return;
        }

        Recipe matchedRecipe = null;
        foreach (var recipe in recipes)
        {
            if (MatchRecipe(recipe))
            {
                matchedRecipe = recipe;
                break;
            }
        }

        if (matchedRecipe != null)
        {
            if (fireMinigame != null)
                StartCoroutine(StartFireMinigame(matchedRecipe));
            else
                Debug.LogError("fireMinigame chưa được gán trong Inspector!");
        }
        else
        {
            ShowResult("Sai công thức!", null);
            ResetIngredients();
        }
    }

    // -----------------------------
    // Bắt đầu minigame canh lửa
    // -----------------------------
    IEnumerator StartFireMinigame(Recipe recipe)
    {
        if (fireMinigame == null)
        {
            Debug.LogError("fireMinigame chưa được gán!");
            yield break;
        }

        fireMinigame.gameObject.SetActive(true);
        fireMinigame.SetupDishDifficulty(recipe.difficulty);

        bool? success = null;
        fireMinigame.onFinish = (bool result) => success = result;

        // Chờ kết quả minigame
        yield return new WaitUntil(() => success.HasValue);

        fireMinigame.gameObject.SetActive(false);

        if (success.Value)
        {
            Debug.Log("Minigame thành công → Bắt đầu nấu...");
            StartCoroutine(CookingProcess(recipe));
        }
        else
        {
            Debug.Log("Thất bại khi canh lửa!");
            ShowResult("Nấu thất bại do canh lửa sai!", null);
            ResetIngredients();
        }
    }

    // -----------------------------
    // Quá trình nấu chính
    // -----------------------------
    private IEnumerator CookingProcess(Recipe recipe)
    {
        if (cookingSlider != null)
        {
            cookingSlider.gameObject.SetActive(true);
            cookingSlider.value = 0f;
        }

        float elapsed = 0f;
        float cookTime = recipe.cookTime;

        while (elapsed < cookTime)
        {
            elapsed += Time.deltaTime;
            if (cookingSlider != null)
                cookingSlider.value = Mathf.Clamp01(elapsed / cookTime);
            yield return null;
        }

        if (cookingSlider != null)
        {
            cookingSlider.value = 1f;
            cookingSlider.gameObject.SetActive(false);
        }

        ShowResult($"{recipe.dishName}", recipe.dishIcon);
        Debug.Log($"Nấu xong món: {recipe.dishName}");

        ResetIngredients();
    }

    // -----------------------------
    // So khớp công thức
    // -----------------------------
    bool MatchRecipe(Recipe recipe)
    {
        if (recipe.ingredients.Count != currentIngredients.Count) return false;

        foreach (var ing in recipe.ingredients)
        {
            if (!currentIngredients.Contains(ing)) return false;
        }
        return true;
    }

    // -----------------------------
    // Hiển thị kết quả
    // -----------------------------
    void ShowResult(string message, Sprite icon)
    {
        if (resultPanel != null)
            resultPanel.SetActive(true);

        if (resultText != null)
            resultText.text = message;

        if (resultImage != null)
            resultImage.sprite = icon;
    }

    // -----------------------------
    // Reset nguyên liệu
    // -----------------------------
    void ResetIngredients()
    {
        currentIngredients.Clear();

        if (addedIngredientParent != null)
        {
            foreach (Transform t in addedIngredientParent)
            {
                if (t != null)
                    Destroy(t.gameObject);
            }
        }

        Debug.Log("Đã reset nguyên liệu trong chảo.");
    }
}
