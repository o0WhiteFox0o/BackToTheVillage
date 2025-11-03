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
    public const float TYPE_SPEED = 0.1f;

    [SerializeField] public GameplayUIManager gameplayUIManager;

    // ----------------------------------

    /// <summary>
    /// Danh sách chứa các cuộc hội thoại nằm trong cốt truyện.
    /// </summary>
    [SerializeField] public List<SO_ConversationData> storyConversation_List;
    private int storyConversationIndex;

    /// <summary>
    /// NPC kích hoạt cuộc hội thoại cốt truyện.
    /// </summary>
    private SO_NPCData triggerStorylineNPC;

    // -------------------------------
    private SO_ConversationData conversationData;

    private bool isTyping;
    private bool isConversationActive;
    private string currentLine;

    private int dialogueIndex;
    private int totalDialogueCount;


    private void Start()
    {
        // TODO: lấy story index trong file saved game
        storyConversationIndex = 0;
        triggerStorylineNPC = storyConversation_List[storyConversationIndex].dialogues[0].npcData;

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
        // kiểm tra npc mà người chơi tương tác có phải là npc kích hoạt hội thoại cốt truyện không
        var npcController = npc.GetComponent<C_NPCController>();

        // thiết lập đoạn hội thoại bình thường cho cuộc thoại hiện tại
        conversationData = npcController.priorityConversation;

        // kiểm tra danh sách cốt truyện còn cuộc hội thoại nào không
        if (storyConversationIndex < storyConversation_List.Count)
        {
            if (triggerStorylineNPC == npcController.npcData)
            {
                // thiết lập đoạn hội thoại stroy cho cuộc thoại hiện tại
                conversationData = storyConversation_List[storyConversationIndex];
            }
        }
        totalDialogueCount = conversationData.dialogues.Count;

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
        var npcName = conversationData.dialogues[dialogueIndex].npcData.npcName;
        var npcPortrait = conversationData.dialogues[dialogueIndex].npcData.portrait;
        gameplayUIManager.UpdateDisplayedNPC(npcName, npcPortrait);

        // hiển thị giao diên hội thoại
        gameplayUIManager.SetActiveConversationPanel(true);

        // laod câu thoại đầu và hiển thị nó
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


    public void EndConversation()
    {
        gameplayUIManager.SetActiveConversationPanel(false);
        isConversationActive = false;

        // nếu cuộc hội thoại story vừa kết thúc thì cập nhật cuộc hội thoại mới
        if (conversationData.isStoryConversation)
        {
            // nếu không còn hội thoại cốt truyện thì đặt npc kích hoạt bằng null
            if (++storyConversationIndex >= storyConversation_List.Count)
            {
                triggerStorylineNPC = null;
            }
            else
            {
                triggerStorylineNPC = storyConversation_List[storyConversationIndex].dialogues[0].npcData;
            }
        }
    }
}