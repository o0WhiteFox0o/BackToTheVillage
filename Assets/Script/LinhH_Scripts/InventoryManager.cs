using System;
using GameUI;
using UnityEngine;

public enum ItemType
{
    None,
    Fish,
    FishingRod,
    Bait,
    Trap,
    Hoe,
    WateringCan,
    Axe,
    Seed,
    Consumable,
    ItemQuest,
    Miscellaneous
}
namespace Management
{
    public class InventoryManager : MonoBehaviour
    {
        // danh sách các slot item, bao gồm cả trong túi đồ
        [SerializeField] private InventorySlot[] _inventorySlots;
        [SerializeField] private TileCursorFollow tileCursorFollow;

        public delegate void CollectionItemHandler(ItemScriptableObject updatedItem, int quantity);
        /// <summary>
        /// Được gọi khi người chơi thu thập một vật phẩm. Quest Manager sẽ kiểm tra xem có nhiệm vụ nào cần cập nhật không.
        /// </summary>
        public static event CollectionItemHandler OnCollectItem;


        // vị trí của item nhân vật đang mang trong inventory
        private int holdingItemIndex;

        // tiền của nhân vật chính
        public static int gold;

        private GameObject itemPrefab;


        /// <summary>
        /// Trả về vật phẩm và nhân vật đang mang.
        /// </summary>
        public ItemScriptableObject holdingItem
        {
            get
            {
                if (_inventorySlots == null || _inventorySlots.Length == 0)
                    return null;

                if (holdingItemIndex < 0 || holdingItemIndex >= _inventorySlots.Length)
                    return null;

                var dragItem = _inventorySlots[holdingItemIndex].GetComponentInChildren<DragableItem>();
                if (dragItem == null)
                    return null;

                return dragItem.itemScriptableObj;
            }
        }
        /// <summary>
        /// Lấy DragableItem COMPONENT mà nhân vật đang mang.
        /// Dùng để đọc dữ liệu động (như mồi câu).
        /// </summary>
        public DragableItem GetHoldingItemComponent(){
            if(_inventorySlots == null || _inventorySlots.Length == 0)
                return null;
            if(holdingItemIndex < 0 || holdingItemIndex >= _inventorySlots.Length)
                return null;

            var currentSlot = _inventorySlots[holdingItemIndex];
            var itemInSlot = currentSlot.GetComponentInChildren<DragableItem>();
            return itemInSlot;
        }


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
            HighlightHoldingItem(); // Giả sử hàm này bạn đã có

            var currentSlot = _inventorySlots[holdingItemIndex];
            var itemInSlot = currentSlot.GetComponentInChildren<DragableItem>();

            if (itemInSlot != null && itemInSlot.itemScriptableObj != null)
            {
                // --- THAY ĐỔI BẮT ĐẦU TỪ ĐÂY ---

                // 1. Lấy ItemType thay vì ID
                var itemType = itemInSlot.itemScriptableObj.itemType;
                Debug.Log($"[Hotbar] Đang chọn item: {itemInSlot.itemScriptableObj.displayName}, Loại: {itemType}");

                // 2. Kiểm tra dựa trên Enum
                if (tileCursorFollow != null)
                {
                    // Kiểm tra xem item có phải là Tool, Seed, hoặc Trap không
                    // (Giả định T = Tool/Trap, S = Seed)
                    bool shouldShowCursor =
                        (itemType == ItemType.Hoe) ||
                        (itemType == ItemType.Seed) ||
                        (itemType == ItemType.Trap); // Thêm bất kỳ type nào khác cần con trỏ

                    tileCursorFollow.SetCursorActive(shouldShowCursor);
                }
                // --- KẾT THÚC THAY ĐỔI ---
            }
            else
            {
                Debug.Log("[Hotbar] Ô này đang trống.");
                if (tileCursorFollow != null)
                    tileCursorFollow.SetCursorActive(false);
            }
        }



        private void HighlightHoldingItem()
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

                // kiểm tra có nhiệm vụ nào cần cập nhật không
                OnCollectItem?.Invoke(item, quantity);

                return true;
            }


            // nếu không có item nào trùng trong inventory hoặc item không thể cộng dồn được nữa thì tìm một slot trống cho nó
            foreach (var slot in _inventorySlots)
            {
                // nếu trong slot đã có item thì bỏ qua nó
                if (slot.transform.childCount != GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT) { continue; }

                SpawnNewItem(item, slot);

                // kiểm tra có nhiệm vụ nào cần cập nhật không
                OnCollectItem?.Invoke(item, quantity);

                return true;
            }


            return false;
        }


        /// <summary>
        /// Thêm một số lượng item vào slot chỉ định.
        /// </summary>
        public void AddItemToSlot(ItemScriptableObject item, int slotIndex, int quantity)
        {
            SpawnNewItem(item, _inventorySlots[slotIndex]);

            var newItem = _inventorySlots[slotIndex].GetComponentInChildren<DragableItem>();

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

        /// <summary>
        /// Hàm hỗ trợ: Đếm tổng số lượng của một item trong túi đồ.
        /// </summary>
        public int GetTotalItemQuantity(ItemScriptableObject item)
        {
            int totalAvailable = 0;
            foreach (var slot in _inventorySlots)
            {
                var itemInSlot = slot.GetComponentInChildren<DragableItem>();
                if (itemInSlot != null && itemInSlot.itemScriptableObj == item)
                {
                    totalAvailable += itemInSlot.quantity;
                }
            }
            return totalAvailable;
        }

        /// <summary>
        /// Bỏ một số lượng item ra khỏi inventory.
        /// </summary>
        public bool RemoveItem(ItemScriptableObject itemToRemove, int quantityToRemove)
        {
            if (itemToRemove == null || quantityToRemove <= 0) return false;

            // Bước 1: Kiểm tra xem có đủ hàng không
            int totalAvailable = GetTotalItemQuantity(itemToRemove);
            if (totalAvailable < quantityToRemove)
            {
                Debug.Log($"Không đủ {itemToRemove.id}. Cần {quantityToRemove} nhưng chỉ có {totalAvailable}");
                return false; // Không đủ, không trừ
            }

            // Bước 2: Nếu đủ, bắt đầu trừ
            int quantityLeftToRemove = quantityToRemove;

            // Vòng lặp ngược (từ cuối lên) an toàn hơn khi xóa
            for (int i = _inventorySlots.Length - 1; i >= 0; i--)
            {
                var itemInSlot = _inventorySlots[i].GetComponentInChildren<DragableItem>();

                // Bỏ qua nếu ô trống hoặc không đúng item
                if (itemInSlot == null || itemInSlot.itemScriptableObj != itemToRemove)
                {
                    continue;
                }

                // Nếu stack này nhiều hơn số cần xóa
                if (itemInSlot.quantity > quantityLeftToRemove)
                {
                    // Chỉ cần trừ stack này là đủ
                    itemInSlot.UpdateCount(itemInSlot.quantity - quantityLeftToRemove);
                    quantityLeftToRemove = 0; // Đã xóa đủ
                    break; // Thoát vòng lặp
                }
                else // Nếu stack này ít hơn hoặc bằng số cần xóa
                {
                    // Xóa hết stack này
                    quantityLeftToRemove -= itemInSlot.quantity; // Trừ số lượng đã xóa
                    Destroy(itemInSlot.gameObject); // Xóa object item khỏi slot
                }

                // Nếu đã xóa đủ, dừng lại
                if (quantityLeftToRemove <= 0)
                {
                    break;
                }
            }

            return true; // Xóa thành công
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