// 
// Member: LinhH
// Date: 03/11/2025
// 


using System;


[Serializable]
public class QuestState
{
    public SO_Quest quest;
    public bool isActive;
    public bool isCompleted;

    public void CheckProgress()
    {
        // if (quest.objectives.TrueForAll(o => o.IsComplete))
        // {
        //     CompleteQuest();
        //     MGR_QuestManager.Instance.RemoveQuest(quest);
        // }
    }

    private void CompleteQuest()
    {
        isCompleted = true;
        isActive = false;

        quest.reward?.GrantReward();
    }
}
