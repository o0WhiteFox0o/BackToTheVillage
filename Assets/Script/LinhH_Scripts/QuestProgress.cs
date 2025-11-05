// 
// Member: LinhH
// Date: 03/11/2025
// 


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public interface IQuestProgress
{
    public void CheckProgress();
    public SO_Quest GetQuest();
    public bool IsComplete();
}


public class CollectionQuestProgress : IQuestProgress
{
    public SO_CollectionQuest quest;
    public bool isActive;
    public bool isCompleted = false;
    public List<QuestItemRequirement> itemRequirements_List = new List<QuestItemRequirement>();


    public delegate void UpdateCollectionQuestHandler(CollectionQuestProgress collectionProgress);
    /// <summary>
    /// Được gọi khi tiến trình của nhiệm vụ thu thập được cập nhật.
    /// </summary>
    public static event UpdateCollectionQuestHandler OnCollectionQuestUpdate;


    public CollectionQuestProgress(SO_CollectionQuest collectionQuest)
    {
        quest = collectionQuest;
        isActive = true;

        // tạo một danh sách lưu trữ và cập nhật các vật phẩm được yêu cầu
        itemRequirements_List.Clear();
        foreach (var targetItem in collectionQuest.targetItems_List)
        {
            var newTargetItem = new QuestItemRequirement { item = targetItem.item, currentQuantity = 0, requirementQuantity = targetItem.requirementQuantity };
            itemRequirements_List.Add(newTargetItem);
        }
    }


    /// <summary>
    /// Cập nhật tiến trình cho nhiệm vụ thu thập.
    /// </summary>
    public void UpdateProgress(ItemScriptableObject updatedItem, int updatedQuantity)
    {
        // nếu vật phẩm không có trong danh sách cần thu thập thì dừng
        if (!itemRequirements_List.Exists(i => i.item == updatedItem)) { return; }

        // cập nhật số lượng vật phẩm cho nhiệm vụ
        var requirementItem = itemRequirements_List.FirstOrDefault(i => i.item == updatedItem);
        if (requirementItem != null)
        {
            requirementItem.currentQuantity += updatedQuantity;
            OnCollectionQuestUpdate?.Invoke(this);
            Debug.Log("Cập nhật số lượng cho " + requirementItem.item);
        }


        CheckProgress();
    }


    public void CheckProgress()
    {
        // nếu có bất kỳ vật phẩm nào trong danh sách chưa thu thập đủ thì không làm gì
        if (itemRequirements_List.Exists(i => i.currentQuantity < i.requirementQuantity)) { return; }

        // hoàn thành nhiệm vụ
        isCompleted = true;
        isActive = false;

        // trao thưởng cho nhân vật
        quest.reward?.GrantReward();
    }


    public SO_Quest GetQuest() => quest;

    public bool IsComplete() => isCompleted;
}


public class TalkingQuestProgress : IQuestProgress
{
    public SO_TalkingQuest quest;
    public bool isActive;
    public bool isCompleted;


    public void UpdateProgress(SO_NPCData targetNpc)
    {
        Debug.Log("Cập nhật nhiệm vụ trò chuyện");
    }


    public void CheckProgress()
    {
        isCompleted = true;
        isActive = false;

        quest.reward?.GrantReward();

        Debug.Log("Hoàn thành nhiệm vụ trò chuyện");
    }


    public SO_Quest GetQuest() => quest;

    public bool IsComplete() => isCompleted;
}