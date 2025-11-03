using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[System.Serializable]
public class TrapLootItem
{
    public ItemScriptableObject item;
    [Range(0f, 100f)]
    public float dropChance;
}
[CreateAssetMenu(fileName = "New Trap Loot Table", menuName = "Loot/Trap Loot Table")]
public class TrapLootTable : ScriptableObject
{
    [Tooltip("Vật phẩm có thể rớt.")]
    public TrapLootItem[] lootItems;
    public ItemScriptableObject PickUpLoot()
    {
       float totalChance = 0f;
        foreach (var item in lootItems) { 
            totalChance += item.dropChance;
        }
        float randamChance = Random.Range(0f, totalChance);
        float cumulativeChance = 0f;

        foreach (var item in lootItems)
        {
            cumulativeChance += item.dropChance;
            if (randamChance <= cumulativeChance)
            {
                return item.item;
            }
        }
        return null;
    }
}
