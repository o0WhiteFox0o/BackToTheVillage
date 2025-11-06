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

    private int questAmount = 0;


    private void Start()
    {
        gameplayUIManager = FindObjectOfType<GameplayUIManager>();
        if (gameplayUIManager == null)
        {
            Debug.LogError("Can't load gameplay UI manager.");
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
        if (questAmount > MGR_QuestManager.Instance.activeQuests_List.Count)
        {
            questAmount = MGR_QuestManager.Instance.activeQuests_List.Count;
            gameplayUIManager.DisableQuestNotification();
            return;
        }

        // bật thông báo trên giao diện gameplay
        gameplayUIManager.EnableQuestNotification();
    }
}
