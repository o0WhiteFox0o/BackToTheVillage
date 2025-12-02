// 
// Member: LinhH
// Date: 24/11/2025
// 


using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class UI_SettingUIManager : MonoBehaviour
{
    [SerializeField] public GameObject settingPanel;
    [SerializeField] public Button backButton;
    [SerializeField] public TMP_Dropdown languageDropdown;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider sfxSlider;


    private void Start()
    {
        var gameplayUIMgr = GetComponentInParent<UI_GameplayUIManager>();

        if (gameplayUIMgr == null)
        {
            Debug.LogError("Can't get component.");
        }


        backButton.onClick.AddListener(gameplayUIMgr.DisableUI);
        musicSlider.onValueChanged.AddListener(MusicSliderChange);
        sfxSlider.onValueChanged.AddListener(SfxSliderChange);

        SetupLanguageDropdown();
    }


    private void OnEnable()
    {
        SetupVolumeUISetting();
    }


    private void OnDisable()
    {
        musicSlider.onValueChanged.RemoveAllListeners();
        sfxSlider.onValueChanged.RemoveAllListeners();
        backButton.onClick.RemoveAllListeners();
        languageDropdown.onValueChanged.RemoveAllListeners();
    }


    private void MusicSliderChange(float value)
    {
        MGR_AudioManager.Instance.ChangeMusicVolume(value);
    }


    private void SfxSliderChange(float value)
    {
        MGR_AudioManager.Instance.ChangeSfxVolume(value);
    }


    private void SetupVolumeUISetting()
    {
        GameConfig gameConfig = GameConfig.LoadGameConfig();

        // thiết lập UI cho slider âm thanh
        musicSlider.value = gameConfig.musicVolume;
        sfxSlider.value = gameConfig.sfxVolume;
    }


    public void EnableSettingUI()
    {
        settingPanel.SetActive(true);
    }


    private void SetupLanguageDropdown()
    {
        List<string> options = new List<string>()
        {
            "Tiếng Việt",
            "English"
        };

        languageDropdown.ClearOptions();
        languageDropdown.AddOptions(options);

        // load ngôn ngữ từ file game config
        GameConfig gameConfig = GameConfig.LoadGameConfig();

        // thiết lập UI cho dropdown ngôn ngữ
        switch (gameConfig.languageId)
        {
            case "vi":
                languageDropdown.value = 0;
                break;

            case "en":
                languageDropdown.value = 1;
                break;
        }
        languageDropdown.RefreshShownValue();

        languageDropdown.onValueChanged.AddListener(ChangeLanguage);
    }


    private void ChangeLanguage(int index)
    {
        switch (index)
        {
            case 0:
                SYS_GameSetting.Instance.SetupGameLanguage("vi");
                break;

            case 1:
                SYS_GameSetting.Instance.SetupGameLanguage("en");
                break;

            default:
                break;
        }
    }
}
