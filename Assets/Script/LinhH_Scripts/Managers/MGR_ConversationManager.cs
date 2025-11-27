// 
// Member   : Linh
// Date     : 30/10/2025
// 

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;
using UnityEngine.Localization.Tables;


public delegate void ConversationHandler(SO_NPCData npc);
public delegate void EndConversationHandler(SO_ConversationData conversationData);


/// <summary>
/// Quản lý các chức năng hội thoại.
/// </summary>
public class MGR_ConversationManager : MonoBehaviour
{
    public const float TYPE_SPEED = 0.1f;

    private UI_GameplayUIManager gameplayUIManager;
    private MGR_QuestManager questManager;

    private SO_ConversationData conversationData;
    private bool isTyping;
    private bool isConversationActive;
    private string currentLine;
    private int dialogueIndex;


    /// <summary>
    /// Được gọi khi một cuộc trò chuyện kết thúc.
    /// </summary>
    public static event EndConversationHandler OnConversationEnd;

    /// <summary>
    /// Được gọi khi người chơi thực hiện một cuộc hội thoại với NPC.
    /// </summary>
    public static event ConversationHandler OnStartConversation;


    private void Start()
    {
        gameplayUIManager = FindObjectOfType<UI_GameplayUIManager>();
        questManager = FindObjectOfType<MGR_QuestManager>();

        if (gameplayUIManager == null || questManager == null)
        {
            Debug.LogError("Can't load a manager component.");
        }

        InputManager.OnSkipDialoguePress += PlayNextLine;
        InputManager.OnRightClickNPC += SetupConversation;
    }


    private void OnDisable()
    {
        InputManager.OnSkipDialoguePress -= PlayNextLine;
        InputManager.OnRightClickNPC -= SetupConversation;
    }


    private void SetupConversation(GameObject npc)
    {
        // lấy thông tin của npc được trò chuyện
        var talkingNPC = npc.GetComponent<C_NPCController>();

        // thực hiện cuộc hội thoại nhiệm vụ
        if (questManager.activeQuests_List.Count != 0)
        {
            foreach (var quest in questManager.activeQuests_List)
            {
                if (quest is TalkingQuestProgress talkingQuest)
                {
                    var questNPCId = talkingQuest.quest.targetNPC.npcId;

                    // nếu npc đang trò chuyện không phải là npc kích hoạt nhiệm vụ thì bỏ qua nó
                    if (talkingNPC.npcData.npcId != questNPCId) { continue; }

                    conversationData = talkingQuest.quest.conversationData;
                    StartConversation();

                    return;
                }
            }
        }

        // thực hiện cuộc hội thoại bình thường của nhân vật
        conversationData = talkingNPC.priorityConversation;
        StartConversation();
    }


    /// <summary>
    /// Bắt đầu một cuộc trò chuyện mới.
    /// </summary>
    public void StartConversation()
    {
        // nếu có một cuộc hội thoại đang diễn ra thì dừng
        if (isConversationActive) { return; }

        isConversationActive = true;
        dialogueIndex = 0;

        // thiết lập avatar và tên NPC
        var npcName = conversationData.dialogue_List[dialogueIndex].npcData.npcName;
        var npcPortrait = conversationData.dialogue_List[dialogueIndex].npcData.portrait;
        gameplayUIManager.conversationUIManager.UpdateDisplayedNPC(npcName, npcPortrait);

        // hiển thị giao diên hội thoại
        gameplayUIManager.conversationUIManager.EnableConversationPanel(true);

        OnStartConversation?.Invoke(conversationData.dialogue_List[dialogueIndex].npcData);

        // laod câu thoại đầu và hiển thị nó
        currentLine = conversationData.dialogue_List[dialogueIndex].dialogue.GetLocalizedString();
        DisplayCurrentLine();
    }


