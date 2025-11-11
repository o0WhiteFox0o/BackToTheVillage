// 
// Member: LinhH
// Date: 05/11/2025
// 


using TMPro;
using UnityEngine;

public class UI_QuestUI : MonoBehaviour
{
    [SerializeField] public TMP_Text questTittle_Text;
    [SerializeField] public TMP_Text questProgress_Text;
    [SerializeField] public TMP_Text questDescription_Text;
    // [SerializeField] public TMP_Text questReward_Text;


    /// <summary>
    /// Thiết lập quest UI.
    /// </summary>
    public void SetupQuestDetails(IQuestProgress quest)
    {
        switch (quest.GetQuest().questType)
        {
            case QuestType.Collection:
                SetupCollectionQuestUI(quest);
                break;

            case QuestType.Talking:
                SetupTalkingQuestUI(quest);
                break;

            case QuestType.Giving:

                break;

            case QuestType.Selling:
                break;
        }
    }


    public void ToggleQuestDetails(bool enable)
    {
        questTittle_Text.gameObject.SetActive(enable);
        questDescription_Text.gameObject.SetActive(enable);
        questProgress_Text.gameObject.SetActive(enable);
    }


    /// <summary>
    /// Thiết lập các thành phần UI của nhiệm vụ thu thập.
    /// </summary>
    private void SetupCollectionQuestUI(IQuestProgress questProgress)
    {
        ToggleQuestDetails(true);

        Debug.Log($"{questProgress.GetType()}");

        questTittle_Text.SetText(questProgress.GetQuest().tittle);

        // thiết lập UI tiến trình của nhiệm vụ thu thập
        if (questProgress is CollectionQuestProgress collectionProgress)
        {
            foreach (var item in collectionProgress.itemRequirements_List)
            {
                questProgress_Text.text = "";
                questProgress_Text.text += $"{item.item.name}: {item.currentQuantity} / {item.requirementQuantity}; ";

                Debug.Log($"{item.item.name}: {item.currentQuantity} / {item.requirementQuantity}; ");
            }
        }

        // thiết lập mô tả nhiệm vụ

        // TODO: thiết lập UI phần thưởng của nhiệm vụ
        // questReward_Text.text = "";
    }


    private void SetupTalkingQuestUI(IQuestProgress quest)
    {
        ToggleQuestDetails(true);

        questTittle_Text.SetText(quest.GetQuest().tittle);

        // thiết lập UI tiến trình của nhiệm vụ thu thập
        if (quest is TalkingQuestProgress talkingQuest)
        {
            questProgress_Text.text = "";
            // TODO: chỉnh sửa lại đề dùng với Localization
            questProgress_Text.text += $"Talking with {talkingQuest.quest.targetNPC.npcName}";
        }

        // thiết lập mô tả nhiệm vụ

        // TODO: thiết lập UI phần thưởng của nhiệm vụ
        // questReward_Text.text = "";
    }
}
