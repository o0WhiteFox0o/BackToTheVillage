using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class IngredientListManager : MonoBehaviour
{
    public CookingPanManager panManager;

    [System.Serializable]
    public class IngredientData
    {
        public string name;
        public Sprite icon;
    }

    public IngredientData[] ingredients;

    // 🔹 Hàm trung gian để gọi từ OnClick trong Editor
    public void OnIngredientClick(string ingredientName)
    {
        IngredientData selected = System.Array.Find(ingredients, i => i.name == ingredientName);
        if (selected != null)
        {
            Debug.Log("Đã chọn nguyên liệu: " + selected.name);
            panManager.AddIngredient(selected);
        }
        else
        {
            Debug.LogWarning("Không tìm thấy nguyên liệu: " + ingredientName);
        }
    }
}