    private IEnumerator TypeNextLine()
    {
        isTyping = true;

        // reset văn bản thoại
        gameplayUIManager.conversationUIManager.UpdateConversationText("");

        // lần lượt thêm từng chữ cái vào dialogue text sau một
        foreach (var letter in currentLine)
        {
            gameplayUIManager.conversationUIManager.AddLetterToDialogueText(letter);

            // chờ để thêm chữ cái tiếp theo
            yield return new WaitForSeconds(TYPE_SPEED);
        }

        isTyping = false;

        UpdateDecisionOptions();
    }


    /// <summary>
    /// Hiển thị câu thoại tiếp theo trong cuộc trò chuyện.
    /// </summary>
    public void PlayNextLine()
    {
        bool skipTyping = false;

        // nếu thoại đang chạy thì hiển thị nó đầy đủ
        if (isTyping)
        {
            StopAllCoroutines();

            gameplayUIManager.conversationUIManager.UpdateConversationText(currentLine);
            isTyping = false;
            skipTyping = true;
        }

        UpdateDecisionOptions();

        // nếu đây là câu thoại cuối của cuộc hội thoại và người chơi không skip câu thoại thì kết thúc cuộc hội thoại
        if (dialogueIndex >= conversationData.dialogue_List.Count && !skipTyping)
        {
            EndConversation();
            return;
        }

        // nếu người chơi skip câu thoại thì dừng
        if (skipTyping) return;


        // kiểm tra xem còn câu thoại nào trong đoạn hội thoại không, nếu như còn thoại thì tiếp tục hiển thị nó
        if (++dialogueIndex < conversationData.dialogue_List.Count)
        {
            // load câu thoại tiếp theo
            currentLine = conversationData.dialogue_List[dialogueIndex].dialogue.GetLocalizedString();
            DisplayCurrentLine();
        }
        // kết thúc cuộc hội thoại
        else
        {
            EndConversation();
        }
    }


    private void DisplayCurrentLine()
    {
        StopAllCoroutines();
        StartCoroutine(TypeNextLine());
    }


    private void EndConversation()
    {
        gameplayUIManager.conversationUIManager.EnableConversationPanel(false);
        isConversationActive = false;

        // kiểm tra có nhiệm vụ nào được giao hay không
        if (conversationData.quest != null)
        {
            // thêm nhiệm vụ vào danh sách nhiệm vụ
            questManager.AddQuest(conversationData.quest);
        }

        OnConversationEnd?.Invoke(conversationData);

        gameplayUIManager.conversationUIManager.HideDecisionPanel();
        gameplayUIManager.conversationUIManager.transform.SetAsFirstSibling();
    }


    public void ContinueConversation(SO_ConversationData newConversation)
    {
        OnConversationEnd?.Invoke(conversationData);

        conversationData = newConversation;
        dialogueIndex = 0;

        // thiết lập avatar và tên NPC
        var npcName = conversationData.dialogue_List[dialogueIndex].npcData.npcName;
        var npcPortrait = conversationData.dialogue_List[dialogueIndex].npcData.portrait;
        gameplayUIManager.conversationUIManager.UpdateDisplayedNPC(npcName, npcPortrait);

        // laod câu thoại đầu và hiển thị nó
        currentLine = conversationData.dialogue_List[dialogueIndex].dialogue.GetLocalizedString();
        DisplayCurrentLine();
    }


    public void SkipConversation()
    {
        // kiểm tra có câu thoại lựa chọn nào không
        // nếu có thì skip tới câu thoại lựa chọn đó

        // không có câu thoại lựa chọn nào thì kết thúc cuộc trò chuyện
        EndConversation();
    }


    private void UpdateDecisionOptions()
    {
        // nếu cuộc hội thoại không có lựa chọn thì dừng
        if (conversationData.decisionIndex == -1) { return; }

        if (dialogueIndex == conversationData.decisionIndex)
        {
            // hiển thị các lựa chọn hội thoại
            gameplayUIManager.conversationUIManager.DisplayConversationDecisions(conversationData.decision_List);
        }
    }
}