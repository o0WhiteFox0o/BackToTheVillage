using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "New Fishing Rod", menuName = "Scriptable Object/Item/Fishing Rod")]
public class FishingRodSO : ItemScriptableObject
{
    [Header("Fishing Rod Settings")]
    public bool canUseBait;

    private void OnEnable()
    {
        itemType = ItemType.FishingRod;
        stackable = false;
    }

}
