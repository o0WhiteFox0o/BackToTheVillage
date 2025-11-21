// 
// Member   : Linh
// Date     : 
// 


using System;
using System.Collections.Generic;
using System.Linq;
using GameUI;
using TMPro;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.Localization;
using UnityEngine.UI;


/// <summary>
/// Quản lý các thành phần giao diện trong gameplay
/// </summary>
public class GameplayUIManager : MonoBehaviour
{
    [Header("Menu UIs")]
    [SerializeField] public GameObject bagUI;
    [SerializeField] public GameObject generalUI;
    [SerializeField] public GameObject npcUI;
    [SerializeField] public GameObject settingUI;
    [SerializeField] public GameObject characterUI;
    [SerializeField] public GameObject generalUI_Notification;


    [Header("Conversation")]
    [SerializeField] public GameObject conversation_UI;
    [SerializeField] public TMP_Text npcName_Text;
    [SerializeField] public Image npcPortrait_Image;
    [SerializeField] public TMP_Text conversationDisplay_Text;
    [SerializeField] public Transform decisionPanel;
    private GameObject decisionButton_Prefab;

    public MGR_QuestUIManager questUIManager { get; private set; }


    [Header("Other")]
    [SerializeField] public GraphicRaycaster uiRaycaster;

    private GameObject itemInfoUI;
    private GameObject itemInfoUI_Prefab;

    private EventSystem eventSystem;
    private bool isAnyUIOpen;


    private void Start()
    {
        isAnyUIOpen = false;

        eventSystem = FindObjectOfType<EventSystem>();
        itemInfoUI_Prefab = Resources.Load<GameObject>("Prefabs/UI/PFB_ItemInfoUI");
        decisionButton_Prefab = Resources.Load<GameObject>("Prefabs/UI/PFB_DecisionButton");

        questUIManager = GetComponentInChildren<MGR_QuestUIManager>();

        if (eventSystem == null || itemInfoUI_Prefab == null || decisionButton_Prefab == null || questUIManager == null)
        {
            Debug.LogError("Can't load a manager component.");
        }

        // đăng ký sự kiện cần thiết
        InputManager.OnOpenBagPress += ToggleBagUI;
        InputManager.OnGeneralUIPress += ToggleGeneralUI;
        InputManager.OnQuestUIButtonPress += ToggleQuestUI;

        DontDestroyOnLoad(this);
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= ToggleBagUI;
        InputManager.OnGeneralUIPress -= ToggleGeneralUI;
        InputManager.OnQuestUIButtonPress -= ToggleQuestUI;

        // MGR_QuestManager.OnQuestListUpdate -= RefreshQuestUIList;
        // CollectionQuestProgress.OnCollectionQuestUpdate -= RefreshCollectionProgressUI;
    }


