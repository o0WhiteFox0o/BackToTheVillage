// 
// Member: LinhH
// Date: 03/11/2025
// 


using UnityEngine;

/// <summary>
/// Nhiệm cụ trò chuyện với NPC.
/// </summary>
[CreateAssetMenu(fileName = "New Talking Quest", menuName = "Scriptable Object/Quest/Talking Quest")]
public class SO_TalkingQuest : SO_Quest
{
    [Header("Quest Details")]
    public SO_NPCData targetNPC;
    public SO_ConversationData conversationData;
}