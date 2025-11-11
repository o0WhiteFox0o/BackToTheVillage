using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Qu?n lý tr?ng thái các ô ??t: ?ã cu?c, ?ã t??i, ?ã tr?ng
/// </summary>
public class SoilManager : MonoBehaviour
{
    // Các t?p v? trí riêng bi?t
    private HashSet<Vector3Int> hoedTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> wateredTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> plantedTiles = new HashSet<Vector3Int>();

    // ========== ??T ?Ã CU?C ==========
    public void AddHoed(Vector3Int pos)
    {
        hoedTiles.Add(pos);
    }

    public bool IsHoed(Vector3Int pos)
    {
        return hoedTiles.Contains(pos);
    }

    // ========== ??T ?Ã T??I ==========
    public void AddWatered(Vector3Int pos)
    {
        wateredTiles.Add(pos);
    }

    public bool IsWatered(Vector3Int pos)
    {
        return wateredTiles.Contains(pos);
    }

    // ========== CÂY ?Ã TR?NG ==========
    public void AddPlanted(Vector3Int pos)
    {
        plantedTiles.Add(pos);
    }

    public bool IsPlanted(Vector3Int pos)
    {
        return plantedTiles.Contains(pos);
    }

    // ========== IN TOÀN B? THÔNG TIN ==========
    public void PrintAll()
    {
        Debug.Log("===== ?? Tr?ng thái ??t =====");

        Debug.Log("? Ô ?ã cu?c:");
        foreach (var pos in hoedTiles)
            Debug.Log(pos);

        Debug.Log("?? Ô ?ã t??i:");
        foreach (var pos in wateredTiles)
            Debug.Log(pos);

        Debug.Log("?? Ô ?ã tr?ng:");
        foreach (var pos in plantedTiles)
            Debug.Log(pos);
    }

    // ========== XÓA D? LI?U ==========
    public void ClearAll()
    {
        hoedTiles.Clear();
        wateredTiles.Clear();
        plantedTiles.Clear();
    }
}
