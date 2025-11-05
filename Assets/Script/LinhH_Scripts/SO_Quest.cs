// 
// Member: LinhH
// Date: 03/11/2025
// 


using UnityEngine;

public class SO_Quest : ScriptableObject
{
    [Header("Overview")]
    public string questID;
    public string title;
    public string description;
    public QuestType questType;
    public QuestReward reward;

    /// <summary>
    /// Nhiệm vụ tiếp theo trong chuỗi nhiệm vụ (nếu có).
    /// </summary>
    public SO_Quest nextQuest;
}