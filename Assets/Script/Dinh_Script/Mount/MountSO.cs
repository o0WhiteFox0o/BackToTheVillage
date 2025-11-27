using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

public enum MountType
{
    Land,
    Water,
}

[CreateAssetMenu(fileName = "New Mount", menuName = "Scriptable Object/MountSO")]
public class MountSO : ScriptableObject
{
    public LocalizedString mountName;
    public MountType type;
    public float speedMultiplier = 1.5f;
    [Tooltip("Prefab của mount")]
    public GameObject mountIcon;
}
