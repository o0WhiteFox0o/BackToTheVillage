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
    [Header("Panels")]
    [SerializeField] public GameObject settingPanel;
    [SerializeField] public Transform hotkeySettingPanel;

    [Header("Buttons")]
    [SerializeField] public Button backButton;
    [SerializeField] public TMP_Dropdown languageDropdown;
    [SerializeField] public Slider musicSlider;
    [SerializeField] public Slider sfxSlider;

    public bool haveNotification { get; private set; }

    public bool firstTimeOpened { get; private set; }


    private void Start()
    {
        firstTimeOpened = true;

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


    /// <summary>
    /// Thiết lập UI cho cài đặt âm lượng. Gọi khi giao diện cài đặt được mở lần đầu.
    /// </summary>
    public void SetupVolumeUI()
    {
        GameConfig gameConfig = GameConfig.LoadGameConfig();

        // thiết lập UI cho slider âm thanh
        musicSlider.value = gameConfig.musicVolume;
        sfxSlider.value = gameConfig.sfxVolume;
    }


    /// <summary>
    /// Thiết lập UI cho cài đặt phím tắt. Gọi khi giao diện cài đặt được mở lần đầu.
    /// </summary>
    public void SetupHotkeyUI()
    {
        // load danh sách hotkey từ file game config
        var hotkeyConfig = GameConfig.LoadGameConfig().hotkeys;

        // duyệt qua các phím tắt trong giao diện cài đặt
        for (int i = 0; i < hotkeySettingPanel.childCount; i++)
        {
            var hotkeySetting = hotkeySettingPanel.GetChild(i);

            // kiểm tra xem trong file game config có chứa hotkey với chức năng trong UI không, không thì bỏ qua
            var hotkey = hotkeyConfig.Find(k => k.action == hotkeySetting.gameObject.name);
            if (hotkey == null) { continue; }

            var hotkeyButton = hotkeySetting.GetComponentInChildren<Button>();

            // TODO: đăng ký sự kiện cần thiết cho button hotkey

            // thiết lập hotkey text cho nút hotkey setting
            hotkeyButton.GetComponentInChildren<TMP_Text>().SetText(hotkey.keyCode);
        }

        firstTimeOpened = false;
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
