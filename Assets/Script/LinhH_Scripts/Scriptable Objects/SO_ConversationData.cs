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
    /// Nhiệm vụ cho nhân vật chính (nếu có).
    /// </summary>
    public SO_Quest quest;

    /// <summary>
    /// Vị trí câu thoại hiển thị các lựa chọn trong cuộc hội thoại.
    /// </summary>
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