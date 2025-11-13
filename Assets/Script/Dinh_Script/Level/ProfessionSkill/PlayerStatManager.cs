using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerStatManager : MonoBehaviour
{
    public static PlayerStatManager Instance { get; private set; }

    private Dictionary<StatType, float> currentStats = new Dictionary<StatType, float>();
    private void Awake()
    {
        if (Instance != null && Instance != this)
        {
            Destroy(this.gameObject);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(this.gameObject);
        }
    }

    public void ApplyProfessionBonus(PerkSO profession) { 
        if(profession == null) return;
        foreach (var modifier in profession.modifiers)
        {
            if (!currentStats.ContainsKey(modifier.statType))
            {
                currentStats[modifier.statType] = 1.0f;
            }
            currentStats[modifier.statType] += modifier.valueToAdd;
        }
        Debug.Log("Áp dụng chỉ số từ " + profession.perkID);
    }

    public float GetStatValue(StatType type, float defaultValue = 1.0f)
    {
       if(currentStats.ContainsKey(type))
       {
            return currentStats[type];
        }
        return defaultValue;
    }
}
