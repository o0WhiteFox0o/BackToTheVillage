using System.Collections.Generic;
using UnityEngine;


 public class PlantedManager : MonoBehaviour
{
    // ? S? d?ng HashSet ?? ki?m tra nhanh
    private List<Vector3Int> plantedPositions = new List<Vector3Int>();

    public void AddPosition(Vector3Int pos)
    {
        plantedPositions.Add(pos);
    }

    public bool IsPositionOccupied(Vector3Int pos)
    {
        return plantedPositions.Contains(pos);
    }

    /// <summary>
    /// L?y danh sách t?t c? các v? trí ?ã tr?ng
    /// </summary>
    public List<Vector3Int> GetAllPositions()
    {
        return new List<Vector3Int>(plantedPositions);
    }

    /// <summary>
    /// Xóa t?t c? d? li?u (n?u c?n reset)
    /// </summary>
    public void Clear()
    {
        plantedPositions.Clear();
    }
}
