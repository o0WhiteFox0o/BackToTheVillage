using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// Qu?n lý tr?ng thái các ô ??t: ?ã cu?c, ?ã t??i, ?ã tr?ng
/// </summary>
public class SoilManager : MonoBehaviour
{
    private HashSet<Vector3Int> hoedTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> wateredTiles = new HashSet<Vector3Int>();
    private HashSet<Vector3Int> plantedTiles = new HashSet<Vector3Int>();

    // ========== ??t ?ã cu?c ==========
    public void AddHoed(Vector3Int pos) => hoedTiles.Add(pos);
    public bool IsHoed(Vector3Int pos) => hoedTiles.Contains(pos);
    public void ClearHoedTiles() => hoedTiles.Clear();

    // ========== ??t ?ã t??i ==========
    public void AddWatered(Vector3Int pos) => wateredTiles.Add(pos);
    public bool IsWatered(Vector3Int pos) => wateredTiles.Contains(pos);
    public void ClearWateredTiles() => wateredTiles.Clear();

    // ========== Cây ?ã tr?ng ==========
    public void AddPlanted(Vector3Int pos) => plantedTiles.Add(pos);
    public bool IsPlanted(Vector3Int pos) => plantedTiles.Contains(pos);
    public void ClearPlantedTiles() => plantedTiles.Clear();

    // ========== In toàn b? tr?ng thái ==========\
    public void PrintAll()
    {
        Debug.Log("===== Tr?ng thái ??t =====");

        Debug.Log("?? Ô ?ã cu?c:");
        foreach (var pos in hoedTiles) Debug.Log(pos);

        Debug.Log("?? Ô ?ã t??i:");
        foreach (var pos in wateredTiles) Debug.Log(pos);

        Debug.Log("?? Ô ?ã tr?ng:");
        foreach (var pos in plantedTiles) Debug.Log(pos);
    }

    // ========== Xóa toàn b? d? li?u ==========
    public void ClearAll()
    {
        ClearHoedTiles();
        ClearWateredTiles();
        ClearPlantedTiles();
    }
}
