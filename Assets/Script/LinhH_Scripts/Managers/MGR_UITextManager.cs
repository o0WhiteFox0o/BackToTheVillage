// 
// Member: LinhH
// Date: 18/11/2025
// 


using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.Localization.Settings;

public class MGR_UITextManager : MonoBehaviour
{
    [Header("General UI Texts")]
    [SerializeField] public TMP_Text generalCharacterButton_Text;
    [SerializeField] public TMP_Text generalQuestButton_Text;
    [SerializeField] public TMP_Text generalSettingButton_Text;


    private void Start()
    {
        LocalizationSettings.SelectedLocaleChanged += ChangeTextLanguage;
    }


    private void OnDisable()
    {
        LocalizationSettings.SelectedLocaleChanged -= ChangeTextLanguage;
    }


    private void ChangeTextLanguage(Locale locale)
    {
        // load text từ localization table (từ bảng GameplayMessage, key gMgs.characterBtn)
        generalCharacterButton_Text.SetText(new LocalizedString("GameplayMessage", "gMgs.characterBtn").GetLocalizedString());
        generalCharacterButton_Text.SetText(new LocalizedString("GameplayMessage", "gMgs.questBtn").GetLocalizedString());
        generalCharacterButton_Text.SetText(new LocalizedString("GameplayMessage", "gMgs.settingBtn").GetLocalizedString());
    }
}
