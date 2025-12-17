
// 
// Member: LinhH
// Date: 08/12/2025
// 


using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class UI_GeneralUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public GameObject background;

    [Header("Buttons")]
    [SerializeField] public Button backButton;
    [SerializeField] public Button npcButton;
    [SerializeField] public Button questButton;
    [SerializeField] public Button settingButton;
    [SerializeField] private Button exitButton;

    private UI_QuestUIManager questUIManager;


    private void Start()
    {
        questUIManager = FindObjectOfType<UI_QuestUIManager>();

        if (questUIManager == null)
        {
            Debug.LogError("Can't load quest UI manager!!!");
        }

        // TEST
        exitButton.onClick.AddListener(() => SceneManager.LoadScene(0));
    }


    void OnDestroy()
    {
        // TEST
        exitButton.onClick.RemoveAllListeners();
    }


    private void Update()
    {
        RefreshQuestNotification();
    }


    public void RefreshQuestNotification()
    {
        var notification = questButton.transform.GetChild(0);
        
        if (questUIManager.haveNotification) { notification.gameObject.SetActive(true); }
        else { notification.gameObject.SetActive(false); }
    }
}
