// 
// Member: LinhH
// Date: 11/11/2025
// 


using UnityEngine;

public class MGR_GameplayManager : MonoBehaviour
{
    private MGR_QuestManager questManager;


    private void Start()
    {
        questManager = FindObjectOfType<MGR_QuestManager>();

        if (questManager == null)
        {
            Debug.LogError("Can't load a manager component");
        }

        LoadGame();

        DontDestroyOnLoad(this);
    }


    public void LoadGame()
    {
        // TEST - LinhH - 11/11/2025
        // TODO - get farm name and load as the file name
        var savedGame = SavedGameConfig.LoadSaveGameConfig("SavedGameTest1");

        questManager.LoadFromSavedGame(savedGame);
    }


    public void SaveGame()
    {
        SavedGameConfig savedGameConfig = new SavedGameConfig();

        // lưu danh sách các nhiệm vụ đang kích hoạt vào saved game config
        foreach (var quest in questManager.activeQuests_List)
        {
            switch (quest.GetQuest().questType)
            {
                case QuestType.Collection:
                    // thêm dữ liệu của nhiệm vụ thu thập vào danh sách nhiệm vụ 
                    savedGameConfig.activeQuest_List.Add(
                        new QuestData
                        {
                            questId = quest.GetQuest().questId,
                            questType = (int)quest.GetQuest().questType,
                            questJsonData = CollectionQuestJson(quest)
                        }
                    );
                    break;

                case QuestType.Talking:
                    // thêm dữ liệu của nhiệm vụ trò chuyện vào danh sách nhiệm vụ 
                    savedGameConfig.activeQuest_List.Add(
                        new QuestData
                        {
                            questId = quest.GetQuest().questId,
                            questType = (int)quest.GetQuest().questType,
                            questJsonData = ""
                        }
                    );
                    break;

                case QuestType.Giving:
                    break;

                case QuestType.Selling:
                    break;

                default:
                    break;
            }
        }

        // TEST - LinhH - 11/11/2025
        // TODO - get farm name and save as the file name
        savedGameConfig.SaveGameConfig("SavedGameTest1");
    }


    private string CollectionQuestJson(IQuestProgress questProgress)
    {
        // tạo một biến lưu trữ dữ liệu tiến trình dưới dạng object
        CollectionQuestData collectionQuestData = new CollectionQuestData();

        if (questProgress is CollectionQuestProgress collectionQuestProgress)
        {
            // lưu trữ dữ liệu của từng item trong danh sách thu thập
            foreach (var collectedItem in collectionQuestProgress.itemRequirements_List)
            {
                // thêm dữ liệu của item vào danh sách
                collectionQuestData.collectedItem_List.Add(
                    new ItemCollectedData
                    {
                        itemId = collectedItem.item.id,
                        currentQuantity = collectedItem.currentQuantity,
                        totalQuantity = collectedItem.requirementQuantity
                    }
                );
            }
        }

        string jsonData = JsonUtility.ToJson(collectionQuestData);
        return jsonData;
    }
}
