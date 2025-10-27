using UnityEngine;

[RequireComponent(typeof(SpriteRenderer))]
public class SortingOrderByY : MonoBehaviour
{
    private SpriteRenderer spriteRenderer;

    void Awake()
    {
        // L?y component SpriteRenderer g?n v?i object
        spriteRenderer = GetComponent<SpriteRenderer>();
    }

    void LateUpdate()
    {
        // C?p nh?t th? t? v? d?a trên v? trí Y
        // Giá tr? càng nh? (?i xu?ng d??i), layer càng cao ? hi?n th? phía tr??c
        spriteRenderer.sortingOrder = -(int)(transform.position.y * 100);
    }
}
