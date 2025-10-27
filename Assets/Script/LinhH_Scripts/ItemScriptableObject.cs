using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemScriptableObject : ScriptableObject
{
    [Header("Overview")]
    public string id;
    public Sprite icon;
    public bool stackable;
    public bool canSell;

    [Header("Price")]
    public int buyPrice;
    public int sellPrice;
}
