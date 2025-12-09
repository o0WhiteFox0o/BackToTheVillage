// 
// Member: LinhH
// Date: 09/12/2025
// 


using UnityEngine;
using UnityEngine.UI;

public class UI_StartUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public GameObject background;

    [Header("Buttons")]
    [SerializeField] public Button backButton;

    private UI_MainMenuUIManager mainMenuUIManager;


    void Start()
    {
        mainMenuUIManager = GetComponentInParent<UI_MainMenuUIManager>();

        if (mainMenuUIManager == null)
        {
            Debug.LogError("Can't load component!!!");
        }

        backButton.onClick.AddListener(mainMenuUIManager.DisableUI);
    }


    void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();
    }


    public void EnableStartUI()
    {
        background.SetActive(true);
    }
}
