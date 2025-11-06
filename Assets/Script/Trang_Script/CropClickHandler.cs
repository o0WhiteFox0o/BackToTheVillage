using UnityEngine;

[RequireComponent(typeof(Collider2D))]
public class CropClickHandler : MonoBehaviour
{
    private CropBehaviour crop;

    void Start()
    {
        // Tìm CropBehaviour ? cha
        crop = GetComponentInParent<CropBehaviour>();
    }

    void OnMouseDown()
    {
        if (crop == null) return;

        if (crop.isHarvestable)
        {
            crop.Harvest();
        }
        else
        {
            Debug.Log($"{crop.cropData.cropName} ch?a chín ?? thu ho?ch.");
        }
    }
}
