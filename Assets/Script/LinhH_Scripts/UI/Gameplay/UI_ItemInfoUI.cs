// 
// Member: LinhH
// Date: 12/11/2025
// 


using TMPro;
using UnityEngine;

public class UI_ItemInfoUI : MonoBehaviour
{
    [SerializeField] public TMP_Text itemName_Text;
    [SerializeField] public TMP_Text itemDescription_Text;


    public void SetUpItemInfo(string itemName, string itemDescription)
    {
        itemName_Text.SetText(itemName);
        itemDescription_Text.SetText(itemDescription);
    }
}
