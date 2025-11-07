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
    public QuestType questType;

    [Header("Reward")]

    /// <summary>
    /// Có hiển thị phần thưởng trong Quest UI không.
    /// </summary>
    public bool displayRewardInUI;
    public QuestReward reward;

    [Header("Next Quest")]

    /// <summary>
    /// Nhiệm vụ tiếp theo trong chuỗi nhiệm vụ (nếu có).
    /// </summary>
    public SO_Quest nextQuest;
}