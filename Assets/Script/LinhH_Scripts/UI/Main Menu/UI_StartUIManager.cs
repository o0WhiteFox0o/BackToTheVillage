// 
// Member: LinhH
// Date: 09/12/2025
// 


using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


[RequireComponent(typeof(UI_CharacterAppearanceUI))]
public class UI_StartUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] public GameObject background;
    [SerializeField] public GameObject incompleteSetupMessage;

    [Header("Buttons")]
    [SerializeField] public Button backButton;
    [SerializeField] private Button startButton;


    [Header("Input Fields")]
    [SerializeField] private TMP_InputField farmNameInput;
    [SerializeField] private TMP_InputField characterNameInput;
    [SerializeField] private TMP_InputField favoriteInput;

    [Header("Audio Clips")]
    [SerializeField] private AudioClip incompleteSetupSFX;


    private UI_MainMenuUIManager mainMenuUIManager;
    private MGR_MainMenuManager mainMenuManager;
    private UI_CharacterAppearanceUI characterAppearanceUI;


    void Start()
    {
        mainMenuUIManager = GetComponentInParent<UI_MainMenuUIManager>();
        mainMenuManager = FindObjectOfType<MGR_MainMenuManager>();
        characterAppearanceUI = GetComponent<UI_CharacterAppearanceUI>();

        if (mainMenuUIManager == null || mainMenuManager == null || characterAppearanceUI == null)
        {
            Debug.LogError("Can't load component!!!");
        }

        backButton.onClick.AddListener(mainMenuUIManager.DisableUI);
        startButton.onClick.AddListener(StartGame);
    }


    void OnDestroy()
    {
        backButton.onClick.RemoveAllListeners();
        startButton.onClick.RemoveAllListeners();
    }


    public void EnableStartUI()
    {
        background.SetActive(true);
        characterAppearanceUI.SelectGender(true);
        characterAppearanceUI.RefreshCharacterAppearance();
    }


    private void StartGame()
    {
        if (farmNameInput.text.Length == 0 || characterNameInput.text.Length == 0 || favoriteInput.text.Length == 0)
        {
            MGR_AudioManager.Instance.PlaySFX(incompleteSetupSFX);
            StartCoroutine(DisplayIncompleteSetupMessage());
            return;
        }

        mainMenuManager.StartNewGame(farmNameInput.text, characterNameInput.text);
    }


    private IEnumerator DisplayIncompleteSetupMessage()
    {
        incompleteSetupMessage.SetActive(true);

        yield return new WaitForSeconds(1.25f);

        incompleteSetupMessage.SetActive(false);
    }
}