using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public enum CookingToolType
{
    CuttingBoard,
    FryingPan,
    Pot
}
public enum CookingMinigameType
{
    Chopping,
    Frying,
    Sequence
}
[CreateAssetMenu(fileName = "NewCookingRecipe", menuName = "Cooking/CookingRecipe")]
public class CookingRecipeSO : ScriptableObject
{
   public LocalizedString dishName;
   public LocalizedString description;
   public ItemScriptableObject resultItem; // Thành Phẩm

   [Header("Yêu cầu")]
   public CookingToolType requiredTool;// Dụng cụ nấu ăn
   public List<MaterialCost> ingredients; // Nguyên liệu

   [Header("Minigame")]
   public CookingMinigameType minigameType;
   [Range(1, 10)] public float difficulty = 1f;
   public float timeLimit = 10f;

   [Header("Thành thạo")]
   public int masteryThreshold = 5; // Thành thạo công thức
}
