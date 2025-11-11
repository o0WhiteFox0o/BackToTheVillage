// 
// Member   : Linh
// Date     : 
// 


using System;
using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Quản lý các thành phần giao diện trong gameplay
/// </summary>
public class GameplayUIManager : MonoBehaviour
{
    [Header("Menu UIs")]
    [SerializeField] public GameObject bagUI;
    [SerializeField] public GameObject generalUI;
    [SerializeField] public GameObject npcUI;
    [SerializeField] public GameObject settingUI;
    [SerializeField] public GameObject questUI;
    [SerializeField] public GameObject generalUI_Notification;


    [Header("Conversation")]
    [SerializeField] public GameObject conversation_UI;
    [SerializeField] public TMP_Text npcName_Text;
    [SerializeField] public Image npcPortrait_Image;
    [SerializeField] public TMP_Text conversationDisplay_Text;
    [SerializeField] public GameObject decisionButton_Prefab;
    [SerializeField] public Transform decisionPanel;


    [Header("Quests")]
    [SerializeField] GameObject questUI_Prefab;
    [SerializeField] Transform questPanel;

    [Tooltip("Danh sách các đối tượng highlight của các nút phân loại nhiệm vụ.")]
    [SerializeField] public List<GameObject> questCategorizeHighlights_List;
    [SerializeField] GameObject noQuestInList_Text;
    [SerializeField] public GameObject questUI_Notification;
    [SerializeField] public UI_QuestUI questUIController;


    [Header("Other")]
    [SerializeField] public GraphicRaycaster uiRaycaster;


    private MGR_QuestManager questManager;
    private EventSystem eventSystem;
    private bool isAnyUIOpen;


    private void Start()
    {
        isAnyUIOpen = false;

        eventSystem = FindObjectOfType<EventSystem>();
        questManager = FindObjectOfType<MGR_QuestManager>();

        if (eventSystem == null || questManager == null)
        {
            Debug.LogError("Can't load a manager component.");
        }

        // đăng ký sự kiện cần thiết
        InputManager.OnOpenBagPress += ToggleBagUI;
        InputManager.OnGeneralUIPress += ToggleGeneralUI;
        InputManager.OnQuestUIButtonPress += ToggleQuestUI;

        DontDestroyOnLoad(this);
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= ToggleBagUI;
        InputManager.OnGeneralUIPress -= ToggleGeneralUI;
        InputManager.OnQuestUIButtonPress -= ToggleQuestUI;

        // MGR_QuestManager.OnQuestListUpdate -= RefreshQuestUIList;
        // CollectionQuestProgress.OnCollectionQuestUpdate -= RefreshCollectionProgressUI;
    }


