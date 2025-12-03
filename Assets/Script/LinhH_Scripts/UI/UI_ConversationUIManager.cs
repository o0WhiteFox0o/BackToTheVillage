// 
// Member: LinhH
// Date: 24/11/2025
// 


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UI_ConversationUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public GameObject conversationPanel;
    [SerializeField] public Transform decisionPanel;

    [Header("Text")]
    [SerializeField] public TMP_Text npcName_Text;
    [SerializeField] public TMP_Text conversationDisplay_Text;

    [Header("Buttons")]
    [SerializeField] public Button skipDialogueButton;
    [SerializeField] public Button skipConversationButton;

    [Header("Image")]
    [SerializeField] public Image npcPortrait_Image;

    [Header("Prefab")]
    [SerializeField] private GameObject decisionButton_Prefab;

    private MGR_ConversationManager conversationManager;
    private UI_GameplayUIManager gameplayUIManager;



    private void Start()
    {
        gameplayUIManager = GetComponentInParent<UI_GameplayUIManager>();

        conversationManager = FindObjectOfType<MGR_ConversationManager>();

        if (conversationManager == null || gameplayUIManager == null)
        {
            Debug.LogError("Can't load component.");
        }

        // đăng ký các sự kiện cần thiết
        skipConversationButton.onClick.AddListener(conversationManager.SkipConversation);
        skipDialogueButton.onClick.AddListener(conversationManager.PlayNextLine);
    }


    private void OnDisable()
    {
        skipConversationButton.onClick.RemoveAllListeners();
        skipDialogueButton.onClick.RemoveAllListeners();
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
        transform.SetAsLastSibling();
    }
}
