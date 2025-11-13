// 
// Member: LinhH
// Date: 03/11/2025
// 


using System;
using System.Collections.Generic;
using System.Linq;
using Management;
using UnityEngine;

public class MGR_QuestManager : MonoBehaviour
{
    public List<IQuestProgress> activeQuests_List { get; private set; }


    /// <summary>
    /// Được gọi khi một nhiệm vụ được thêm vào hoặc loại bỏ khỏi danh sách nhiệm vụ.
    /// </summary>
    public static event Action OnQuestListUpdate;


    private void Start()
    {
        activeQuests_List = new List<IQuestProgress>();

        // đăng ký sự kiện cần thiết
        InventoryManager.OnCollectItem += UpdateCollectionProgress;
        MGR_ConversationManager.OnStartConversation += UpdateTalkingProgress;
    }


    private void OnDisable()
    {
        InventoryManager.OnCollectItem -= UpdateCollectionProgress;
        MGR_ConversationManager.OnStartConversation -= UpdateTalkingProgress;
    }


    /// <summary>
    /// Thêm nhiệm vụ mới vào danh sách nhiệm vụ.
    /// </summary>
    public void AddQuest(SO_Quest newQuest)
    {
        // nếu nhiệm vụ đã tồn tại trong danh sách nhiệm vụ thì không thêm nó vào nữa
        if (activeQuests_List.Exists(q => q.GetQuest().questID == newQuest.questID)) { return; }

        var addedQuest = QuestProgressFactory.CreateQuestProgress(newQuest);
        activeQuests_List.Add(addedQuest);

        OnQuestListUpdate?.Invoke();
    }


    private void UpdateCollectionProgress(ItemScriptableObject item, int quantity)
    {
        // nếu không có nhiệm vụ nào trong danh sách thì dừng cập nhật
        if (activeQuests_List.Count == 0) { return; }

        foreach (var quest in activeQuests_List)
        {
            // nếu nhiệm vụ được duyệt là nhiệm vụ thu thập thì kiểm tra cập nhật nó
            if (quest is CollectionQuestProgress collectionQuest)
            {
                collectionQuest.UpdateProgress(item, quantity);
            }
        }

        RefreshQuestList();
    }


    private void UpdateTalkingProgress(SO_NPCData npc)
    {
        // nếu không có nhiệm vụ nào trong danh sách thì dừng cập nhật
        if (activeQuests_List.Count == 0) { return; }

        foreach (var quest in activeQuests_List)
        {
            // nếu nhiệm vụ được duyệt là nhiệm vụ thu thập thì kiểm tra cập nhật nó
            if (quest is TalkingQuestProgress talkingQuest)
            {
                talkingQuest.UpdateProgress(npc);
            }
        }

        RefreshQuestList();
    }


    /// <summary>
    /// Kiểm tra nếu có nhiệm vụ nào đã hoàn thành thì loại nó khỏi danh sách.
    /// </summary>
    private void RefreshQuestList()
    {
        var completedQuest = activeQuests_List.FirstOrDefault(q => q.IsComplete() == true);
        if (completedQuest != null)
        {
            // kiểm tra xem có nhiệm vụ nào trong chuỗi nhiệm vụ không
            if (completedQuest.GetQuest().nextQuest != null)
            {
                AddQuest(completedQuest.GetQuest().nextQuest);
            }

            // loại bỏ nhiệm vụ khỏi danh sách
            activeQuests_List.Remove(completedQuest);
            OnQuestListUpdate?.Invoke();
        }
    }
}


public enum QuestType
{
    Collection,
    Talking,
    Giving,
    Selling
}