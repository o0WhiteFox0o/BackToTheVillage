using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class InputManager : MonoBehaviour
{
    [SerializeField] private EventSystem eventSystem;
    [SerializeField] private GraphicRaycaster uiRaycaster;
    [SerializeField] private Camera mainCamera;

    public delegate void GetSelectItemInput(int index);
    public delegate void GetObjectClicked(GameObject gameObject);


    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn phím đóng/mở túi đồ.
    /// </summary>
    public static event Action OnOpenBagPress;

    /// <summary>
    /// Bắt sự kiện khi người chơi nhận phím đóng/mở giao diện tổng hợp.
    /// </summary>
    public static event Action OnGeneralUIPress;

    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn phím thay đổi vật phẩm.
    /// </summary>
    public static event GetSelectItemInput OnItemSelected;

    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn skip dialogue trong khi đang hội thoại.
    /// </summary>
    public static event Action OnSkipDialoguePress;

    /// <summary>
    /// Bắt sự kiện khi người chơi click chuột phải vào NPC.
    /// </summary>
    public static event GetObjectClicked OnRightClickNPC;

    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn phím tắt mở giao diện nhiệm vụ.
    /// </summary>
    public static event Action OnQuestUIButtonPress;

    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn chuột trái.
    /// </summary>
    public static event Action<Vector2> OnLeftClick;

    private Dictionary<string, KeyCode> keyBindings = new Dictionary<string, KeyCode>();


    private void Start()
    {
        LoadHotkeys();
    }


    private void Update()
    {
        HandleBagUIPress();
        HandleGeneralUIPress();
        HandleSelectItemInput();
        HandleQuestUIPress();
        CheckPlayerClickHotBar();
        CheckPlayerRightClick();
        HandleSkipDialoguePress();
        HandleOnLeftClick();
    }


    private void LoadHotkeys()
    {
        // nếu dictionary key bindings là null thì tạo dictionary mới
        if (keyBindings == null) { keyBindings = new Dictionary<string, KeyCode>(); }

        // lấy danh sách hot key từ file game config
        var keyConfigs = GameConfig.LoadGameConfig().hotkeys;

        keyBindings.Clear();
        // gán hot key cho từng hành động
        foreach (var key in keyConfigs)
        {
            // kiểm tra nếu key code hợp lệ thì gán cho hành động tương ứng
            if (Enum.TryParse(key.keyCode, out KeyCode keyCode))
            {
                keyBindings.Add(key.action, keyCode);
            }
        }
    }


    private void HandleOnLeftClick()
    {
        if (Input.GetMouseButtonDown(0))
        {
            Vector2 mouseWorldPos = mainCamera.ScreenToWorldPoint(Input.mousePosition);
            OnLeftClick?.Invoke(mouseWorldPos);
        }
    }



    private void HandleBagUIPress()
    {
        if (Input.GetKeyDown(keyBindings["OpenBag"]))
        {
            OnOpenBagPress?.Invoke();
        }
    }


    private void HandleGeneralUIPress()
    {
        if (Input.GetKeyDown(keyBindings["OpenGeneralUI"]))
        {
            OnGeneralUIPress?.Invoke();
        }
    }


    private void HandleQuestUIPress()
    {
        if (Input.GetKeyDown(keyBindings["OpenQuestUI"]))
        {
            OnQuestUIButtonPress?.Invoke();
        }
    }


    public void HandleSkipDialoguePress()
    {
        if (Input.GetKeyDown(keyBindings["SkipDialogue"]))
        {
            OnSkipDialoguePress?.Invoke();
        }
    }


    private void HandleSelectItemInput()
    {
        var input = Input.inputString;
        if (input == "1" || input == "2" || input == "3" || input == "4" || input == "5" || input == "6" || input == "7" || input == "8" || input == "9" || input == "0")
        {
            OnItemSelected?.Invoke(int.Parse(input));
        }
    }


    private void CheckPlayerClickHotBar()
    {
        // người chơi không nhấn chuột thì dừng
        if (!Input.GetMouseButtonDown(0)) { return; }

        // kiểm tra các phần tử UI được người chơi click chuột vào
        PointerEventData pointerData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        uiRaycaster.Raycast(pointerData, results);

        // kiểm tra danh sách các đối tượng được click
        foreach (var result in results)
        {
            var clickedObject = result.gameObject;

            // nếu các item được click nằm trong túi đồ thì bỏ qua
            if (clickedObject.transform.parent.name == "Bag Container") { continue; }

            // kiểm tra tag của các đối tượng, nếu không trùng thì bỏ qua
            if (!clickedObject.CompareTag("InventorySlot")) { continue; }

            // lấy vị trí của item trong hotbar
            var itemIndex = clickedObject.transform.GetSiblingIndex();

            // kiểm tra item index từ 1 
            OnItemSelected?.Invoke(itemIndex + 1);
        }
    }


    private void CheckPlayerRightClick()
    {
        if (!Input.GetMouseButtonDown(1)) { return; }

        Vector2 mouseWorldPos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        RaycastHit2D hit = Physics2D.Raycast(mouseWorldPos, Vector2.zero);

        if (hit.collider == null) { return; }

        if (hit.collider.CompareTag("NPC"))
        {
            OnRightClickNPC?.Invoke(hit.collider.gameObject);
        }
    }


    public void EnableDecisionUI(List<SO_Decision> decisions)
    {
        
    }
}
