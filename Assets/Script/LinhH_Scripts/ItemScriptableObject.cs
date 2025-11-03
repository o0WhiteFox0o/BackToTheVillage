using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item/Basic Item")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Overview")]
    public string id;
    public Sprite icon;
    public bool stackable;
    public bool canSell;
    public ItemType itemType;
    [Header("Price")]
    public int buyPrice;
    public int sellPrice;
}
