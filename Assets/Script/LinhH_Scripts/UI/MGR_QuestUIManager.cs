// 
// Member: LinhH
// Date: 21/11/2025
// 


using System.Collections.Generic;
using System.Linq;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class MGR_QuestUIManager : MonoBehaviour
{
    [SerializeField] public GameObject backgroundImage;
    [SerializeField] Transform questPanel;

    [Tooltip("Danh sách các đối tượng highlight của các nút phân loại nhiệm vụ.")]
    [SerializeField] public List<GameObject> questCategorizeHighlights_List;
    [SerializeField] GameObject noQuestInList_Text;
    [SerializeField] public GameObject questUI_Notification;
    [SerializeField] public Button backButton;

    private List<GameObject> questPrefab_List = new List<GameObject>();
    private UI_QuestDetails questDetailsUI;
    private GameObject questUI_Prefab;
    private MGR_QuestManager questManager;


    private void Start()
    {
        questManager = FindObjectOfType<MGR_QuestManager>();
        questDetailsUI = backgroundImage.GetComponentInChildren<UI_QuestDetails>();

        questUI_Prefab = Resources.Load<GameObject>("Prefabs/UI/PFB_QuestUI");

        var gameplayUIMgr = GetComponentInParent<UI_GameplayUIManager>();

        if (questManager == null || questDetailsUI == null || questUI_Prefab == null || gameplayUIMgr == null)
        {
            Debug.LogError("Can't load a manager component.");
        }

        backButton.onClick.AddListener(gameplayUIMgr.ToggleQuestUI);
    }


    private void OnDisable() {
        backButton.onClick.RemoveAllListeners();
    }


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
            questDetailsUI.ToggleQuestDetails(false);
            return;
        }

        noQuestInList_Text.SetActive(false);
        questPrefab_List.Clear();

        // hiển thị các nhiệm vụ trong danh sách được truyền vào
        for (int i = 0; i < questProgress_List.Count; i++)
        {
            var quest = questProgress_List[i];

            var newQuestUI = MGR_ObjectPoolManager.SpawnObject(questUI_Prefab, questPanel);
            questPrefab_List.Add(newQuestUI);

            newQuestUI.GetComponentInChildren<TMP_Text>().SetText(quest.GetQuest().questTittle.GetLocalizedString());

            // đăng ký sự kiện cho nút nhiệm vụ mới
            var questButton = newQuestUI.GetComponent<Button>();
            int index = i;

            questButton.onClick.AddListener(() => DisplayQuestDetail(quest));
            questButton.onClick.AddListener(() => HighlightQuestSelected(index));
        }

        // highlisht và hiển thị chi tiết của nhiệm vụ đầu tiên trong danh sách
        HighlightQuestSelected(0);
        DisplayQuestDetail(questProgress_List[0]);
    }


    private void DisplayQuestDetail(IQuestProgress quest)
    {
        questDetailsUI.SetupQuestDetails(quest);
    }


    private void HighlightQuestSelected(int selectedIndex)
    {
        // tắt highlight của toàn bộ quest
        foreach (var questUI in questPrefab_List)
        {
            questUI.GetComponent<Image>().enabled = false;
        }

        // highlight quest được chọn
        questPrefab_List[selectedIndex].GetComponent<Image>().enabled = true;
    }


    public void EnableQuestUI(bool enable)
    {
        backgroundImage.SetActive(enable);
    }
}
