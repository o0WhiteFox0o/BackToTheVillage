using UnityEngine;

[CreateAssetMenu(fileName = "NewCrop", menuName = "Farm/Crop Data (Prefab Version)")]
public class CropData : ScriptableObject
{
    [Header("Thông tin c? b?n")]
    public string cropName;

    [Tooltip("Prefab c?a t?ng giai ?o?n (ví d?: 4 ho?c 5 prefab)")]
    public GameObject[] growthPrefabs;

    [Tooltip("S? ngày c?n ?? chuy?n sang prefab ti?p theo. Ph?i có cùng ?? dài v?i growthPrefabs.")]
    public int[] daysPerStage;

    [Tooltip("Cây này có th? thu ho?ch nhi?u l?n không?")]
    public bool isRegrowable;

    [Header("N?u là cây thu ho?ch nhi?u l?n")]
    [Tooltip("S? ngày ?? m?c l?i (?? quay v? giai ?o?n tr??c cu?i)")]
    public int regrowDays;

    [Header("S?n ph?m thu ???c")]
    public ItemScriptableObject harvestItem;
}
