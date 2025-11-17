using UnityEngine;
using Management;
public class CropBehaviour : MonoBehaviour
{
    public CropData cropData;         // Dữ liệu ScriptableObject của cây
    public int currentStage = 0;      // Giai đoạn hiện tại
    public int daysInCurrentStage = 0; // Số ngày đã trôi qua trong giai đoạn này
    public bool isHarvestable = false;

    private GameObject currentPrefab;  // Prefab hiện tại đang hiển thị

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

        daysInCurrentStage++;

        // Kiểm tra nếu đủ ngày để qua giai đoạn mới
        if (daysInCurrentStage >= cropData.daysPerStage[currentStage])
        {
            daysInCurrentStage = 0;
            currentStage++;

            // Nếu vượt qua giai đoạn cuối
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

        // ✅ Tìm InventoryManager đang có trong scene
        var inventoryManager = FindObjectOfType<Management.InventoryManager>();
        if (inventoryManager != null)
        {
            inventoryManager.AddItem(cropData.harvestItem, 1);
            Debug.Log($"Đã thu hoạch: {cropData.harvestItem.displayName}");
        }
        else
        {
            Debug.LogWarning("Không tìm thấy InventoryManager trong scene!");
        }

        // ✅ Nếu cây có thể tái sinh
        if (cropData.isRegrowable)
        {
            isHarvestable = false;
            currentStage = cropData.growthPrefabs.Length - 2; // quay về giai đoạn trước chín
            daysInCurrentStage = 0;
            UpdateVisual();
        }
        else
        {
            // ✅ Nếu cây chỉ thu hoạch 1 lần thì hủy
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
