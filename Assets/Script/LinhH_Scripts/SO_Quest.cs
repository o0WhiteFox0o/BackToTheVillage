// 
// Member: LinhH
// Date: 03/11/2025
// 

using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Quest", menuName = "Quest")]
public class SO_Quest : ScriptableObject
{
    public string questID;
    public string title;
    public string description;
    public List<QuestObjective> objectives;
    public QuestReward reward;
}


[Serializable]
public class QuestObjective
{
    public string description;
    public ObjectiveType type;

    // ví dụ: item id, ...
    public string targetID;
    public int requiredAmount = 1;
    public int currentAmount = 0;

    public bool IsComplete => currentAmount >= requiredAmount;
}


public class QuestReward
{
    
}


public enum ObjectiveType
{
    Collect,
    TalkTo,
    Explore
}