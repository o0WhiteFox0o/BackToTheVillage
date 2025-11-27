using UnityEngine;
using System.Collections.Generic;

[CreateAssetMenu(fileName = "New Recipe", menuName = "Scriptable Object/Crafting/Recipe")]
public class CraftingRecipeSO : ItemScriptableObject
{
    [Header("Crafting Details")]
    [Tooltip("Vật phẩm sẽ được tạo ra")]
    public ItemScriptableObject itemToCraft;
    public int quantityCreated = 1;

    [Tooltip("Danh sách nguyên liệu cần thiết")]
    public List<MaterialCost> materials;

    

    // Ghi đè để nó không phải là item bình thường
    private void OnValidate()
    {
        itemType = ItemType.CraftingRecipe; 
        stackable = false;
        canSell = false;
    }
}
[System.Serializable]
    public class MaterialCost
    {
        public ItemScriptableObject item;
        public int quantity;
    }