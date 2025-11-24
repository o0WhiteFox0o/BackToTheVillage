// 
// Member   : Linh
// Date     : 31/10/2025
// 

using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class C_NPCController : MonoBehaviour
{
    [SerializeField] public SO_NPCData npcData;
    [SerializeField] public List<SO_ConversationData> conversation_List;

    public SO_ConversationData priorityConversation { get; private set; }


    private void Start()
    {
        // ----- TEST -----
        priorityConversation = conversation_List[0];

        MGR_ConversationManager.OnConversationEnd += UpdateConversationList;
    }


    private void OnDisable() {
        MGR_ConversationManager.OnConversationEnd -= UpdateConversationList;
    }


    private void UpdateConversationList(SO_ConversationData conversationData)
    {
        var removedConversation = conversation_List.FirstOrDefault(c => c.conversationId == conversationData.conversationId);

        // nếu trong danh sách hội thoại không có đoạn hội thoại được truyền vào thì dừng lại
        if (removedConversation == null) { return; }

        // nếu đoạn hội thoại được truyền vào không phải là hội thoại một lần thì dừng lại
        if (!removedConversation.oneTimeConversation) { return; }

        conversation_List.Remove(removedConversation);

        UpdatePriorityConversation();
    }


    private void UpdatePriorityConversation()
    {
    }
}
