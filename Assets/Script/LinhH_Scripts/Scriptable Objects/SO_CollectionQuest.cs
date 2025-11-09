// 
// Member: LinhH
// Date: 03/11/2025
//


using System;
using System.Collections.Generic;
using UnityEngine;


/// <summary>
/// Nhiệm vụ thu thập vật phẩm.
/// </summary>
[CreateAssetMenu(fileName = "New Collection Quest", menuName = "Scriptable Object/Quest/Collection Quest")]
public class SO_CollectionQuest : SO_Quest
{
    [Header("Quest Details")]
    public List<QuestItemRequirement> targetItems_List;


    private void OnValidate() {
        questType = QuestType.Collection;
    }
}


[Serializable]
public class QuestItemRequirement
{
    public ItemScriptableObject item;
    public int currentQuantity = 0;
    public int requirementQuantity;
}