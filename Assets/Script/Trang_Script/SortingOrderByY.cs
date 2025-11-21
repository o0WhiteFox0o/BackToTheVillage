using UnityEngine;
using UnityEngine.Rendering; // b?t bu?c ?? dùng SortingGroup

[RequireComponent(typeof(SortingGroup))]
public class SortingOrderByY : MonoBehaviour
{
    private SortingGroup sortingGroup;

    void Awake()
    {
        sortingGroup = GetComponent<SortingGroup>();
    }

    void LateUpdate()
    {
        sortingGroup.sortingOrder = -(int)(transform.position.y * 100);
    }
}
