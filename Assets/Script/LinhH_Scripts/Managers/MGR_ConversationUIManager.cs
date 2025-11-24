// 
// Member: LinhH
// Date: 24/11/2025
// 


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class MGR_ConversationUIManager : MonoBehaviour
{
    [SerializeField] public GameObject conversationPanel;

    [SerializeField] public TMP_Text npcName_Text;
    [SerializeField] public Image npcPortrait_Image;
    [SerializeField] public TMP_Text conversationDisplay_Text;
    [SerializeField] public Transform decisionPanel;
    [SerializeField] public Button skipButton;

    private GameObject decisionButton_Prefab;


    private void Start()
    {
        decisionButton_Prefab = Resources.Load<GameObject>("Prefabs/UI/PFB_DecisionButton");

        var conversationManger = FindObjectOfType<MGR_ConversationManager>();

        if (decisionButton_Prefab == null || conversationManger == null)
        {
            Debug.LogError("Can't load component.");
        }

        skipButton.onClick.AddListener(conversationManger.SkipConversation);
    }

    
    private void OnDisable() {
        skipButton.onClick.RemoveAllListeners();
    }



    /// <summary>
    /// Cập nhật tên và avatar của NPC đang nói chuyện.
    /// </summary>
    public void UpdateDisplayedNPC(LocalizedString npcName, Sprite npcPortrait)
    {
        npcName_Text.SetText(npcName.GetLocalizedString());
        npcPortrait_Image.sprite = npcPortrait;
    }


    public void UpdateConversationText(string npcDialogue)
    {
        conversationDisplay_Text.SetText(npcDialogue);
    }


    public void AddLetterToDialogueText(char letter)
    {
        conversationDisplay_Text.text += letter;
    }


    public void DisplayConversationDecisions(List<SO_Decision> decision_List)
    {
        decisionPanel.gameObject.SetActive(true);

        foreach (var decision in decision_List)
        {
            var decisionPrefab = MGR_ObjectPoolManager.SpawnObject(decisionButton_Prefab, decisionPanel);

            // lấy các thành phần trong game object decision
            var decisionController = decisionPrefab.GetComponent<C_DecisionController>();
            var decisionBtn = decisionPrefab.GetComponent<Button>();

            // thiết lập các thành phần của game object decision
            decisionController.SetupDecisionUI(decision);
            decisionBtn.onClick.AddListener(decisionController.ImplementDecision);
            decisionBtn.onClick.AddListener(HideDecisionPanel);
        }

        // tắt tính năng skip dialogue
        ToggleSkipDialogueButton(false);
    }


    public void HideDecisionPanel()
    {
        foreach (Transform decision in decisionPanel)
        {
            decision.GetComponent<Button>().onClick.RemoveAllListeners();
            MGR_ObjectPoolManager.ReturnObjectToPool(decision.gameObject);
        }

        decisionPanel.gameObject.SetActive(false);
    }


    public void ToggleSkipDialogueButton(bool state)
    {
        var skipDialogueButton = conversationDisplay_Text.GetComponentInParent<Button>();
        skipDialogueButton.enabled = state;
    }


    public void EnableConversationPanel(bool enable)
    {
        conversationPanel.SetActive(enable);
    }
}