    #region General
    public void ToggleBagUI()
    {
        // tắt UI túi đồ nếu nó đang bật
        if (bagUI.activeSelf)
        {
            bagUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI túi đồ nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            bagUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }


    public void ToggleGeneralUI()
    {
        // tắt UI general nếu nó đang bật
        if (generalUI.activeInHierarchy)
        {
            generalUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI general nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            generalUI.SetActive(true);
            // isAnyUIOpen = true;
        }
    }


    public void ToggleNPC_UI()
    {
        // tắt UI npc nếu nó đang bật
        if (npcUI.activeInHierarchy)
        {
            npcUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI npc nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            npcUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }


    public void ToggleQuestUI()
    {
        // tắt UI quest nếu nó đang bật
        if (questUIManager.backgroundImage.activeSelf)
        {
            questUIManager.EnableQuestUI(false);
            isAnyUIOpen = false;
        }
        // bật UI quest nếu nó đang tắt và không có UI nào khác đang được bật
        else 
        // if (!isAnyUIOpen)
        {
            questUIManager.EnableQuestUI(true);
            isAnyUIOpen = true;

            questUIManager.FillQuestCategorize(0);

            // tắt thông báo quest khi người chơi mở giao diện nhiệm vụ
            // DisableQuestNotification();
        }
    }


    public void ToggleSettingUI()
    {
        // tắt UI setting nếu nó đang bật
        if (settingUI.activeInHierarchy)
        {
            settingUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI setting nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            settingUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }


    public void ToggleCharacterUI()
    {
        // tắt UI thông tin nhân vật nếu nó đang bật
        if (characterUI.activeSelf)
        {
            characterUI.SetActive(false);
            isAnyUIOpen = false;
        }
        // bật UI thông tin nhân vật nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            characterUI.SetActive(true);
            isAnyUIOpen = true;
        }
    }
    #endregion


    #region Conversation
    public void SetActiveConversationPanel(bool value)
    {
        conversation_UI.SetActive(value);
    }


    /// <summary>
    /// Cập nhật tên và avatar của NPC đang nói chuyện.
    /// </summary>
    public void UpdateDisplayedNPC(LocalizedString npcName, Sprite npcPortrait)
    {
        npcName_Text.SetText(npcName.GetLocalizedString());
        npcPortrait_Image.sprite = npcPortrait;
    }


    public void UpdateConversationText(string npcDialogue)
    {
        conversationDisplay_Text.SetText(npcDialogue);
    }


    public void AddLetterToDialogueText(char letter)
    {
        conversationDisplay_Text.text += letter;
    }


    public void DisplayConversationDecisions(List<SO_Decision> decision_List)
    {
        decisionPanel.gameObject.SetActive(true);

        foreach (var decision in decision_List)
        {
            var decisionPrefab = MGR_ObjectPoolManager.SpawnObject(decisionButton_Prefab, decisionPanel);

            // lấy các thành phần trong game object decision
            var decisionController = decisionPrefab.GetComponent<C_DecisionController>();
            var decisionBtn = decisionPrefab.GetComponent<Button>();

            // thiết lập các thành phần của game object decision
            decisionController.SetupDecisionUI(decision);
            decisionBtn.onClick.AddListener(decisionController.ImplementDecision);
            decisionBtn.onClick.AddListener(HideDecisionPanel);
        }

        // tắt tính năng skip dialogue
        ToggleSkipDialogueButton(false);
    }


    public void HideDecisionPanel()
    {
        foreach (Transform decision in decisionPanel)
        {
            decision.GetComponent<Button>().onClick.RemoveAllListeners();
            MGR_ObjectPoolManager.ReturnObjectToPool(decision.gameObject);
        }

        decisionPanel.gameObject.SetActive(false);
    }


    public void ToggleSkipDialogueButton(bool state)
    {
        var skipDialogueButton = conversationDisplay_Text.GetComponentInParent<Button>();
        skipDialogueButton.enabled = state;
    }
    #endregion

    // #region Notification
    // public void EnableQuestNotification()
    // {
    //     questUI_Notification.SetActive(true);
    //     generalUI_Notification.SetActive(true);
    // }


    // public void DisableQuestNotification()
    // {
    //     questUI_Notification.SetActive(false);

    //     UpdateGeneralUINotification();
    // }


    // /// <summary>
    // /// Cập nhật thông báo của general UI dựa theo thông báo của các UI con nằm trong nó.
    // /// </summary>
    // private void UpdateGeneralUINotification()
    // {
    //     // kiểm tra thông báo của tất cả các UI của general UI
    //     // nếu có bất kỳ thông báo nào được bật thì bật thông báo general
    //     if (questUI_Notification.activeSelf)
    //     {
    //         generalUI_Notification.SetActive(true);
    //         return;
    //     }

    //     // nếu không có thông báo nào được bật thì tắt thông báo general
    //     generalUI_Notification.SetActive(false);
    // }

    // #endregion


    #region Inventory
    public void EnableItemInfoUI(Transform inventorySlot)
    {
        // 1. Tạo UI
        itemInfoUI = MGR_ObjectPoolManager.SpawnObject(itemInfoUI_Prefab, transform);

        // 2. Lấy vật phẩm từ Slot
        var itemInSlot = inventorySlot.GetComponentInChildren<DragableItem>();
        // Nếu không có vật phẩm thì dừng lại, không làm gì cả
        if (itemInSlot == null) return;

        // 3. Tính giá thực tế (Đã cộng Buff nghề nghiệp)
        // Gọi Manager để lấy giá của 1 món đồ
        int finalUnitPrice = PlayerStatManager.Instance.GetActualItemPrice(itemInSlot.itemScriptableObj);

        // 4. Lấy thông tin hiển thị
        var itemName = itemInSlot.itemScriptableObj.displayName.GetLocalizedString();
        var itemDesc = itemInSlot.itemScriptableObj.itemDescription.GetLocalizedString();

        // 5. Cập nhật biến giá (Dùng giá mới tính được)
        var itemPrice = finalUnitPrice; // Giá đơn lẻ
        var itemPriceStack = finalUnitPrice * itemInSlot.quantity; // Giá tổng (Stack)

        // 6. Gửi vào UI để hiển thị
        itemInfoUI.GetComponent<UI_ItemInfoUI>().SetUpItemInfo(itemName, itemDesc, itemPrice, itemPriceStack);

        SetUpItemInfoPosition(inventorySlot);
    }


    public void DisableItemInfoUI()
    {
        // tắt ui thông tin item
        MGR_ObjectPoolManager.ReturnObjectToPool(itemInfoUI);
    }


    /// <summary>
    /// Thiết lập vị trí của item info UI dựa theo vị trí của slot trên màn hình.
    /// </summary>
    private void SetUpItemInfoPosition(Transform inventorySlot)
    {
        // lấy vị trí x và y của con trỏ chuột
        var mousePosX = Input.mousePosition.x;
        var mousePosY = Input.mousePosition.y;

        var itemInfoUIRectTransform = itemInfoUI.GetComponent<RectTransform>();

        // lấy vị trí 4 góc của inventory slot
        Vector3[] worldCorners = new Vector3[4];
        inventorySlot.gameObject.GetComponent<RectTransform>().GetWorldCorners(worldCorners);


        // thiết lập vị trí của item info ui khi con trỏ chuột ở phần trên - trái
        if ((mousePosY > Screen.height / 2) && (mousePosX < Screen.width / 2))
        {
            // thiết lập pivot của item info ui
            itemInfoUIRectTransform.pivot = new Vector2(0, 1f);

            // gán vị trí của item info ui tại góc dưới bên phải của slot
            itemInfoUI.transform.position = worldCorners[3];
        }

        // thiết lập vị trí của item info ui khi con trỏ chuột ở phần trên - phải
        else if ((mousePosY > Screen.height / 2) && (mousePosX > Screen.width / 2))
        {
            // thiết lập pivot của item info ui
            itemInfoUIRectTransform.pivot = Vector2.one;

            // gán vị trí của item info ui tại góc dưới bên phải của slot
            itemInfoUI.transform.position = worldCorners[0];
        }

        // thiết lập vị trí của item info ui khi con trỏ chuột ở phần dưới - trái
        else if ((mousePosY < Screen.height / 2) && (mousePosX < Screen.width / 2))
        {
            // thiết lập pivot của item info ui
            itemInfoUIRectTransform.pivot = Vector2.zero;

            // gán vị trí của item info ui tại góc dưới bên phải của slot
            itemInfoUI.transform.position = worldCorners[2];
        }

        // thiết lập vị trí của item info ui khi con trỏ chuột ở phần dưới - phải
        else
        {
            // thiết lập pivot của item info ui
            itemInfoUIRectTransform.pivot = new Vector2(1f, 0);

            // gán vị trí của item info ui tại góc dưới bên phải của slot
            itemInfoUI.transform.position = worldCorners[1];
        }

        #endregion
    }
}