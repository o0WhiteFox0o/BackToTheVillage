// 
// Member   : Linh
// Date     : 
// 


using System;
using System.Collections.Generic;
using TMPro;
using UnityEditor.Localization.Plugins.XLIFF.V12;
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
    [SerializeField] public GameObject questUI_Notification;


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

        MGR_QuestManager.OnQuestListUpdate += RefreshQuestUIList;

        CollectionQuestProgress.OnCollectionQuestUpdate += RefreshCollectionProgressUI;
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= ToggleBagUI;
        InputManager.OnGeneralUIPress -= ToggleGeneralUI;
        InputManager.OnQuestUIButtonPress -= ToggleQuestUI;

        MGR_QuestManager.OnQuestListUpdate -= RefreshQuestUIList;
        CollectionQuestProgress.OnCollectionQuestUpdate -= RefreshCollectionProgressUI;
    }


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


    /// <summary>
    /// Làm mới danh sách nhiệm vụ khi thêm hoặc loại bỏ một nhiệm vụ.
    /// </summary>
    private void RefreshQuestUIList()
    {
        // ẩn tất cả các nhiệm vụ trong giao diện nhiệm vụ
        foreach (Transform questUI in questPanel)
        {
            questUI.gameObject.SetActive(false);
        }

        // cập nhật giao diện nhiệm vụ cho từng nhiệm vụ trong danh sách
        foreach (var quest in questManager.activeQuests_List)
        {
            // tạo các quest UI
            var questUI = MGR_ObjectPoolManager.SpawnObject(questUI_Prefab, questPanel);

            // thiết lập các thành phần của quest UI
            questUI.GetComponent<UI_QuestUI>().SetupQuestUI(quest.GetQuest());
        }
    }


    /// <summary>
    /// Cập nhật UI nhiệm vụ thu thập vật phẩm khi có một vật phẩm được thêm vào.
    /// </summary>
    private void RefreshCollectionProgressUI(CollectionQuestProgress collectionProgress)
    {
        foreach (Transform questUI in questPanel)
        {
            // nếu nhiệm vụ được duyệt không phải là nhiệm vụ cần cập nhật thì bỏ qua nó
            if (questUI.GetComponent<UI_QuestUI>().questTittle_Text.text != collectionProgress.quest.tittle) { continue; }

            questUI.GetComponent<UI_QuestUI>().RefreshCollectionProgressUI(collectionProgress);

            Debug.Log("Update collection quest UI");
        }
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
}