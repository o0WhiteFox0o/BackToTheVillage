// 
// Member: LinhH
// Date: 05/11/2025
// 


/// <summary>
/// Tạo các biến thể của Quest Progress (e.g. Collection, Talking, ...)
/// </summary>
public static class QuestProgressFactory
{
    public static QuestProgress CreateQuestProgress(SO_Quest newQuest)
    {
        switch (newQuest.questType)
        {
            case QuestType.Collection:
                if (newQuest is SO_CollectionQuest collectionQuest)
                {
                    return new CollectionQuestProgress(collectionQuest);
                }
                break;

            case QuestType.Talking:
                if (newQuest is SO_TalkingQuest talkingQuest)
                {
                    return new TalkingQuestProgress { quest = talkingQuest, isActive = true };
                }
                break;

            case QuestType.Giving:
                break;
        }

        return null;
    }
}