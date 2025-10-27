using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCursorFollow : MonoBehaviour
{
    public Tilemap targetTilemap;     // Tilemap chính
    public Transform cursorObject;    // Object di chuy?n theo chu?t

    void Update()
    {
        // L?y v? trí chu?t trên màn hình
        Vector3 mouseScreenPos = Input.mousePosition;

        // Quan tr?ng: gán z = kho?ng cách t? camera ??n tilemap
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // Chuy?n sang t?a ?? th? gi?i
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0; // ??m b?o nó n?m trên m?t ph?ng 2D

        // Chuy?n sang t?a ?? tile
        Vector3Int cellPos = targetTilemap.WorldToCell(mouseWorldPos);

        // L?y tâm c?a ô tile
        Vector3 cellCenter = targetTilemap.GetCellCenterWorld(cellPos);

        // ??t object t?i ô ?ó
        cursorObject.position = cellCenter;
    }
}
