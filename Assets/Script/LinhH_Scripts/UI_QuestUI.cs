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
    [SerializeField] public TMP_Text questReward_Text;


    /// <summary>
    /// Thiết lập quest UI.
    /// </summary>
    public void SetupQuestUI(SO_Quest quest)
    {
        switch (quest.questType)
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


    /// <summary>
    /// Cập nhật UI tiến trình cho nhiệm vụ thu thập được truyền vào.
    /// </summary>
    public void RefreshCollectionProgressUI(CollectionQuestProgress collectionProgress)
    {
        foreach (var item in collectionProgress.itemRequirements_List)
        {
            questProgress_Text.text = "";
            questProgress_Text.text += $"{item.item.name}: {item.currentQuantity} / {item.requirementQuantity}; ";
        }
    }


    /// <summary>
    /// Thiết lập các thành phần UI của nhiệm vụ thu thập.
    /// </summary>
    private void SetupCollectionQuestUI(SO_Quest quest)
    {
        questTittle_Text.SetText(quest.tittle);

        // thiết lập UI tiến trình của nhiệm vụ thu thập
        if (quest is SO_CollectionQuest collectionQuest)
        {
            foreach (var item in collectionQuest.targetItems_List)
            {
                questProgress_Text.text = "";
                questProgress_Text.text += $"{item.item.name}: {item.currentQuantity} / {item.requirementQuantity}; ";
            }
        }

        // TODO: thiết lập UI phần thưởng của nhiệm vụ
        questReward_Text.text = "";
    }


    private void SetupTalkingQuestUI(SO_Quest quest)
    {
        questTittle_Text.SetText(quest.tittle);

        // thiết lập UI tiến trình của nhiệm vụ thu thập
        if (quest is SO_TalkingQuest talkingQuest)
        {
            questProgress_Text.text = "";
            // TODO: chỉnh sửa lại đề dùng với Localization
            questProgress_Text.text += $"Talking with {talkingQuest.targetNPC.npcName}";
        }

        // TODO: thiết lập UI phần thưởng của nhiệm vụ
        questReward_Text.text = "";
    }
}
