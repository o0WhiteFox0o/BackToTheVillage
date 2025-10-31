// 
// Member   : Linh
// Date     : 31/10/2025
// 

using System.Collections.Generic;
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
    }
}
