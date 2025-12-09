// 
// Member: LinhH
// Date: 09/12/2025
// 


using TMPro;
using UnityEngine;
using UnityEngine.Localization;
using UnityEngine.UI;

public class UI_SavedGameUI : MonoBehaviour
{
    [Header("Buttons")]
    [SerializeField] public Button deleteFarmButton;
    [SerializeField] public Button loadFarmButton;

    [Header("Texts")]
    [SerializeField] private TMP_Text farmNameText;
    [SerializeField] private TMP_Text characterNameText;
    [SerializeField] private TMP_Text playedTimeText;
    [SerializeField] private TMP_Text gameTimeText;
    [SerializeField] private TMP_Text currencyText;


    void OnDisable()
    {
        loadFarmButton.onClick.RemoveAllListeners();
        deleteFarmButton.onClick.RemoveAllListeners();
    }


    public void SetupSavedFarmDetails(SavedGameConfig savedGame)
    {
        if (savedGame == null)
        {
            Debug.LogError($"Can't load saved game!!!");
            return;
        }

        LocalizedString localizedString = new LocalizedString();

        localizedString.SetReference("GameplayMessage", "gMsg.savedGame.farmName");
        farmNameText.SetText(localizedString.GetLocalizedString() + savedGame.farmName);

        localizedString.SetReference("GameplayMessage", "gMsg.savedGame.characterName");
        characterNameText.SetText(localizedString.GetLocalizedString() + savedGame.characterName);

        localizedString.SetReference("GameplayMessage", "gMsg.savedGame.playedTime");
        playedTimeText.SetText(localizedString.GetLocalizedString() + savedGame.playedTime.ToString());

        currencyText.SetText(savedGame.currency.ToString() + "vnđ");
    }
}
