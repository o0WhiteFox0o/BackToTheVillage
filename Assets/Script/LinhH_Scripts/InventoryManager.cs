using System;
using GameUI;
using UnityEngine;

namespace Management
{
    public class InventoryManager : MonoBehaviour
    {
        // danh sách các slot item, bao gồm cả trong túi đồ
        [SerializeField] private InventorySlot[] _inventorySlots;

        // vị trí của item nhân vật đang mang trong inventory
        private int holdingItemIndex;

        // tiền của nhân vật chính
        public static int gold;

        private GameObject itemPrefab;

        public InventorySlot[] inventorySlots { get => _inventorySlots; set => _inventorySlots = value; }

        /// <summary>
        /// Trả về vật phẩm và nhân vật đang mang.
        /// </summary>
        public ItemScriptableObject holdingItem { get => _inventorySlots[holdingItemIndex].GetComponentInChildren<DragableItem>().itemScriptableObj; }


        void Start()
        {
            itemPrefab = Resources.Load<GameObject>("Prefabs/ItemPrefab");

            if (itemPrefab == null)
            {
                Debug.LogError("Can't load item prefab from resources.");
            }

            // đăng ký các sự kiện cần thiết
            InputManager.OnItemSelected += ChangeSelectedItem;
        }


        void OnDisable()
        {
            InputManager.OnItemSelected -= ChangeSelectedItem;
        }


        private void ChangeSelectedItem(int itemSelected)
        {
            holdingItemIndex = (itemSelected == 0) ? 9 : itemSelected - 1;

            // cập nhật UI hotbar
            SetHoldingItem();
        }


        private void SetHoldingItem()
        {
            // tắt highlight của tất cả các item trong inventory
            foreach (var slot in _inventorySlots)
            {
                slot.DisableHighlight();
            }

            // highlight slot được chọn
            _inventorySlots[holdingItemIndex].HighlightSlot();
        }


        // test chức năng thêm vật phẩm
        public void TestAddItem(ItemScriptableObject item)
        {
            AddItem(item, 1);
        }


        /// <summary>
        /// Thêm một số lượng item vào inventory, trả về true nếu thêm thành công, nếu không thì trả về false.
        /// </summary>
        /// <returns></returns>
        public bool AddItem(ItemScriptableObject item, int quantity)
        {
            // check if any slot has the same item with count lower than max
            // kiểm tra nếu có bất kỳ slot nào trùng item với item được thêm vào inventory và có số lượng chưa đạt tối đa
            foreach (var slot in _inventorySlots)
            {
                // // nếu slot đang được duyệt không chứa item nào thì bỏ qua nó
                // if (slot.transform.childCount == 0) { continue; }

                // kiểm tra xem có item trong slot không, nếu không thì bỏ qua
                var itemInSlot = slot.GetComponentInChildren<DragableItem>();
                if (itemInSlot == null) { continue; }

                // nếu item trong slot khác loại với item được thêm vào thì bỏ qua nó
                if (itemInSlot.itemScriptableObj != item) { continue; }

                // nếu item không thể cộng dồn hoặc số lượng cộng dồn đã đạt tối đa thì bỏ qua nó
                if (!itemInSlot.itemScriptableObj.stackable || itemInSlot.quantity == GameConstants.ITEM_MAX_STACK_COUNT)
                { continue; }

                // cập nhật số lượng item
                itemInSlot.UpdateCount(quantity);

                return true;
            }


            // nếu không có item nào trùng trong inventory hoặc item không thể cộng dồn được nữa thì tìm một slot trống cho nó
            foreach (var slot in _inventorySlots)
            {
                // nếu trong slot đã có item thì bỏ qua nó
                if (slot.transform.childCount != GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT) { continue; }

                SpawnNewItem(item, slot);
                return true;
            }


            return false;
        }


        /// <summary>
        /// Thêm một số lượng item vào slot chỉ định.
        /// </summary>
        public void AddItemToSlot(ItemScriptableObject item, int slotIndex, int quantity)
        {
            SpawnNewItem(item, inventorySlots[slotIndex]);

            var newItem = inventorySlots[slotIndex].GetComponentInChildren<DragableItem>();

            newItem.UpdateCount(quantity);
            newItem.RefreshCount();
        }



        /// <summary>
        /// Tạo một item mới vào một slot trong inventory của nhân vật.
        /// </summary>
        private void SpawnNewItem(ItemScriptableObject item, InventorySlot slot)
        {
            // tạo và thiết lập các thông tin cho item mới
            GameObject newItem = GameObject.Instantiate(itemPrefab, slot.transform);
            newItem.GetComponent<DragableItem>().InitializeItem(item, 1);

            // đưa item mới được thêm vào slot
            // newItem.transform.SetParent(slot.transform);
        }



        // tiêu thụ item
        public void ConsumeItem()
        {

        }


        /// <summary>
        /// Bỏ nhiều item ra khỏi inventory, dùng khi bán item.
        /// </summary>
        private void RemoveItems(ItemScriptableObject removedItem, int removedQuantity)
        {
        }


        /// <summary>
        /// Cập nhật lại số lượng vật phẩm cho toàn bộ inventory
        /// </summary>
        public void RefreshAllItems()
        {
            foreach (var itemSlot in _inventorySlots)
            {
                var item = itemSlot.GetComponentInChildren<DragableItem>();
                if (item != null)
                {
                    item.RefreshCount();
                }
            }
        }
    }
}