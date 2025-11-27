// 
// Member: LinhH
// Date: 24/11/2025
// 


using UnityEngine;
using UnityEngine.Localization.Settings;

public class SYS_GameSetting : MonoBehaviour
{
    public static SYS_GameSetting Instance;


    private void Start() {
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
}
