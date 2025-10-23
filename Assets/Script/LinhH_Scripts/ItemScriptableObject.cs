using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Item")]
public class ItemScriptableObject : ScriptableObject
{
    public string id;
    public Sprite icon;
    public bool stackable;
    public bool canSell;

    [Header("Price")]
    public int buyPrice;
    public int sellPrice;
}
