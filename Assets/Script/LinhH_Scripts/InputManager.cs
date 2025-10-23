using System;
using UnityEngine;

public class InputManager : MonoBehaviour
{
    public static event Action OnOpenBagPress;


    private void Update()
    {
        HandleOpenBag();
    }

    
    private void HandleOpenBag()
    {
        if (Input.GetKeyDown(KeyCode.E))
        {
            OnOpenBagPress?.Invoke();
        }
    }
}
