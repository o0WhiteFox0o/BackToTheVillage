// 
// Member: LinhH
// Date: 06/11/2025
// 


using UnityEngine;
using UnityEngine.Localization;

public class SO_Decision : ScriptableObject
{
    public LocalizedString dialogue;
    [HideInInspector] public DecisionType decisionType;
}


public enum DecisionType
{
    Conversation,
    GetQuest,
    OpenUI,
    ReceiveGift
}