// 
// Member: LinhH
// Date: 06/11/2025
// 


using TMPro;
using UnityEngine;

public class C_DecisionController : MonoBehaviour
{
    [SerializeField] public TMP_Text decision_Text;

    private MGR_ConversationManager conversationManager;
    private MGR_QuestManager questManager;
    private UI_GameplayUIManager gameplayUIManager;

    private SO_Decision decisionData;


    private void Start() {
        conversationManager = FindObjectOfType<MGR_ConversationManager>();
        questManager = FindObjectOfType<MGR_QuestManager>();
        gameplayUIManager = FindObjectOfType<UI_GameplayUIManager>();

        if (conversationManager == null || questManager == null || gameplayUIManager == null)
        {
            Debug.LogError("Can't get a manager component.");
        }
    }


    public void SetupDecisionUI(SO_Decision decision)
    {
        decisionData = decision;

        decision_Text.SetText(decisionData.dialogue.GetLocalizedString());
    }


    public void ImplementDecision()
    {
        if (decisionData is SO_ConversationDecision conversationDecision)
        {
            conversationManager.ContinueConversation(conversationDecision.conversationData);
        }
        else if (decisionData is SO_GetQuestDecision questDecision)
        {
            questManager.AddQuest(questDecision.quest);
            conversationManager.PlayNextLine();
        }
        else if (decisionData is SO_OpenUIDecision uiDecision)
        {

        }
        else if (decisionData is SO_ReceiveGiftDecision giftDecision)
        {

        }

        gameplayUIManager.conversationUIManager.ToggleSkipDialogueButton(true);
    }
}