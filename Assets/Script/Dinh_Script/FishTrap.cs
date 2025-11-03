using Management;
using Newtonsoft.Json.Bson;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FishTrap : MonoBehaviour
{
    public enum TrapState
    {
        Empty,
        Baited,
        Ready
    }

    [Header("Trap Settings")]
    [SerializeField] private TrapLootTable lootTable;
    [SerializeField] private ItemScriptableObject requiredBait;// Item mồi
    [SerializeField] private int dayToWait = 1; // số ngày cần chờ

    [Header("Sprite Trap")]
    [SerializeField] private Sprite emptyTrap;
    [SerializeField] private Sprite baitedTrap;
    [SerializeField] private Sprite readyTrap;
    
    //Biến trạng thái
    private TrapState currentState = TrapState.Empty;
    private ItemScriptableObject caughtItem = null;
    private int dayBaited = 0;
    private SpriteRenderer spriteRenderer;
    private bool playerInRange = false;

    private void Awake()
    {
        spriteRenderer.GetComponent<SpriteRenderer>();
        UpdateVisual();
    }
    private void OnEnable()
    {
        //Đăng ký sự kiện khi bãy được tạo
        TimeManager.OnNewDay += CheckIfReady;
    }
    private void OnDisable()
    {
        //Hủy đăng kí khi bị phá hủy
        TimeManager.OnNewDay -= CheckIfReady;
    }
    private void CheckIfReady() 
    {
        if (currentState == TrapState.Baited) 
        {
            //Kiểm tra xem đã đủ số ngày chưa
            if (TimeManager.Instance.currentDay >= dayBaited + dayToWait ) 
            {
                currentState = TrapState.Ready;
                caughtItem = lootTable.PickUpLoot();//Quyết định vật phẩm bắt được
                UpdateVisual();
                Debug.Log("Bẫy có thể gỡ");
            }
        }   
    }
    // Hàm này được PlayerInteraction gọi
    // (Trong FishTrap.cs)
    public void Interact(InventoryManager playerInventory)
    {
        switch (currentState)
        {
            case TrapState.Empty:
                // Thử đặt mồi
                if (playerInventory != null && playerInventory.holdingItem == requiredBait)
                {
                    // Thử xóa 1 Mồi khỏi túi
                    if (playerInventory.RemoveItem(requiredBait, 1))
                    {
                        currentState = TrapState.Baited;
                        dayBaited = TimeManager.Instance.currentDay;
                        UpdateVisual();
                        Debug.Log("Đã đặt mồi!");
                    }
                    else
                    {
                        // Code này hiếm khi chạy vì đã check holdingItem, 
                        Debug.Log("Lỗi không thể xóa mồi!");
                    }
                }
                else { /* ... (Báo cần mồi) ... */ }
                break;

            case TrapState.Baited: /* ... */ break;
            case TrapState.Ready: /* ... */ break;
        }
    }
    private void UpdateVisual()
    {
        switch (currentState)
        {
            case TrapState.Empty: spriteRenderer.sprite = emptyTrap; break;
            case TrapState.Baited: spriteRenderer.sprite = baitedTrap; break;
            case TrapState.Ready: spriteRenderer.sprite = readyTrap; break;
        }
    }

    // Nhớ thêm hàm save/load để lưu khi thoát game
}
