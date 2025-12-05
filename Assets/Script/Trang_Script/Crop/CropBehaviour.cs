
﻿using UnityEngine;
using Management;
public class CropBehaviour : MonoBehaviour
{
    public CropData cropData;         // Dữ liệu ScriptableObject của cây
    public int currentStage = 0;      // Giai đoạn hiện tại
    public int daysInCurrentStage = 0; // Số ngày đã trôi qua trong giai đoạn này
    public bool isHarvestable = false;

    private GameObject currentPrefab;
    // Prefab hiện tại đang hiển thị
    public SoilManager soilManager;    // tham chiếu đến SoilManager
    public Vector3Int cellPosition;
    void Start()
    {
        if (cropData != null)
        {
            LoadStage(currentStage);
        }
    }

    // Hàm này được gọi mỗi ngày (DayNight gọi GrowAllCrops)
    public void GrowOneDay()
    {
        if (cropData == null || isHarvestable) return;

        // --- KIỂM TRA ĐẤT TƯỚI ---
        if (soilManager != null && !soilManager.IsWatered(cellPosition))
        {
            Debug.Log($"{cropData.cropName} tại {cellPosition} không được tưới, không lớn hôm nay.");
            return; // cây không tăng stage nếu đất chưa tưới
        }

        // --- Tăng stage như bình thường ---
        daysInCurrentStage++;

        if (daysInCurrentStage >= cropData.daysPerStage[currentStage])
        {
            daysInCurrentStage = 0;
            currentStage++;

            if (currentStage >= cropData.growthPrefabs.Length)
            {
                currentStage = cropData.growthPrefabs.Length - 1;
                isHarvestable = true;
                Debug.Log($"{cropData.cropName} đã trưởng thành và có thể thu hoạch!");
            }
            else
            {
                LoadStage(currentStage);
            }
        }

    }


    // Tải prefab cho giai đoạn hiện tại
    private void LoadStage(int stageIndex)
    {
        // Xoá prefab cũ
        if (currentPrefab != null)
            Destroy(currentPrefab);

        // Sinh prefab mới
        currentPrefab = Instantiate(cropData.growthPrefabs[stageIndex], transform);
        currentPrefab.transform.localPosition = Vector3.zero;
    }

    public void Harvest()
    {
        if (!isHarvestable) return;

        // ⭐ Spawn item rơi ra đất thay vì add ngay
        if (cropData.droppedItemPrefab != null)
        {
            GameObject drop = Instantiate(cropData.droppedItemPrefab, transform.position, Quaternion.identity);

            PickupItem pickup = drop.GetComponent<PickupItem>();
            if (pickup != null)
            {
                pickup.itemData = cropData.harvestItem;
                pickup.quantity = 1;
            }

            Debug.Log($"Item rơi ra: {cropData.harvestItem.displayName}");
        }
        else
        {
            Debug.LogWarning("Chưa gán droppedItemPrefab trong CropData!");
        }

        // ⭐ Xử lý cây tái sinh hoặc hủy
        if (cropData.isRegrowable)
        {
            isHarvestable = false;
            currentStage = cropData.growthPrefabs.Length - 2;
            daysInCurrentStage = 0;
            UpdateVisual();
        }
        else
        {
            Destroy(gameObject);
        }
    }



    private void UpdateVisual()
    {
        if (currentPrefab != null)
            Destroy(currentPrefab);

        if (cropData.growthPrefabs != null && currentStage < cropData.growthPrefabs.Length)
            currentPrefab = Instantiate(cropData.growthPrefabs[currentStage], transform);
    }
}
