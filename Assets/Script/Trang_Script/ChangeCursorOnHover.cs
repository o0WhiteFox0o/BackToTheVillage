using UnityEngine;
using Management;
public class ChangeCursorOnHover : MonoBehaviour
{
    public Texture2D hoverCursor;  // icon khi hover
    private Texture2D defaultCursor;  // icon m?c ??nh
    public Vector2 hotSpot = Vector2.zero; // tâm con tr?

    void Start()
    {
        defaultCursor = null;
    }

    void OnMouseEnter()
    {
        var holdingItem = InventoryManager.Instance?.holdingItem;
        bool isNoItem = holdingItem == null;
        if (isNoItem)
        {
            Cursor.SetCursor(hoverCursor, hotSpot, CursorMode.Auto);
        }
    }

    void OnMouseExit()
    {
        Cursor.SetCursor(defaultCursor, Vector2.zero, CursorMode.Auto);
    }
}
