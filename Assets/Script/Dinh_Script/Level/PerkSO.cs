using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Localization;

[CreateAssetMenu(fileName = "New Perk", menuName = "Scriptable Objects/PerkSO")]
public class PerkSO : ScriptableObject
{
    [Header("Perk Detail")]
    // Dùng để kiểm tra xem Player đã có Perk này chưa
    public string perkID;
    public LocalizedString perkName;
    public LocalizedString perkDescription;
    public Sprite perkIcon;

    [System.Serializable]
    public class StatModifier
    {
        public StatType statType;
        [Tooltip("Giá trị cộng thêm. VD: 0.25 là +25%, -0.1 là giảm 10%")]
        public float valueToAdd;
    }
    [Header("Stat Modifiers")]
    public List<StatModifier> modifiers;
}
