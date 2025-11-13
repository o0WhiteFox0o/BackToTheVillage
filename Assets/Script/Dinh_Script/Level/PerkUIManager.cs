using UnityEngine;
using UnityEngine.UI;

public class PerkUIManager : MonoBehaviour
{
    public static PerkUIManager Instance { get; private set; }

    // (Tham chiếu đến Panel UI lựa chọn của bạn)
    public GameObject professionChoicePanel; 
    public GameObject professionChoiceLevelA, professionChoiceLevelB, professionChoiceLevelC, professionChoiceLevelD;
    public Button perkA_Button, perkB_Button;

    void Awake()
    {
        if (Instance != null) Destroy(gameObject);
        else Instance = this;
    }
    void Start()
    {
        professionChoicePanel.SetActive(false);
    }

    public void ShowChoice(ProfessionChoice choice)
    {
        Debug.LogWarning($"[PerkUIManager] HIỂN THỊ LỰA CHỌN CẤP {choice.levelToUnlock}: {choice.choiceTitle}");
        Debug.Log($"Lựa chọn A: {choice.perkA.perkName}");
        Debug.Log($"Lựa chọn B: {choice.perkB.perkName}");

        if(choice.levelToUnlock == 5)
        {
            professionChoiceLevelA.SetActive(true);
            professionChoiceLevelB.SetActive(true);
            professionChoiceLevelC.SetActive(false);
            professionChoiceLevelD.SetActive(false);
        }
        else if (choice.levelToUnlock == 10)
        {
            professionChoiceLevelA.SetActive(false);
            professionChoiceLevelB.SetActive(false);
            professionChoiceLevelC.SetActive(true);
            professionChoiceLevelD.SetActive(true);
        }
        professionChoicePanel.SetActive(true);
        Time.timeScale = 0f; // Dừng game

        // Gán thông tin và sự kiện cho 2 nút bấm
        perkA_Button.onClick.AddListener(() => { SelectPerk(choice.perkA); });
        perkB_Button.onClick.AddListener(() => { SelectPerk(choice.perkB); });
    }

    public void SelectPerk(PerkSO chosenPerk)
    {
         Debug.Log($"[PerkUIManager] Bạn đã chọn Perk: {chosenPerk.perkName}!");
         professionChoicePanel.SetActive(false);
         Time.timeScale = 1f;
    }
}