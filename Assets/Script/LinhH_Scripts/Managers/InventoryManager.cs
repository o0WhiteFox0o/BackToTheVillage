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
    Miscellaneous,
    CraftingRecipe
}

namespace Management
{
    public class InventoryManager : MonoBehaviour
    {
        public static InventoryManager Instance { get; private set; }

        [Header("Cấu hình Inventory")]
        [SerializeField] private InventorySlot[] _inventorySlots;
        [SerializeField] private TileCursorFollow tileCursorFollow;

        [HideInInspector] public GameObject itemPrefab;

        public static event CollectionItemHandler OnCollectItem;
        public event Action OnInventoryChanged;


        private int holdingItemIndex = 1;
        public static int gold;


        public void Awake()
        {
            if (Instance != null && Instance != this)
            {
                Destroy(gameObject);
            }
            else
            {
                Instance = this;
            }
        }

        void Start()
        {
            itemPrefab = Resources.Load<GameObject>("Prefabs/UI/PFB_ItemPrefab");
            if (itemPrefab == null)
                Debug.LogError("⚠️ Không thể load prefab ItemPrefab từ Resources.");

            ChangeSelectedItem(holdingItemIndex);
            InputManager.OnItemSelected += ChangeSelectedItem;

            DontDestroyOnLoad(this);
        }

        void OnDisable()
        {
            InputManager.OnItemSelected -= ChangeSelectedItem;
        }

        // --- GETTERS ---


        /// <summary>
        /// An toàn, sẽ trả về NULL nếu ô trống.
        /// </summary>
        public DragableItem GetHoldingItemComponent()
        {
            // Kiểm tra xem _inventorySlots có tồn tại và index có hợp lệ không
            if (_inventorySlots == null || holdingItemIndex < 0 || holdingItemIndex >= _inventorySlots.Length)
            {
                return null; // Mảng slot không hợp lệ
            }

            InventorySlot slot = _inventorySlots[holdingItemIndex];
            if (slot == null)
            {
                return null; // Slot không tồn tại
            }

            return slot.GetComponentInChildren<DragableItem>();
        }

        /// <summary>
        /// An toàn, sẽ trả về NULL nếu đang cầm ô trống.
        /// </summary>
        public ItemScriptableObject holdingItem
        {
            get
            {
                DragableItem itemComponent = GetHoldingItemComponent();

                // Kiểm tra an toàn: Nếu ô trống (component null), trả về null
                if (itemComponent == null)
                {
                    return null;
                }

                // Nếu ô không trống, trả về item data
                return itemComponent.itemScriptableObj;
            }
        }


        // --- LOGIC ĐỔI SLOT ---

        private void ChangeSelectedItem(int itemSelected)
        {
            holdingItemIndex = (itemSelected == 0) ? 9 : itemSelected - 1;
            HighlightHoldingItem();

            var currentSlot = _inventorySlots[holdingItemIndex];
            var itemInSlot = currentSlot.GetComponentInChildren<DragableItem>();

            if (itemInSlot != null && itemInSlot.itemScriptableObj != null)
            {
                var itemType = itemInSlot.itemScriptableObj.itemType;
                Debug.Log($"[Hotbar] Đang chọn item: {itemInSlot.itemScriptableObj.displayName}, Loại: {itemType}");

                if (tileCursorFollow != null)
                {
                    bool shouldShowCursor =
                        (itemType == ItemType.Hoe) ||
                        (itemType == ItemType.Seed) ||
                        (itemType == ItemType.WateringCan) ||
                        (itemType == ItemType.Trap);

                    tileCursorFollow.SetCursorActive(shouldShowCursor);
                }
            }
            else
            {
                Debug.Log("[Hotbar] Ô này đang trống.");
                tileCursorFollow?.SetCursorActive(false);
            }
        }

        private void HighlightHoldingItem()
        {
            foreach (var slot in _inventorySlots)
                slot.DisableHighlight();

            _inventorySlots[holdingItemIndex].HighlightSlot();
        }

        // --- CHỨC NĂNG INVENTORY ---

        public void TestAddItem(ItemScriptableObject item)
        {
            AddItem(item, 1);
            OnInventoryChanged?.Invoke();
        }

