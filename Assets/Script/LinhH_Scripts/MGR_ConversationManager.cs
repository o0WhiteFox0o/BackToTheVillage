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


/// <summary>
/// Quản lý các chức năng hội thoại.
/// </summary>
public class MGR_ConversationManager : MonoBehaviour
{
    // _________ TEST ___________
    [SerializeField] public SO_ConversationData testDialogueData;

    public const float TYPE_SPEED = 0.1f;

    [SerializeField] public GameplayUIManager gameplayUIManager;

    private SO_ConversationData conversationData;


    private bool isTyping;
    private bool isConversationActive;
    private string currentLine;

    private int dialogueIndex;
    private int totalDialogueCount;


    private void Start()
    {
        InputManager.OnSkipDialoguePress += PlayNextLine;
    }


    private void OnDisable()
    {
        InputManager.OnSkipDialoguePress -= PlayNextLine;
    }


    /// <summary>
    /// Bắt đầu một cuộc trò chuyện mới.
    /// </summary>
    public void StartConversation(SO_ConversationData conversation)
    {
        // nếu có một cuộc hội thoại đang diễn ra thì dừng
        if (isConversationActive) { return; }

        // thiết lập các thông tin cần thiết
        conversationData = conversation;
        totalDialogueCount = conversation.dialogues.Count;

        isConversationActive = true;
        dialogueIndex = 0;

        // thiết lập avatar và tên NPC
        var npcName = conversationData.dialogues[dialogueIndex].npcData.npcName;
        var npcPortrait = conversationData.dialogues[dialogueIndex].npcData.portrait;
        gameplayUIManager.UpdateDisplayedNPC(npcName, npcPortrait);

        // hiển thị giao diên hội thoại
        gameplayUIManager.SetActiveConversationPanel(true);

        currentLine = conversationData.dialogues[dialogueIndex].dialogue.GetLocalizedString();

        DisplayCurrentLine();
    }


    private IEnumerator TypeNextLine()
    {
        isTyping = true;

        // reset văn bản thoại
        gameplayUIManager.UpdateConversationText("");

        // lần lượt thêm từng chữ cái vào dialogue text sau một
        foreach (var letter in currentLine)
        {
            gameplayUIManager.AddLetterToDialogueText(letter);

            // chờ để thêm chữ cái tiếp theo
            yield return new WaitForSeconds(TYPE_SPEED);
        }

        isTyping = false;
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

            gameplayUIManager.UpdateConversationText(currentLine);
            isTyping = false;

            skipTyping = true;
        }

        // nếu đây là câu thoại cuối của cuộc hội thoại và người chơi không skip câu thoại thì kết thúc cuộc hội thoại
        if (dialogueIndex >= totalDialogueCount && !skipTyping)
        {
            EndConversation();
            return;
        }

        // nếu người chơi skip câu thoại thì dừng
        if (skipTyping) return;


        // kiểm tra xem còn câu thoại nào trong đoạn hội thoại không, nếu như còn thoại thì tiếp tục hiển thị nó
        if (++dialogueIndex < totalDialogueCount)
        {
            // load câu thoại tiếp theo
            currentLine = conversationData.dialogues[dialogueIndex].dialogue.GetLocalizedString();

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
        gameplayUIManager.SetActiveConversationPanel(false);
        isConversationActive = false;
    }
}