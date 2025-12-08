// 
// Member: LinhH
// Date: 03/11/2025
// 


using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;


public abstract class QuestProgress
{
    public static event Action OnQuestComplete;


    public void CompleteQuest()
    {
        OnQuestComplete?.Invoke();
    }


    public abstract void CheckProgress();

    // Lấy dữ liệu cơ bản của nhiệm vụ từ Quest Progress.
    public abstract SO_Quest GetQuest();
    public abstract bool IsComplete();
    public abstract bool IsClaimed();
}


public class CollectionQuestProgress : QuestProgress
{
    public SO_CollectionQuest quest;
    public bool isActive;
    public bool isCompleted = false;
    private bool isClaimed = false;
    public List<QuestItemRequirement> itemRequirements_List = new List<QuestItemRequirement>();

    public delegate void UpdateCollectionQuestHandler(CollectionQuestProgress collectionProgress);

    // Được gọi khi tiến trình của nhiệm vụ thu thập được cập nhật.
    public static event UpdateCollectionQuestHandler OnCollectionQuestUpdate;


    /// <summary>
    /// Tạo một tiến trình nhiệm vụ thu thập mới từ nhiệm vụ được truyền vào.
    /// </summary>
    public CollectionQuestProgress(SO_CollectionQuest collectionQuest)
    {
        quest = collectionQuest;
        isActive = true;

        // tạo một danh sách lưu trữ và cập nhật các vật phẩm được yêu cầu
        itemRequirements_List.Clear();
        foreach (var targetItem in collectionQuest.targetItems_List)
        {
            var newTargetItem = new QuestItemRequirement { item_SO = targetItem.item_SO, currentQuantity = 0, requirementQuantity = targetItem.requirementQuantity };
            itemRequirements_List.Add(newTargetItem);
        }
    }


    /// <summary>
    /// Tạo một tiến trình nhiệm vụ thu thập cho các nhiệm vụ được load từ file (không phải là một tiến trình mới).
    /// </summary>
    public CollectionQuestProgress(string questId, List<ItemCollectedData> targetItems_List)
    {
        // load nhiệm vụ từ Resources
        var collectionQuest_List = Resources.LoadAll<SO_Quest>("Quests");
        var resourceQuest = collectionQuest_List.FirstOrDefault(q => q.questId == questId);

        // nếu nhiệm vụ được load là nhiệm vụ thu thập thì gán nó vào quest
        if (resourceQuest is SO_CollectionQuest collectionQuest) { quest = collectionQuest; }
        else { return; }

        // load danh sách item có trong Resources
        var itemSO_List = Resources.LoadAll<ItemScriptableObject>("Items");

        // thiết lập tiến trình của từng vật phẩm trong nhiệm vụ
        itemRequirements_List.Clear();
        foreach (var itemProgress in targetItems_List)
        {
            // load vật phẩm tương ứng với vật phẩm được lưu trong file
            var requirementItem = itemSO_List.FirstOrDefault(i => i.id == itemProgress.itemId);

            if (requirementItem == null) { continue; }

            // thêm tiến trình của vật phẩm vào danh sách vật phẩm yêu cầu
            itemRequirements_List.Add(new QuestItemRequirement
            {
                item_SO = requirementItem,
                currentQuantity = itemProgress.currentQuantity,
                requirementQuantity = itemProgress.totalQuantity
            });
        }

        isActive = true;
    }


    /// <summary>
    /// Cập nhật tiến trình cho nhiệm vụ thu thập vật phẩm.
    /// </summary>
    public void UpdateProgress(ItemScriptableObject updatedItem, int updatedQuantity)
    {
        // nếu vật phẩm không có trong danh sách cần thu thập thì dừng
        if (!itemRequirements_List.Exists(i => i.item_SO.id == updatedItem.id)) { return; }

        // cập nhật số lượng vật phẩm cho nhiệm vụ
        var requirementItem = itemRequirements_List.FirstOrDefault(i => i.item_SO.id == updatedItem.id);
        if (requirementItem != null)
        {
            requirementItem.currentQuantity += updatedQuantity;
            OnCollectionQuestUpdate?.Invoke(this);
            Debug.Log("Cập nhật số lượng cho " + requirementItem.item_SO);
        }

        CheckProgress();
    }


    /// <summary>
    /// Kiểm tra tiến trình của nhiệm vụ thu thập.
    /// </summary>
    public override void CheckProgress()
    {
        // nếu có bất kỳ vật phẩm nào trong danh sách chưa thu thập đủ thì không làm gì
        if (itemRequirements_List.Exists(i => i.currentQuantity < i.requirementQuantity)) { return; }

        // hoàn thành nhiệm vụ
        isCompleted = true;
        isActive = false;

        CompleteQuest();
    }


    public override SO_Quest GetQuest() => quest;
    public override bool IsComplete() => isCompleted;
    public override bool IsClaimed() => isClaimed;
}


public class TalkingQuestProgress : QuestProgress
{
    public SO_TalkingQuest quest;
    public bool isActive;
    public bool isCompleted;
    private bool isClaimed = false;


    public void UpdateProgress(SO_NPCData npc)
    {
        Debug.Log("Cập nhật nhiệm vụ trò chuyện");

        // nếu không phải npc cần trò chuyện thì dừng cập nhật
        if (quest.targetNPC.npcId != npc.npcId) { return; }

        CheckProgress();
    }


    public override void CheckProgress()
    {
        CompleteQuest();

        isCompleted = true;
        isActive = false;
    }


    public override SO_Quest GetQuest() => quest;
    public override bool IsComplete() => isCompleted;
    public override bool IsClaimed() => isClaimed;
}