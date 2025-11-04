// 
// Member: LinhH
// Date: 03/11/2025
// 

using System;
using System.Collections.Generic;
using UnityEngine;

public class SO_Quest : ScriptableObject
{
    [Header("Overview")]
    public string questID;
    public string title;
    public string description;
    public QuestReward reward;
}


/// <summary>
/// Nhiệm vụ thu thập vật phẩm.
/// </summary>
[CreateAssetMenu(fileName = "New Collection Quest", menuName = "Scriptable Object/Quest/Collection Quest")]
public class SO_CollectionQuest : SO_Quest
{
    [Header("Quest Details")]
    public List<QuestItemRequirement> targetItems;
}


[Serializable]
public class QuestItemRequirement
{
    public ItemScriptableObject item;
    public int amount;
}


/// <summary>
/// Nhiệm cụ trò chuyện với NPC.
/// </summary>
[CreateAssetMenu(fileName = "New Talking Quest", menuName = "Scriptable Object/Quest/Talking Quest")]
public class SO_TalkingQuest : SO_Quest
{
    [Header("Quest Details")]
    public SO_NPCData targetNPC;
}