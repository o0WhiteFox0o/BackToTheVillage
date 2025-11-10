// 
// Member: LinhH
// Date: 06/11/2025
// 


using System;
using System.Collections.Generic;
using UnityEngine;


[CreateAssetMenu(fileName = "New Gift Decision", menuName = "Scriptable Object/Decision/Receive Gift Decision")]
public class SO_ReceiveGiftDecision : SO_Decision
{
    public List<ReceiveItem> receiveItem_List;

    // TODO: thêm công thức chế tạo và nấu ăn

    private void OnValidate() {
        decisionType = DecisionType.ReceiveGift;
    }
}


[Serializable]
public class ReceiveItem
{
    public ItemScriptableObject item;
    public int quantity;
}