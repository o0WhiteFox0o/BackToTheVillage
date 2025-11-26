// 
// Member   : Linh
// Date     : 
// 


using System.Collections;
using System.Collections.Generic;
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
    [SerializeField] public GameObject inventoryUI;
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
    public UI_QuestUIManager questUIManager { get; private set; }
    public UI_ConversationUIManager conversationUIManager { get; private set; }
    public UI_SettingUIManager settingUIManager { get; private set; }


    // Temporary variables
    private GameObject itemInfoUI;
    private Stack<GameObject> openedUIs = new Stack<GameObject>();
    private bool generalUIOpen = false;


    private void Start()
    {
        // thiết lập các biến cần thiết
        openedUIs.Clear();

        // load các thành phần từ Resources
        itemInfoUI_Prefab = Resources.Load<GameObject>("Prefabs/UI/PFB_ItemInfoUI");

        // load các thành phần cần thiết
        questUIManager = GetComponentInChildren<UI_QuestUIManager>();
        conversationUIManager = GetComponentInChildren<UI_ConversationUIManager>();
        settingUIManager = GetComponentInChildren<UI_SettingUIManager>();

        eventSystem = FindObjectOfType<EventSystem>();
        uiRaycaster = GetComponent<GraphicRaycaster>();

        if (eventSystem == null || itemInfoUI_Prefab == null || questUIManager == null || conversationUIManager == null
            || uiRaycaster == null || settingUIManager == null)
        {
            Debug.LogError("Can't load a manager component.");
        }

        // đăng ký sự kiện cần thiết
        InputManager.OnOpenBagPress += EnableBagUI;
        InputManager.OnEscPress += EnableGeneralUI;
        InputManager.OnEscPress += DisableUI;
        InputManager.OnQuestUIButtonPress += EnableQuestUI;

        RefreshUILayer();

        DontDestroyOnLoad(this);
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= EnableBagUI;
        InputManager.OnEscPress -= EnableGeneralUI;
        InputManager.OnEscPress -= DisableUI;
        InputManager.OnQuestUIButtonPress -= EnableQuestUI;
    }


    /// <summary>
    /// Thiết lập layer cho các UI trong gameplay, để có thể tương tác được với các UI đang bật.
    /// </summary>
    private void RefreshUILayer()
    {
        if (openedUIs.Count != 0)
        {
            openedUIs.Pop();
        }

        if (openedUIs.Count == 0)
        {
            inventoryUI.transform.SetAsLastSibling();
            return;
        }

        openedUIs.Peek().transform.SetAsLastSibling();
    }


    public void DisableUI()
    {
        if (openedUIs.Count == 0) { return; }

        // nếu general UI vừa được mở thì không đóng nó ngay lập tức
        if (generalUIOpen)
        {
            generalUIOpen = false;
            return;
        }

        openedUIs.Peek().SetActive(false);
        RefreshUILayer();
    }


    public void EnableGeneralUI()
    {
        // không thể mở general UI khi có một UI bất kỳ được mở
        if (openedUIs.Count != 0) { return; }

        generalUI.SetActive(true);
        generalUI.transform.SetAsLastSibling();

        openedUIs.Push(generalUI);

        generalUIOpen = true;
    }


    public void EnableBagUI()
    {
        // không thể mở setting ui khi nó đang mở
        if (bagUI.activeSelf) { return; }

        bagUI.SetActive(true);
        inventoryUI.transform.SetAsLastSibling();

        openedUIs.Push(bagUI);
    }



    public void EnableNPC_UI()
    {
        // không thể mở setting ui khi nó đang mở
        if (npcUI.activeSelf) { return; }

        npcUI.SetActive(true);
        npcUI.transform.SetAsLastSibling();

        openedUIs.Push(npcUI);
    }


    public void EnableQuestUI()
    {
        // không thể mở quest ui khi nó đang mở
        if (questUIManager.backgroundImage.activeSelf) { return; }

        questUIManager.EnableQuestUI();
        questUIManager.gameObject.transform.SetAsLastSibling();

        openedUIs.Push(questUIManager.backgroundImage);
    }


    public void EnableSettingUI()
    {
        // không thể mở setting ui khi nó đang mở
        if (settingUIManager.settingPanel.activeSelf) { return; }

        settingUIManager.EnableSettingUI();
        settingUIManager.gameObject.transform.SetAsLastSibling();

        openedUIs.Push(settingUIManager.settingPanel);
    }


    public void EnableCharacterUI()
    {
        // không thể mở character ui khi nó đang mở
        if (characterUI.activeSelf) { return; }

        characterUI.SetActive(true);
        characterUI.transform.SetAsLastSibling();

        openedUIs.Push(characterUI);
    }


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


    /// <summary>
    /// Bật UI hiển thị thông tin vật phẩm.
    /// </summary>
    public void EnableItemInfoUI(Transform inventorySlot)
    {
        // 1. Tạo UI
        itemInfoUI = MGR_ObjectPoolManager.SpawnObject(itemInfoUI_Prefab, transform);

        // 2. Lấy vật phẩm từ Slot
        var itemInSlot = inventorySlot.GetComponentInChildren<DragableItem>();
        // Nếu không có vật phẩm thì dừng lại, không làm gì cả
        if (itemInSlot == null) return;

        // 4. Lấy thông tin hiển thị
        var itemName = itemInSlot.itemScriptableObj.displayName.GetLocalizedString();
        var itemDesc = itemInSlot.itemScriptableObj.itemDescription.GetLocalizedString();

        // 6. Gửi vào UI để hiển thị
        itemInfoUI.GetComponent<UI_ItemInfoUI>().SetUpItemInfo(itemName, itemDesc);
        // thiết lập layer cho item info (cho nó lên đầu tiên)
        itemInfoUI.transform.SetAsLastSibling();

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