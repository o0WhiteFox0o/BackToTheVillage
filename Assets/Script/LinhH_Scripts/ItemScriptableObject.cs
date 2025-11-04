using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Basic Item")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Overview")]
    public string id;
    public Sprite icon;
    public string displayName;
    public bool stackable;
    public bool canSell;
    public ItemType itemType;
    [Header("Price")]
    public int buyPrice;
    public int sellPrice;
    [Header("Prefab của item có thể đặt xuống")]
    public GameObject itemPrefab;

    [Header("Seed Settings")]
    public GameObject plantPrefab;
}