// 
// Member: LinhH
// Date: 06/11/2025
// 


using UnityEngine;


[CreateAssetMenu(fileName = "New Conversation Decision", menuName = "Scriptable Object/Decision/Conversation Decision")]
public class SO_ConversationDecision : SO_Decision
{
    public SO_ConversationData conversationData;

    private void OnValidate() {
        decisionType = DecisionType.Conversation;
    }
}
