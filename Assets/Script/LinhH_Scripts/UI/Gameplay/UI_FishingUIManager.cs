// 
// Member: LinhH
// Date: 18/12/2025
// 


using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;


public class UI_FishingUIManager : MonoBehaviour
{
    [Header("Panels")]
    [SerializeField] private GameObject castingPanel;
    [SerializeField] private GameObject caughtFishPanel;

    [Header("Images")]
    [SerializeField] private Image castingBar;
    [SerializeField] private Image fishingIcon;
    [SerializeField] private GameObject fishingExclamation;

    [Header("Texts")]
    [SerializeField] private TMP_Text fishNameText;
    [SerializeField] private TMP_Text fishWeightText;
    [SerializeField] private TMP_Text fishLengthText;

    [Header("Prefabs")]
    [SerializeField] private GameObject fishIconPrefab;

    [SerializeField] private float caughtFishDisplayTime = 3.0f;


    private void Start()
    {
        ToggleCastingPanel(false);
    }


    public void ToggleCastingPanel(bool enable)
    {
        castingPanel.SetActive(false);
    }


    public void ToggleFishingExclamation(bool enable)
    {
        fishingExclamation.SetActive(enable);
    }


    public void ToggleCaughtFishPanel(bool enable)
    {
        caughtFishPanel.SetActive(enable);
    }


    public void FillCastingBar(float progress)
    {
        castingPanel.SetActive(true);
        castingBar.fillAmount = progress;
    }


    private IEnumerator ShowCaughtFishUI(FishData fish)
    {
        if (fish == null) yield break;

        float randomWeight = Random.Range(fish.min_weight, fish.max_weight);    // Giả sử là minWeight
        float randomLength = Random.Range(fish.min_length, fish.max_length);    // Giả sử là minLength

        fishNameText.text = fish.displayName.GetLocalizedString();
        fishingIcon.sprite = fish.icon;
        fishWeightText.text = $"Nặng: {randomWeight:F1} kg";
        fishLengthText.text = $"Dài: {randomLength:F1} cm";

        ToggleCaughtFishPanel(true);

        yield return new WaitForSeconds(caughtFishDisplayTime);

        ToggleCaughtFishPanel(false);
    }


    public void DisplayCaughtFishUI(FishData fishData)
    {
        StopAllCoroutines();
        StartCoroutine(ShowCaughtFishUI(fishData));

        Debug.Log("Test: caught fish");
    }
}