    #region General
    public void ToggleBagUI()
    {
        // tắt UI túi đồ nếu nó đang bật
        if (bagUI.activeSelf)
        {
            bagUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI túi đồ nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            bagUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }


    public void ToggleGeneralUI()
    {
        // tắt UI general nếu nó đang bật
        if (generalUI.activeInHierarchy)
        {
            generalUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI general nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            generalUI.SetActive(true);
            // isAnyUIOpen = true;
        }
    }


    public void ToggleNPC_UI()
    {
        // tắt UI npc nếu nó đang bật
        if (npcUI.activeInHierarchy)
        {
            npcUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI npc nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            npcUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }


    public void ToggleQuestUI()
    {
        // tắt UI quest nếu nó đang bật
        if (questUI.activeInHierarchy)
        {
            questUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI quest nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            questUI.SetActive(true);
            isAnyUIOpen = true;

            FillQuestCategorize(0);

            // tắt thông báo quest khi người chơi mở giao diện nhiệm vụ
            DisableQuestNotification();
        }
    }


    public void ToggleSettingUI()
    {
        // tắt UI setting nếu nó đang bật
        if (settingUI.activeInHierarchy)
        {
            settingUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI setting nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            settingUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }
    #endregion


    #region Conversation
    public void SetActiveConversationPanel(bool value)
    {
        conversation_UI.SetActive(value);
    }


    /// <summary>
    /// Cập nhật tên và avatar của NPC đang nói chuyện.
    /// </summary>
    public void UpdateDisplayedNPC(string npcName, Sprite npcPortrait)
    {
        npcName_Text.SetText(npcName);
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
    #endregion


    #region Quest
    /// <summary>
    /// Hiển thị tất cả nhiệm vụ của loại nhiệm vụ được truyền vào trên giao diện nhiệm vụ.
    /// </summary>
    public void FillQuestCategorize(int questCategorize)
    {
        HighlightQuestCategorizeButton(questCategorize);

        switch ((QuestCategorize)questCategorize)
        {
            case QuestCategorize.StoryQuest:
                var storyQuest = questManager?.activeQuests_List?.Where(q => q.GetQuest().questCategorize == QuestCategorize.StoryQuest);
                DisplayQuestList(storyQuest.ToList());
                break;

            case QuestCategorize.EventQuest:
                var eventQuest_List = questManager.activeQuests_List?.Where(q => q.GetQuest().questCategorize == QuestCategorize.EventQuest);
                DisplayQuestList(eventQuest_List.ToList());
                break;

            case QuestCategorize.CompanionQuest:
                var companionQuest_List = questManager.activeQuests_List?.Where(q => q.GetQuest().questCategorize == QuestCategorize.CompanionQuest);
                DisplayQuestList(companionQuest_List.ToList());
                break;

            case QuestCategorize.Other:
                var otherQuest_List = questManager.activeQuests_List?.Where(q => q.GetQuest().questCategorize == QuestCategorize.Other);
                DisplayQuestList(otherQuest_List.ToList());
                break;

            default:
                break;
        }
    }


    private void HighlightQuestCategorizeButton(int index)
    {
        foreach (var questCategorize in questCategorizeHighlights_List)
        {
            questCategorize.SetActive(false);
        }

        questCategorizeHighlights_List[index].SetActive(true);
    }


    private void DisplayQuestList(List<IQuestProgress> questProgress_List)
    {
        // clear các nhiệm vụ danh có trong panel
        foreach (Transform questUI in questPanel)
        {
            questUI.GetComponent<Button>().onClick.RemoveAllListeners();
            MGR_ObjectPoolManager.ReturnObjectToPool(questUI.gameObject);
        }

        // nếu không có nhiệm vụ để hiển thị thì hiển thị text thông báo
        if (questProgress_List.Count == 0)
        {
            noQuestInList_Text.SetActive(true);
            questUIController.ToggleQuestDetails(false);
            return;
        }

        noQuestInList_Text.SetActive(false);

        // hiển thị các nhiệm vụ trong danh sách được truyền vào
        foreach (var quest in questProgress_List)
        {
            var newQuestUI = MGR_ObjectPoolManager.SpawnObject(questUI_Prefab, questPanel);

            newQuestUI.GetComponentInChildren<TMP_Text>().SetText(quest.GetQuest().tittle);

            // đăng ký sự kiện cho nút nhiệm vụ mới
            var questButton = newQuestUI.GetComponent<Button>();
            questButton.onClick.AddListener(() => DisplayQuestDetail(quest));
        }

        // hiển thị chi tiết của nhiệm vụ đầu tiên trong danh sách
        DisplayQuestDetail(questProgress_List[0]);
    }


    private void DisplayQuestDetail(IQuestProgress quest)
    {
        questUIController.SetupQuestDetails(quest);
    }
    #endregion


    #region Notification
    public void EnableQuestNotification()
    {
        questUI_Notification.SetActive(true);
        generalUI_Notification.SetActive(true);
    }


    public void DisableQuestNotification()
    {
        questUI_Notification.SetActive(false);

        UpdateGeneralUINotification();
    }


    /// <summary>
    /// Cập nhật thông báo của general UI dựa theo thông báo của các UI con nằm trong nó.
    /// </summary>
    private void UpdateGeneralUINotification()
    {
        // kiểm tra thông báo của tất cả các UI của general UI
        // nếu có bất kỳ thông báo nào được bật thì bật thông báo general
        if (questUI_Notification.activeSelf)
        {
            generalUI_Notification.SetActive(true);
            return;
        }

        // nếu không có thông báo nào được bật thì tắt thông báo general
        generalUI_Notification.SetActive(false);
    }

    #endregion
}