using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Perk", menuName = "Scriptable Objects/PerkSO")]
public class PerkSO : ScriptableObject
{
    [Header("Perk Detail")]
    public LocalizedString perkName;
    [TextArea (3, 5)]
    public LocalizedString perkDescription;
    public Sprite perkIcon;

    [Header("Định danh (Quan trọng)")]
    // Dùng để kiểm tra xem Player đã có Perk này chưa
    public string perkID;
}
