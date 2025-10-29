using UnityEngine;

<<<<<<< HEAD
[CreateAssetMenu(fileName = "New Item", menuName = "BasicItem/Item")]
=======
[CreateAssetMenu(fileName = "New Item", menuName = "Item/Basic Item")]
>>>>>>> 8d5a0d410eeac573723646388f08a591b5b22e01
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
