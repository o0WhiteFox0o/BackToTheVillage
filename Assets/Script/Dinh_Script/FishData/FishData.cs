using UnityEngine;

[CreateAssetMenu(fileName = "New Fish", menuName = "Scriptable Object/Item/Fish Data")]
public class FishData : ItemScriptableObject
{
    [Header("Thông tin cơ bản")]
    public int rarity = 1;
    public float min_weight = 1f;
    public float min_length = 10f;
    public float max_weight = 20f;
    public float max_length = 25f;

    [Header("Độ khó QTE (Kéo co)")]
    [Range(0.5f, 5f)] public float qteBarSpeed = 1f;
    [Range(0.05f, 0.5f)] public float successWindowSize = 0.2f;
    [Range(5f, 50)] public float maxGameTime = 10f;
    [Range(0.01f, 0.2f)] public float progressIncrease = 0.1f;
    [Range(0.01f, 0.2f)] public float progressDecrease = 0.05f;

    private void OnValidate()
    {
        itemType = ItemType.Fish;
        stackable = true;
    }
}
