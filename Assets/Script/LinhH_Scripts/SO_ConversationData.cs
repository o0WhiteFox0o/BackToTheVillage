// 
// Member   : Linh
// Date     : 30/10/2025
// 

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;


/// <summary>
/// Scriptable object chứa dữ liệu của cuộc hội thoại giữa người chơi và NPC.
/// </summary>
[Serializable]
[CreateAssetMenu(fileName = "New Conversation", menuName = "Scriptable Object/NPC/NPC Conversation")]
public class SO_ConversationData : ScriptableObject
{
    public string conversationId;

    /// <summary>
    /// Danh sách câu thoại của từng NPC trong cuộc hội thoại.
    /// </summary>
    public List<NPCDialogue> dialogue_List;

    /// <summary>
    /// Đánh dấu đây có phải là đoạn hội thoại nằm trong một tuyến cốt truyện hay không.
    /// </summary>
    public bool isStoryConversation;


    /// <summary>
    /// Nhiệm vụ cho nhân vật chính (nếu có).
    /// </summary>
    public SO_Quest quest;
}


/// <summary>
/// Chứa câu lệnh đơn của một nhân vật.
/// </summary>
[Serializable]
public class NPCDialogue
{
    public SO_NPCData npcData;
    public LocalizedString dialogue;
}