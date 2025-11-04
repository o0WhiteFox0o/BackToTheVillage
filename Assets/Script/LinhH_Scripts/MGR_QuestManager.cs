// 
// Member: LinhH
// Date: 03/11/2025
// 


using System.Collections.Generic;
using UnityEngine;

public class MGR_QuestManager : MonoBehaviour
{
    public static MGR_QuestManager Instance;

    private List<QuestState> activeQuests = new List<QuestState>();

    /// <summary>
    /// Kích hoạt khi một mục tiêu của nhiệm vụ được cập nhật.
    /// </summary>
    public delegate void QuestUpdatedHandler(string targetID);


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }
    }


    public void AddQuest(SO_Quest newQuest)
    {
        // nếu nhiệm vụ đã tồn tại trong danh sách nhiệm vụ thì không thêm nó vào nữa
        if (activeQuests.Exists(q => q.quest == newQuest)) { return; }

        var quest = new QuestState { quest = newQuest, isActive = true };
    }


    public void UpdateObjective(string targetID)
    {
       // kiệm tra loại nhiệm vụ cần cập nhật

    }


    public void RemoveQuest(SO_Quest quest)
    {
        var removedQuest = activeQuests.Find(q => q.quest = quest);

        if (removedQuest != null)
        {
            activeQuests.Remove(removedQuest);
        }
    }
}