        /// <summary>
        /// Thêm item vào inventory (tự động cộng dồn nếu có sẵn)
        /// </summary>
        public bool AddItem(ItemScriptableObject item, int quantity)
        {
            if (item == null || quantity <= 0) return false;

            // 1️⃣ Cộng dồn vào slot có cùng item nếu còn chỗ
            foreach (var slot in _inventorySlots)
            {
                var itemInSlot = slot.GetComponentInChildren<DragableItem>();
                if (itemInSlot == null) continue;
                if (itemInSlot.itemScriptableObj != item) continue;
                if (!itemInSlot.itemScriptableObj.stackable ||
                    itemInSlot.quantity >= GameConstants.ITEM_MAX_STACK_COUNT)
                    continue;

                int freeSpace = GameConstants.ITEM_MAX_STACK_COUNT - itemInSlot.quantity;
                int toAdd = Mathf.Min(freeSpace, quantity);

                itemInSlot.AddCount(toAdd);
                OnCollectItem?.Invoke(item, toAdd);
                OnInventoryChanged?.Invoke();
                return true;

            }

            // 2️⃣ Nếu không có slot trùng, spawn mới
            foreach (var slot in _inventorySlots)
            {
                if (slot.transform.childCount != GameConstants.DEFAULT_INVENTORY_SLOT_CHILDREN_COUNT)
                    continue;

                SpawnNewItem(item, slot, quantity);
                OnCollectItem?.Invoke(item, quantity);
                return true;
            }

            Debug.Log("❌ Inventory đầy, không thể thêm item.");
            return false;
        }

        /// <summary>
        /// Tạo item mới trong slot chỉ định với số lượng cụ thể
        /// </summary>
        private void SpawnNewItem(ItemScriptableObject item, InventorySlot slot, int quantity)
        {
            GameObject newItem = Instantiate(itemPrefab, slot.transform);
            var dragable = newItem.GetComponent<DragableItem>();

            // Khởi tạo với số lượng ban đầu
            dragable.InitializeItem(item, quantity);
        }

        /// <summary>
        /// Lấy tổng số lượng item trong inventory
        /// </summary>
        public int GetTotalItemQuantity(ItemScriptableObject item)
        {
            int totalAvailable = 0;
            foreach (var slot in _inventorySlots)
            {
                var itemInSlot = slot.GetComponentInChildren<DragableItem>();
                if (itemInSlot != null && itemInSlot.itemScriptableObj == item)
                    totalAvailable += itemInSlot.quantity;
            }
            return totalAvailable;
        }

        /// <summary>
        /// Bỏ một số lượng item ra khỏi inventory
        /// </summary>
        public bool RemoveItem(ItemScriptableObject itemToRemove, int quantityToRemove)
        {
            if (itemToRemove == null || quantityToRemove <= 0) return false;

            // Kiểm tra tổng trước xem có đủ không
            int totalAvailable = GetTotalItemQuantity(itemToRemove);
            if (totalAvailable < quantityToRemove)
            {
                Debug.Log($"❌ Không đủ {itemToRemove.id}. Cần {quantityToRemove} nhưng chỉ có {totalAvailable}");
                return false;
            }

            int quantityLeftToRemove = quantityToRemove;

            // Duyệt ngược từ cuối lên đầu
            for (int i = _inventorySlots.Length - 1; i >= 0; i--)
            {
                var itemInSlot = _inventorySlots[i].GetComponentInChildren<DragableItem>();

                // Nếu slot trống hoặc không phải item cần tìm -> Bỏ qua
                if (itemInSlot == null || itemInSlot.itemScriptableObj != itemToRemove)
                    continue;

                if (itemInSlot.quantity > quantityLeftToRemove)
                {
                    itemInSlot.SubtractCount(quantityLeftToRemove);
                    OnInventoryChanged?.Invoke();

                    return true; // Đã trừ xong, thoát hàm
                }
                else
                {
                    quantityLeftToRemove -= itemInSlot.quantity;
                    Destroy(itemInSlot.gameObject);
                }

                // Nếu sau khi trừ slot trên mà đã đủ chỉ tiêu
                if (quantityLeftToRemove <= 0)
                {
                    OnInventoryChanged?.Invoke();
                    return true;
                }
            }

            OnInventoryChanged?.Invoke();
            return true;
        }

        /// <summary>
        /// Làm mới số lượng hiển thị cho toàn bộ item
        /// </summary>
        public void RefreshAllItems()
        {
            foreach (var itemSlot in _inventorySlots)
            {
                var item = itemSlot.GetComponentInChildren<DragableItem>();
                if (item != null)
                    item.RefreshCount();
            }
        }
    }
}

public delegate void CollectionItemHandler(ItemScriptableObject updatedItem, int quantity);