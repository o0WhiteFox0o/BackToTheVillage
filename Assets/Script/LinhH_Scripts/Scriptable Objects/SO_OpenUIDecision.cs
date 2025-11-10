// 
// Member: LinhH
// Date: 06/11/2025
// 


using UnityEngine;


[CreateAssetMenu(fileName = "New UI Decision", menuName = "Scriptable Object/Decision/Open UI Decision")]
public class SO_OpenUIDecision : SO_Decision
{
    public string openedUI_id;

    private void OnValidate() {
        decisionType = DecisionType.OpenUI;
    }
}
