using System;
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


    private void Update()
    {
        HandleBagUIPress();
        HandleGeneralUIPress();
        HandleSelectItemInput();
    }


    private void HandleBagUIPress()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnOpenBagPress?.Invoke();
        }
    }


    private void HandleGeneralUIPress()
    {
        if (Input.GetKeyDown(KeyCode.Escape))
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
