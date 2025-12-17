// 
// Member: LinhH
// Date: 09/12/2025
// 


using System.IO;
using UnityEngine;
using UnityEngine.UI;

public class UI_SavedGameUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public GameObject background;
    [SerializeField] private Transform savedGamePanel;

    [Header("Buttons")]
    [SerializeField] public Button backButton;

    [Header("Prefabs")]
    [SerializeField] private GameObject savedGameUI_Prefab;

    private UI_MainMenuUIManager mainMenuUIManager;
    private MGR_MainMenuManager mainMenuManager;


    void Start()
    {
        mainMenuUIManager = GetComponentInParent<UI_MainMenuUIManager>();
        mainMenuManager = FindObjectOfType<MGR_MainMenuManager>();

        if (mainMenuUIManager == null || mainMenuManager == null)
        {
            Debug.LogError("Can't load component!!!");
        }

        backButton.onClick.AddListener(mainMenuUIManager.DisableUI);
    }


    void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();
    }


    private void LoadSavedFarmList()
    {
        // reset farm list
        foreach (Transform savedGame in savedGamePanel)
        {
            MGR_ObjectPoolManager.ReturnObjectToPool(savedGame.gameObject);
        }

        // load các file có trong thư mục saved farm
        string savedGamesPath = Path.Combine(Application.streamingAssetsPath, GameConstants.SAVED_GAMES_FOLDER);
        if (!Directory.Exists(savedGamesPath)) { return; }

        // tạo các ui saved game dựa vào các file load được
        var files = Directory.GetFiles(savedGamesPath);
        foreach (var file in files)
        {
            // bỏ qua các file .meta
            if (file[file.Length - 1] == 'a') { continue; }

            var savedGameUI_GO = MGR_ObjectPoolManager.SpawnObject(savedGameUI_Prefab, savedGamePanel);

            // load thông tin của game đã lưu
            var savedGameConfig = SavedGameConfig.LoadSavedGameByPath(file);
            var savedGameUI = savedGameUI_GO.GetComponent<UI_SavedGameUI>();

            // thiết lập các thông tin game lên UI
            savedGameUI.SetupSavedFarmDetails(savedGameConfig);

            // đang ký các sự kiện cần thiết cho button
            savedGameUI.deleteFarmButton.onClick.AddListener(() => mainMenuManager.DeleteSavedGame(savedGameConfig));
            savedGameUI.deleteFarmButton.onClick.AddListener(LoadSavedFarmList);

            savedGameUI.loadFarmButton.onClick.AddListener(() => mainMenuManager.LoadGame(savedGameConfig));
        }
    }


    public void EnableSavedFarmUI()
    {
        background.SetActive(true);
        LoadSavedFarmList();
    }
}
