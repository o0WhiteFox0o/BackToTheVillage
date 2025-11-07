// 
// Member: LinhH
// Date: 06/11/2025
// 


using UnityEditor.Localization.Plugins.XLIFF.V12;
using UnityEngine;

[CreateAssetMenu(fileName = "New Quest Decision", menuName = "Scriptable Object/Decision/Quest Decision")]
public class SO_GetQuestDecision : SO_Decision
{
    public SO_Quest quest;

    private void OnValidate() {
        decisionType = DecisionType.GetQuest;
    }
}
