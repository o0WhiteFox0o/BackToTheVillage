using UnityEngine;
using UnityEngine.Tilemaps;

public class TileCursorFollow : MonoBehaviour
{
    public Tilemap targetTilemap;
    public Transform cursorObject;

    private Vector3Int lastCellPos;

    void Start()
    {
        lastCellPos = new Vector3Int(int.MinValue, int.MinValue, int.MinValue);
    }

    void Update()
    {
        if (cursorObject == null || targetTilemap == null)
            return;

        // N?u con tr? ?ang t?t thì không c?n c?p nh?t v? trí
        if (!cursorObject.gameObject.activeSelf)
            return;

        // L?y v? trí chu?t trên màn hình
        Vector3 mouseScreenPos = Input.mousePosition;
        mouseScreenPos.z = Mathf.Abs(Camera.main.transform.position.z);

        // Chuy?n sang t?a ?? th? gi?i
        Vector3 mouseWorldPos = Camera.main.ScreenToWorldPoint(mouseScreenPos);
        mouseWorldPos.z = 0;

        // Chuy?n sang t?a ?? tile
        Vector3Int cellPos = targetTilemap.WorldToCell(mouseWorldPos);

        // L?y tâm c?a ô tile
        Vector3 cellCenter = targetTilemap.GetCellCenterWorld(cellPos);

        // ??t object t?i ô ?ó
        cursorObject.position = cellCenter;

        // N?u con tr? ?ang ? ô m?i
        //if (cellPos != lastCellPos)
        //{
        //    lastCellPos = cellPos;
        //    TileBase tile = targetTilemap.GetTile(cellPos);

        //    if (tile != null)
        //        Debug.Log($"?ang tr? vào tile: {tile.name}");
        //    else
        //        Debug.Log("Ô tr?ng, không có tile ? ?ây.");
        //}
    }

    // ?? Hàm b?t/t?t con tr?
    public void SetCursorActive(bool active)
    {
        if (cursorObject != null)
            cursorObject.gameObject.SetActive(active);
    }
}
