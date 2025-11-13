// 
// Member: LinhH
// Date: 12/11/2025
// 


using TMPro;
using UnityEngine;
using UnityEngine.Localization;

public class UI_ItemInfoUI : MonoBehaviour
{
    [SerializeField] public TMP_Text itemName_Text;
    [SerializeField] public TMP_Text itemDescription_Text;
    [SerializeField] public TMP_Text itemPrice_Text;
    [SerializeField] public TMP_Text itemPriceStack_Text;


    public void SetUpItemInfo(string itemName, string itemDescription, int price, int priceStack)
    {
        itemName_Text.SetText(itemName);
        itemDescription_Text.SetText(itemDescription);

        if (price > 0)
        {
            itemPrice_Text.SetText($"Price: {price.ToString()} vnđ");
            itemPriceStack_Text.SetText($"Price Stack: {priceStack.ToString()} vnđ");
        }
        else
        {
            // load thông báo từ localization table (từ bảng GameplayMessage, key gMgs.itemCantSell)
            LocalizedString localizedString = new LocalizedString("GameplayMessage", "gMgs.itemCantSell");
            itemPrice_Text.SetText(localizedString.GetLocalizedString());
            itemPriceStack_Text.SetText(localizedString.GetLocalizedString());
        }
    }
}
