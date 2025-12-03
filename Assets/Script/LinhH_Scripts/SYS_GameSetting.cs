// 
// Member: LinhH
// Date: 24/11/2025
// 


using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization.Settings;

public class SYS_GameSetting : MonoBehaviour
{
    public static SYS_GameSetting Instance;


    private void Start()
    {
        if (Instance == null)
        {
            Instance = this;
        }

        DontDestroyOnLoad(this);

        LoadGameLanguage();
    }


    private void LoadGameLanguage()
    {
        var gameConfig = GameConfig.LoadGameConfig();
        SetupGameLanguage(gameConfig.languageId);
    }


    public void SetupGameLanguage(string localeId)
    {
        var locale = LocalizationSettings.AvailableLocales.GetLocale(localeId);

        if (locale != null)
        {
            LocalizationSettings.SelectedLocale = locale;
        }
    }


    /// <summary>
    /// Hàm test chức năng save game config.
    /// </summary>
    // TEST - LinhH - 02/12/2025 - Đang gán cho button Eixt trong gameplay UI (General UI).
    public void TestSaveGameConfig()
    {
        GameConfig gameConfig = new GameConfig();

        // save ngôn ngữ trò chơi
        var langId = LocalizationSettings.SelectedLocale.Identifier.Code;
        gameConfig.languageId = langId;

        // save phím tắt
        List<HotkeyConfig> hotkeyConfigs = new List<HotkeyConfig>();
        foreach (var hotkey in SYS_InputManager.Instance.keyBindings)
        {
            hotkeyConfigs.Add(
                new HotkeyConfig
                {
                    action = hotkey.Key,
                    keyCode = hotkey.Value.ToString()
                }
            );
        }

        gameConfig.hotkeys = hotkeyConfigs;

        // lấy giá trị của âm thanh từ Audio Mixer
        MGR_AudioManager.Instance.audioMixer.GetFloat("Music", out gameConfig.musicVolume);
        MGR_AudioManager.Instance.audioMixer.GetFloat("SFX", out gameConfig.sfxVolume);

        // tính toán lại giá trị của âm thanh 
        gameConfig.musicVolume = Mathf.Pow(10, gameConfig.musicVolume / 20);
        gameConfig.sfxVolume = Mathf.Pow(10, gameConfig.sfxVolume / 20);

        // Lưu thiết lập của trò chơi
        gameConfig.SaveGameConfig();
    }
}
