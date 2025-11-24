// 
// Member   : Linh
// Date     : 
// 


using GameUI;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;


/// <summary>
/// Quản lý các thành phần giao diện trong gameplay
/// </summary>
public class UI_GameplayUIManager : MonoBehaviour
{
    [Header("Menu UIs")]
    [SerializeField] public GameObject bagUI;
    [SerializeField] public GameObject generalUI;
    [SerializeField] public GameObject npcUI;
    [SerializeField] public GameObject characterUI;
    [SerializeField] public GameObject generalUI_Notification;


    // Load from Resources
    private GameObject itemInfoUI_Prefab;


    // Load from Hierarchy
    private EventSystem eventSystem;
    private GraphicRaycaster uiRaycaster;


    // Load from children game objects
    public MGR_QuestUIManager questUIManager { get; private set; }
    public MGR_ConversationUIManager conversationUIManager { get; private set; }
    public UI_SettingUIManager settingUIManager { get; private set; }


    // Temporary variables
    private GameObject itemInfoUI;
    private bool isAnyUIOpen;


    private void Start()
    {
        // thiết lập các biến cần thiết
        isAnyUIOpen = false;

        // load các thành phần từ Resources
        itemInfoUI_Prefab = Resources.Load<GameObject>("Prefabs/UI/PFB_ItemInfoUI");

        // load các thành phần cần thiết
        questUIManager = GetComponentInChildren<MGR_QuestUIManager>();
        conversationUIManager = GetComponentInChildren<MGR_ConversationUIManager>();
        settingUIManager = GetComponentInChildren<UI_SettingUIManager>();

        eventSystem = FindObjectOfType<EventSystem>();
        uiRaycaster = GetComponent<GraphicRaycaster>();

        if (eventSystem == null || itemInfoUI_Prefab == null || questUIManager == null || conversationUIManager == null
            || uiRaycaster == null || settingUIManager == null)
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

            generalUI.transform.SetAsLastSibling();
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

            generalUI.transform.SetAsLastSibling();
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

            generalUI.transform.SetAsLastSibling();
        }
        // bật UI quest nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            questUIManager.EnableQuestUI(true);
            isAnyUIOpen = true;

            // mặc định mở mục story quest khi mở giao diện
            questUIManager.FillQuestCategorize(0);

            // thiết lập UI quest hiển thị lên trên các UI khác
            questUIManager.transform.SetAsLastSibling();

            // tắt thông báo quest khi người chơi mở giao diện nhiệm vụ
            // DisableQuestNotification();
        }
    }


    public void ToggleSettingUI()
    {
        // tắt UI setting nếu nó đang bật
        if (settingUIManager.settingPanel.activeSelf)
        {
            settingUIManager.EnableSettingUI(false);
            isAnyUIOpen = false;

            generalUI.transform.SetAsLastSibling();
        }
        // bật UI setting nếu nó đang tắt và không có UI nào khác đang được bật
        else if (!isAnyUIOpen)
        {
            settingUIManager.EnableSettingUI(true);
            isAnyUIOpen = true;

            // thiết lập UI setting hiển thị lên trên các UI khác
            settingUIManager.transform.SetAsLastSibling();
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
    }
}