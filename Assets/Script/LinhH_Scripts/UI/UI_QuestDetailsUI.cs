// 
// Member: LinhH
// Date: 05/11/2025
// 


using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_QuestDetails : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] public Transform questRewardPanel;

    [Header("Text")]
    [SerializeField] public TMP_Text questTittle_Text;
    [SerializeField] public TMP_Text questProgress_Text;
    [SerializeField] public TMP_Text questDescription_Text;

    [Header("Button")]
    [SerializeField] public Button claimReward_Button;

    [Header("Prefab")]
    [SerializeField] private GameObject itemReward_Prefab;

    private MGR_QuestManager questManager;
    private UI_QuestUIManager questUIManager;


    private void OnEnable()
    {
        questManager = FindObjectOfType<MGR_QuestManager>();
        questUIManager = GetComponentInParent<UI_QuestUIManager>();
    }


    /// <summary>
    /// Thiết lập quest UI.
    /// </summary>
    public void SetupQuestDetails(QuestProgress quest)
    {
        ToggleQuestDetails(true);

        SetupQuestDescription(quest);

        switch (quest.GetQuest().questType)
        {
            case QuestType.Collection:
                SetupCollectionQuestProgress(quest);
                break;

            case QuestType.Talking:
                SetupTalkingQuestProgress(quest);
                break;

            case QuestType.Giving:
                break;

            case QuestType.Selling:
                break;
        }

        ResetQuestRewardUI();
        SetupQuestReward(quest);

        // hiển thị nút Claim nếu nhiệm vụ đã hoàn thành
        if (!quest.IsComplete())
        {
            claimReward_Button.gameObject.SetActive(false);
            claimReward_Button.onClick.RemoveAllListeners();
        }
        else
        {
            claimReward_Button.onClick.AddListener(() => questManager.GrantReward(quest));
            claimReward_Button.onClick.AddListener(questUIManager.RefreshQuestList);
            claimReward_Button.onClick.AddListener(questUIManager.RefreshCategorizesNotification);
        }
    }


    public void ToggleQuestDetails(bool enable)
    {
        questTittle_Text.gameObject.SetActive(enable);
        questDescription_Text.gameObject.SetActive(enable);
        questProgress_Text.gameObject.SetActive(enable);

        ResetQuestRewardUI();
        ToggleClaimButton(enable);
    }


    /// <summary>
    /// Thiết lập tiến trình của nhiệm vụ thu thập trên Quest UI.
    /// </summary>
    private void SetupCollectionQuestProgress(QuestProgress questProgress)
    {
        // thiết lập UI tiến trình của nhiệm vụ thu thập
        if (questProgress is CollectionQuestProgress collectionProgress)
        {
            Debug.Log($"quest item requirement count: {collectionProgress.itemRequirements_List.Count}");
            foreach (var item in collectionProgress.itemRequirements_List)
            {
                questProgress_Text.text = "";
                questProgress_Text.text += $"{item.item_SO.displayName.GetLocalizedString()}: {item.currentQuantity} / {item.requirementQuantity}; ";
            }
        }
    }


    /// <summary>
    /// Thiết lập tiến trình của nhiệm vụ trò chuyện trên Quest UI.
    /// </summary>
    private void SetupTalkingQuestProgress(QuestProgress quest)
    {
        // thiết lập UI tiến trình của nhiệm vụ thu thập
        if (quest is TalkingQuestProgress talkingQuest)
        {
            questProgress_Text.text = "";
            // TODO: chỉnh sửa lại đề dùng với Localization
            questProgress_Text.text += $"Talking with {talkingQuest.quest.targetNPC.npcName}";
        }
    }


    private void SetupQuestDescription(QuestProgress questProgress)
    {
        // thiết lập tiêu đề nhiệm vụ
        questTittle_Text.SetText(questProgress.GetQuest().questTittle.GetLocalizedString());

        // thiết lập mô tả nhiệm vụ
        questDescription_Text.SetText(questProgress.GetQuest().questDescription.GetLocalizedString());
    }


    /// <summary>
    /// Thiết lập các phần thưởng của nhiệm vụ trên UI.
    /// </summary>
    private void SetupQuestReward(QuestProgress questProgress)
    {
        var reward = questProgress.GetQuest().reward;

        // hiển thị kinh nghiệm nhận được
        if (reward.experience != 0)
        {
            var expReward = MGR_ObjectPoolManager.SpawnObject(itemReward_Prefab, questRewardPanel);

            expReward.GetComponentInChildren<Image>().sprite = null;
            expReward.GetComponentInChildren<TMP_Text>().SetText($"x {reward.experience}");

            // sắp xếp thứ tự hiển thị phần thưởng trên UI
            expReward.transform.SetAsLastSibling();
        }

        // hiển thị vàng nhận được
        if (reward.currency != 0)
        {
            var currencyReward = MGR_ObjectPoolManager.SpawnObject(itemReward_Prefab, questRewardPanel);

            currencyReward.GetComponentInChildren<Image>().sprite = null;
            currencyReward.GetComponentInChildren<TMP_Text>().SetText($"x {reward.currency}");

            // sắp xếp thứ tự hiển thị phần thưởng trên UI
            currencyReward.transform.SetAsLastSibling();
        }

        // hiển thị item nhận được
        if (reward.itemRewards.Count > 0)
        {
            foreach (var item in reward.itemRewards)
            {
                var itemReward = MGR_ObjectPoolManager.SpawnObject(itemReward_Prefab, questRewardPanel);

                itemReward.GetComponentInChildren<Image>().sprite = item.item.icon;
                itemReward.GetComponentInChildren<TMP_Text>().SetText($"x {item.quantity}");

                // sắp xếp thứ tự hiển thị phần thưởng trên UI
                itemReward.transform.SetAsLastSibling();
            }
        }

        // hiển thị công thức nhận được
        if (reward.craftingRecipe != null)
        {
            var recipeReward = MGR_ObjectPoolManager.SpawnObject(itemReward_Prefab, questRewardPanel);

            recipeReward.GetComponentInChildren<Image>().sprite = reward.craftingRecipe.icon;
            recipeReward.GetComponentInChildren<TMP_Text>().SetText($"");

            // sắp xếp thứ tự hiển thị phần thưởng trên UI
            recipeReward.transform.SetAsLastSibling();
        }
    }


    private void ResetQuestRewardUI()
    {
        foreach (Transform reward in questRewardPanel)
        {
            MGR_ObjectPoolManager.ReturnObjectToPool(reward.gameObject);
        }
    }


    public void ToggleClaimButton(bool enable)
    {
        claimReward_Button.gameObject.SetActive(enable);
    }
}
