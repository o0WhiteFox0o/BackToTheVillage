// 
// Member: LinhH
// Date: 06/11/2025
// 


using UnityEngine;


/// <summary>
/// Quản lý các thông báo trong gameplay.
/// </summary>
public class MGR_GameplayNotification : MonoBehaviour
{
    private GameplayUIManager gameplayUIManager;
    private MGR_QuestManager questManager;

    private int questAmount = 0;


    private void Start()
    {
        gameplayUIManager = FindObjectOfType<GameplayUIManager>();
        questManager = FindObjectOfType<MGR_QuestManager>();

        if (gameplayUIManager == null || questManager == null)
        {
            Debug.LogError("Can't load a manager component.");
        }

        MGR_QuestManager.OnQuestListUpdate += UpdateQuestNotification;
    }
    

    private void OnDisable() {
        MGR_QuestManager.OnQuestListUpdate -= UpdateQuestNotification;
    }


    /// <summary>
    /// Cập nhật thông báo khi người chơi nhận nhiệm vụ mới.
    /// </summary>
    private void UpdateQuestNotification()
    {
        // nếu có nhiệm vụ nào bị loại bỏ khỏi danh sách thì không cần hiển thị thông báo
        Debug.Log($"current quest {questAmount}, quest in list {questManager.activeQuests_List.Count}");
        if (questAmount >= questManager.activeQuests_List.Count)
        {
            questAmount = questManager.activeQuests_List.Count;
            // gameplayUIManager.DisableQuestNotification();
            return;
        }

        // bật thông báo trên giao diện gameplay
        // gameplayUIManager.EnableQuestNotification();
    }
}
