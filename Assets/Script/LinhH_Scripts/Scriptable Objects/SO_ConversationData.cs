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
    [Header("Overview")]
    public string conversationId;

    /// <summary>
    /// Danh sách câu thoại của từng NPC trong cuộc hội thoại.
    /// </summary>
    public List<NPCDialogue> dialogue_List;

    /// <summary>
    /// Đánh dấu đoạn hội thoại chỉ dùng một lần (e.g. các đoạn hội thoại giao nhiệm vụ, ...)
    /// </summary>
    [Tooltip(" Đánh dấu đoạn hội thoại chỉ dùng một lần (e.g. các đoạn hội thoại giao nhiệm vụ, ...)")]
    public bool oneTimeConversation;


    [Header("Quest")]

    /// <summary>
    /// Nhiệm vụ cho nhân vật chính (nếu có).
    /// </summary>
    [Tooltip("Nhiệm vụ được giao trong cuộc hội thoại (nếu có).")]
    public SO_Quest quest;


    [Header("Decision")]

    /// <summary>
    /// Vị trí câu thoại hiển thị các lựa chọn trong cuộc hội thoại.
    /// </summary>
    [Tooltip("Vị trí của câu thoại hiển thị các lựa chọn trong cuộc hội thoại.")]
    public int decisionIndex = -1;

    /// <summary>
    /// Danh sách các lựa chọn được hiển thị.
    /// </summary>
    public List<SO_Decision> decision_List;
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