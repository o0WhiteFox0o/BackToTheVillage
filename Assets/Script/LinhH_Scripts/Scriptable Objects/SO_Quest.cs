// 
// Member: LinhH
// Date: 03/11/2025
// 


using UnityEngine;

public class SO_Quest : ScriptableObject
{
    [Header("Overview")]
    public string questID;
    public string tittle;
    public string description;

    /// <summary>
    /// Phân loại nhiệm vụ. E.g. Story quest, Event quest, ...
    /// </summary>
    public QuestCategorize questCategorize;

    [HideInInspector] public QuestType questType;

    [Header("Reward")]
    public QuestReward reward;

    [Header("Next Quest")]

    /// <summary>
    /// Nhiệm vụ tiếp theo trong chuỗi nhiệm vụ (nếu có).
    /// </summary>
    public SO_Quest nextQuest;
}


public enum QuestType
{
    Collection,
    Talking,
    Giving,
    Selling
}


public enum QuestCategorize
{
    StoryQuest,
    EventQuest,
    CompanionQuest,
    Other
}