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


    [Header("Conversation")]
    [SerializeField] public GameObject conversationPanel;
    [SerializeField] public TMP_Text npcNameText;
    [SerializeField] public Image npcPortraitImage;
    [SerializeField] public TMP_Text conversationDisplayText;


    [Header("Quests")]
    [SerializeField] GameObject questUI_Prefab;
    [SerializeField] Transform questPanel;


    [Header("Other")]
    [SerializeField] public GraphicRaycaster uiRaycaster;
    [SerializeField] public EventSystem eventSystem;


    private bool isAnyUIOpen;


    private void Start()
    {
        isAnyUIOpen = false;

        // đăng ký sự kiện cần thiết
        InputManager.OnOpenBagPress += ToggleBagUI;
        InputManager.OnGeneralUIPress += ToggleGeneralUI;

        MGR_QuestManager.OnQuestListUpdate += RefreshQuestUIList;
        CollectionQuestProgress.OnCollectionQuestUpdate += RefreshCollectionProgressUI;
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= ToggleBagUI;
        InputManager.OnGeneralUIPress -= ToggleGeneralUI;

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
            isAnyUIOpen = true;
        }
    }


    /// <summary>
    /// Hiển thị UI con trong general UI.
    /// </summary>
    public void EnableGeneralSubUI(Transform subUI)
    {
        subUI.SetAsLastSibling();
    }


    public void SetActiveConversationPanel(bool value)
    {
        conversationPanel.SetActive(value);
    }


    /// <summary>
    /// Cập nhật tên và avatar của NPC đang nói chuyện.
    /// </summary>
    public void UpdateDisplayedNPC(string npcName, Sprite npcPortrait)
    {
        npcNameText.SetText(npcName);
        npcPortraitImage.sprite = npcPortrait;
    }


    public void UpdateConversationText(string npcDialogue)
    {
        conversationDisplayText.SetText(npcDialogue);
    }


    public void AddLetterToDialogueText(char letter)
    {
        conversationDisplayText.text += letter;
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
        foreach (var quest in MGR_QuestManager.Instance.activeQuests_List)
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
}
