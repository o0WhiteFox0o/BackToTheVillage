// 
// Member: LinhH
// Date: 03/11/2025
// 


using System;
using System.Collections.Generic;
using UnityEngine;


[Serializable]
public class QuestReward
{
    // TODO: cập nhật thêm loại kinh nghiệm khi đã phát triển
    public int experience;
    public int gold;
    public List<ItemStack> itemRewards = new List<ItemStack>();
    public CraftingRecipeSO craftingRecipe;

    // TODO: thêm danh sách công thức nấu ăn
}


[Serializable]
public class ItemStack
{
    public ItemScriptableObject item;
    public int quantity;
}