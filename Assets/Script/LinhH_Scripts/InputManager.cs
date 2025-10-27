using System;
using System.Collections.Generic;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn phím đóng/mở túi đồ.
    /// </summary>
    public static event Action OnOpenBagPress;

    /// <summary>
    /// Bắt sự kiện khi người chơi nhận phím đóng/mở giao diện tổng hợp
    /// </summary>
    public static event Action OnGeneralUIPress;

    public delegate void GetSelectItemInput(int index);
    /// <summary>
    /// Bắt sự kiện khi người chơi nhấn phím thay đổi vật phẩm.
    /// </summary>
    public static event GetSelectItemInput OnItemSelected;

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


    private void HandleSelectItemInput()
    {
        var input = Input.inputString;
        if (input == "1" || input == "2" || input == "3" || input == "4" || input == "5" || input == "6" || input == "7" || input == "8" || input == "9" || input == "0")
        {
            OnItemSelected?.Invoke(int.Parse(input));
        }
    }
}
