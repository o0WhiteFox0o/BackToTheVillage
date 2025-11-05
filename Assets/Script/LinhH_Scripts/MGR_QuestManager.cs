// 
// Member: LinhH
// Date: 03/11/2025
// 


using System.Collections.Generic;
using System.Linq;
using Management;
using UnityEngine;

public class MGR_QuestManager : MonoBehaviour
{
    public static MGR_QuestManager Instance;

    private List<IQuestProgress> activeQuests_List = new List<IQuestProgress>();

    /// <summary>
    /// Kích hoạt khi một vật phẩm được thu thập. Dùng để cập nhật nhiệm vụ thu thập.
    /// </summary>
    public delegate void CollectionQuestUpdatedHandler(ItemScriptableObject updatedItem, int quantity);


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }


        InventoryManager.OnCollectItem += UpdateCollectionProgress;
    }


    private void OnDisable()
    {
        InventoryManager.OnCollectItem -= UpdateCollectionProgress;
    }


    public void AddQuest(SO_Quest newQuest)
    {
        // nếu nhiệm vụ đã tồn tại trong danh sách nhiệm vụ thì không thêm nó vào nữa
        if (activeQuests_List.Exists(q => q.GetQuest().questID == newQuest.questID)) { return; }

        var addedQuest = QuestProgressFactory.CreateQuestProgress(newQuest);
        activeQuests_List.Add(addedQuest);
    }


    private void UpdateCollectionProgress(ItemScriptableObject item, int quantity)
    {
        // nếu không có nhiệm vụ nào trong danh sách thì dừng cập nhật
        if (activeQuests_List.Count == 0) { return; }

        Debug.Log($"Danh sách nhiệm vụ đang có {activeQuests_List.Count} nhiệm vụ");

        foreach (var quest in activeQuests_List)
        {
            // nếu nhiệm vụ được duyệt là nhiệm vụ thu thập thì kiểm tra cập nhật nó
            if (quest is CollectionQuestProgress collectionQuest)
            {
                collectionQuest.UpdateProgress(item, quantity);
            }
        }

        // kiểm tra nếu có nhiệm vụ nào đã hoàn thành thì loại nó khỏi danh sách 
        var completedQuest = activeQuests_List.FirstOrDefault(q => q.IsComplete() == true);
        if (completedQuest != null)
        {
            activeQuests_List.Remove(completedQuest);
        }
    }


    private void UpdateTalkingProgress(SO_NPCData npc)
    {
        // nếu không có nhiệm vụ nào trong danh sách thì dừng cập nhật
        if (activeQuests_List.Count == 0) { return; }

    }


    // public void RemoveQuest(SO_Quest quest)
    // {
    //     var removedQuest = activeQuests_List.FirstOrDefault(q => q.GetQuest().questID == quest.questID);
    //     if (removedQuest != null)
    //     {
    //         // loại bỏ nhiệm vụ ra khỏi danh sách nhiệm vụ
    //         activeQuests_List.Remove(removedQuest);

    //         // nếu không có nhiệm vụ nào tiếp theo (trong chuỗi nhiệm vụ) thì bỏ qua
    //         if (removedQuest.GetQuest().nextQuest == null) { return; }
    //         AddQuest(removedQuest.GetQuest().nextQuest);

    //         Debug.Log($"Loại nhiệm vụ {quest.title} ra khỏi danh sách nhiệm vụ.");
    //     }
    // }
}


public enum QuestType
{
    Collection,
    Talking,
    Giving
}