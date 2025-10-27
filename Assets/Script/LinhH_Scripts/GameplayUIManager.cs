using System;
using System.Collections.Generic;
using Management;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public class GameplayUIManager : MonoBehaviour
{
    [Header("UIs")]
    [SerializeField] public GameObject bagUI;
    [SerializeField] public GameObject generalUI;
    [SerializeField] public GameObject npcUI;
    [SerializeField] public GameObject settingUI;

    [Header("Other")]
    [SerializeField] public GraphicRaycaster uiRaycaster;
    [SerializeField] public EventSystem eventSystem;

    /// <summary>
    /// Event kiểm tra người chơi có click chuột vào item trong hot bar không.
    /// </summary>
    public static event InputManager.GetSelectItemInput OnPlayerClickHotBarItem;
    private bool isAnyUIOpen;


    private void Start()
    {
        isAnyUIOpen = false;

        // đăng ký sự kiện cần thiết
        InputManager.OnOpenBagPress += ToggleBagUI;
        InputManager.OnGeneralUIPress += ToggleGeneralUI;
    }


    private void OnDisable()
    {
        InputManager.OnOpenBagPress -= ToggleBagUI;
        InputManager.OnGeneralUIPress -= ToggleGeneralUI;
    }


    private void Update()
    {
        CheckPlayerClickHotBar();
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
            // inventory manager check item index từ 1 
            OnPlayerClickHotBarItem?.Invoke(itemIndex + 1);
        }
    }


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
            isAnyUIOpen = true;
        }
    }


    /// <summary>
    /// Hiển thị UI con trong general UI.
    /// </summary>
    public void EnableGeneralSubUI(Transform subUI)
    {
        subUI.SetAsLastSibling();
    }
}
