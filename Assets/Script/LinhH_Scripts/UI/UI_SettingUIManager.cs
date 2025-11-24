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
    [SerializeField] public TMP_Dropdown langDropdown;


    private void Start()
    {
        var gameplayUIMgr = GetComponentInParent<UI_GameplayUIManager>();

        if (gameplayUIMgr == null)
        {
            Debug.LogError("Can't get component.");
        }


        backButton.onClick.AddListener(gameplayUIMgr.ToggleSettingUI);

        SetupLanguageDropdown();
    }


    private void OnDisable()
    {
        backButton.onClick.RemoveAllListeners();
    }


    public void EnableSettingUI(bool enable)
    {
        settingPanel.SetActive(enable);
    }


    private void SetupLanguageDropdown()
    {
        List<string> options = new List<string>()
        {
            "Tiếng Việt",
            "English"
        };

        langDropdown.ClearOptions();
        langDropdown.AddOptions(options);

        langDropdown.onValueChanged.AddListener(ChangeLanguage);
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
