// 
// Member: LinhH
// Date: 06/11/2025
// 


using TMPro;
using UnityEngine;

public class C_DecisionController : MonoBehaviour
{
    [SerializeField] public TMP_Text decision_Text;

    private SO_Decision decisionData;


    public void SetupDecisionUI(SO_Decision decision)
    {
        decisionData = decision;

        decision_Text.SetText(decisionData.dialogue.GetLocalizedString());
    }


    public void ImplementDecision()
    {
        if (decisionData is SO_ConversationDecision conversationDecision)
        {

        }
        else if (decisionData is SO_GetQuestDecision questDecision)
        {

        }
        else if (decisionData is SO_OpenUIDecision uiDecision)
        {

        }
        else if (decisionData is SO_ReceiveGiftDecision giftDecision)
        {

        }
    }
}