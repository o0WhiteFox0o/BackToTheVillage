// 
// Member: LinhH
// Date: 03/11/2025
// 


using System;
using System.Collections.Generic;
using System.Linq;
using Management;
using UnityEngine;
using UnityEngine.Localization;

public class MGR_QuestManager : MonoBehaviour
{
    public List<IQuestProgress> activeQuests_List { get; private set; }

    private InventoryManager inventoryManager;


    /// <summary>
    /// Được gọi khi một nhiệm vụ được thêm vào hoặc loại bỏ khỏi danh sách nhiệm vụ.
    /// </summary>
    public static event Action OnQuestListUpdate;


    private void Awake()
    {
        activeQuests_List = new List<IQuestProgress>();
    }


    private void Start()
    {
        inventoryManager = transform.parent.GetComponentInChildren<InventoryManager>();

        if (inventoryManager == null)
        {
            Debug.LogError("Can't load a component!!!");
        }

        // đăng ký sự kiện cần thiết
        InventoryManager.OnCollectItem += UpdateCollectionProgress;
        MGR_ConversationManager.OnStartConversation += UpdateTalkingProgress;
    }


    private void OnDisable()
    {
        InventoryManager.OnCollectItem -= UpdateCollectionProgress;
        MGR_ConversationManager.OnStartConversation -= UpdateTalkingProgress;
    }


    public void LoadFromSavedGame(SavedGameConfig savedGame)
    {
        Debug.Log($"total quest in saved file: {savedGame.activeQuest_List.Count}");

        // load danh sách các nhiệm vụ được lưu
        foreach (var questData in savedGame.activeQuest_List)
        {
            switch (questData.questType)
            {
                case (int)QuestType.Collection:
                    LoadCollectionProgress(questData);
                    break;

                case (int)QuestType.Talking:
                    LoadTalkingProgress(questData);
                    break;

                case (int)QuestType.Giving:
                    break;

                case (int)QuestType.Selling:
                    break;

                default:
                    break;
            }
        }
    }



    /// <summary>
    /// Thêm nhiệm vụ mới vào danh sách nhiệm vụ.
    /// </summary>
    public void AddQuest(SO_Quest newQuest)
    {
        // nếu nhiệm vụ đã tồn tại trong danh sách nhiệm vụ thì không thêm nó vào nữa
        if (activeQuests_List.Exists(q => q.GetQuest().questId == newQuest.questId)) { return; }

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
        // tìm nhiệm vụ đã nhận phần thưởng trong danh sách
        var claimedQuest = activeQuests_List.FirstOrDefault(q => q.IsClaimed() == true);
        if (claimedQuest != null)
        {
            // kiểm tra xem có nhiệm vụ nào trong chuỗi nhiệm vụ không
            if (claimedQuest.GetQuest().nextQuest != null)
            {
                AddQuest(claimedQuest.GetQuest().nextQuest);
            }

            // loại bỏ nhiệm vụ khỏi danh sách
            activeQuests_List.Remove(claimedQuest);
            OnQuestListUpdate?.Invoke();

            // hiển thị thông báo quest UI
        }
    }


    private void LoadCollectionProgress(QuestData questData)
    {
        // load danh sách tiến trình của vật phẩm
        var itemProgress = JsonUtility.FromJson<CollectionQuestData>(questData.questJsonData);

        // nếu danh sách tiến trình rỗng thì không làm gì cả
        if (itemProgress == null || itemProgress.collectedItem_List.Count == 0) { return; }

        activeQuests_List.Add(new CollectionQuestProgress(questData.questId, itemProgress.collectedItem_List));
    }


    private void LoadTalkingProgress(QuestData questData)
    {
        // load nhiệm vụ có cùng id trong Resource
        var quest_List = Resources.LoadAll<SO_Quest>("Quests");
        var quest = quest_List.FirstOrDefault(q => q.questId == questData.questId);

        // thêm nhiệm vụ vừa được load vào danh sách nhiệm vụ
        AddQuest(quest);
    }


    /// <summary>
    /// Phát thưởng cho nhân vật.
    /// </summary>
    public void GrantReward(IQuestProgress quest)
    {
        if (!activeQuests_List.Contains(quest)) { return; }

        // TODO: Hook into player inventory or stats system

        var questReward = quest.GetQuest().reward;

        // thêm vật phẩm thưởng vào inventory của nhân vật
        if (questReward.itemRewards.Count != 0)
        {
            foreach (var itemReward in questReward.itemRewards)
            {
                inventoryManager.AddItem(itemReward.item, itemReward.quantity);
            }
        }

        // thêm công thức thưởng cho nhân vật
        if (questReward.craftingRecipe != null)
        {
            CraftingManager.Instance.UnLockRecipe(questReward.craftingRecipe);
        }

        activeQuests_List.Remove(quest);
        RefreshQuestList();
    }
}