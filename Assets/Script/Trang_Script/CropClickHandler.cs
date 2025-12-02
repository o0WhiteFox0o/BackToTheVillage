using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CropClickHandler : MonoBehaviour
{
    private CropBehaviour crop;

    void Start()
    {
        crop = GetComponentInParent<CropBehaviour>();

        // ??ng ký s? ki?n chu?t trái t? InputManager
        InputManager.OnLeftClickCrop += HandleLeftClickOnCrop;
    }

    void OnDestroy()
    {
        // H?y ??ng ký khi object b? destroy
        InputManager.OnLeftClickCrop -= HandleLeftClickOnCrop;
    }

    private void HandleLeftClickOnCrop(GameObject clickedCrop)
    {
        // Ch? quan tâm ??n crop c?a chính object này
        if (clickedCrop != gameObject) return;

        if (crop != null)
        {
            if (crop.isHarvestable)
            {
                crop.Harvest();

                if (Player.Instance != null) {
                    Player.Instance.PlayToolAnimation("isHarvest");
                }
            }
            else
            {
                Debug.Log($"{crop.cropData.cropName} ch?a chín ?? thu ho?ch.");
            }
        }
    }
}
