
using UnityEngine;
using Management;
[RequireComponent(typeof(Collider2D))]
public class CropClickHandler : MonoBehaviour
{
    private CropBehaviour crop;

    void Start()
    {
        crop = GetComponentInParent<CropBehaviour>();

        // ??ng k? s? ki?n chu?t tr?i t? InputManager
        SYS_InputManager.OnLeftClickCrop += HandleLeftClickOnCrop;
    }

    void OnDestroy()
    {
        // H?y ??ng k? khi object b? destroy
        SYS_InputManager.OnLeftClickCrop -= HandleLeftClickOnCrop;
    }

    private void HandleLeftClickOnCrop(GameObject clickedCrop)
    {
        // Ch? quan t?m ??n crop c?a ch?nh object n?y
        if (clickedCrop != gameObject) return;

        if (crop != null)
        {
            if (crop.isHarvestable)
            {
                crop.Harvest();

                if (Player.Instance != null)
                {
                    Player.Instance.PlayToolAnimation("isHarvest");
                }
            }
            else
            {
                Debug.Log($"{crop.cropData.cropName} ch?a ch?n ?? thu ho?ch.");
            }
        }
    }
}