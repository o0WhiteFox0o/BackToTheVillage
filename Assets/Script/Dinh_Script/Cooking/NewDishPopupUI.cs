using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class NewDishPopupUI : MonoBehaviour
{
    [Header("UI Components")]
    [SerializeField] private GameObject panel;
    [SerializeField] private Image dishIcon;
    [SerializeField] private TMP_Text dishNameText;
    [SerializeField] private Button closeButton;

    // Hiệu ứng ánh sáng xoay phía sau (Optional)
    [SerializeField] private Transform sunburstEffect;

    private void Start()
    {
        closeButton.onClick.AddListener(ClosePopup);
        panel.SetActive(false);
    }

    private void Update()
    {
        // Làm hiệu ứng xoay nhẹ cho đẹp
        if (panel.activeSelf && sunburstEffect != null)
        {
            sunburstEffect.Rotate(Vector3.forward * 30f * Time.deltaTime);
        }
    }

    public void Show(CookingRecipeSO recipe)
    {
        panel.SetActive(true);

        dishIcon.sprite = recipe.resultItem.icon;
        dishNameText.text = recipe.dishName.GetLocalizedString(); // Hoặc "Món mới: " + recipe.dishName
    }

    private void ClosePopup()
    {
        panel.SetActive(false);

        // Sau khi đóng bảng chúc mừng, đóng luôn bảng nấu ăn chính
        // Để người chơi quay lại game ngắm thành quả
        CookingManager.Instance.CloseMainPanel();
    }
}