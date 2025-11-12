using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using TMPro;
using Management;


namespace GameUI
{
    [RequireComponent(typeof(Image))]
    public class DragableItem : MonoBehaviour, IBeginDragHandler, IDragHandler, IEndDragHandler
    {
        public static DragableItem itemBeingHeld;

        // private properties
        private ItemScriptableObject _itemScriptableObj;
        [HideInInspector] public Transform parentAfterDrag;
        [HideInInspector] public Transform parentBeforeDrag;
        private int _quantity;

        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _countText;

        //Biến này chỉ có tác dụng khi item là cần câu
        [HideInInspector] public BaitSO attachedBait;
        [HideInInspector] public int baitQuantity = 0;

        [Header("Fishing Rod Only - Bait UI")]
        [SerializeField] private Image _baitIconImage;
        [SerializeField] private TMP_Text _baitCountText;

        public ItemScriptableObject itemScriptableObj { get => _itemScriptableObj; set => _itemScriptableObj = value; }
        public int quantity { get => _quantity; set => _quantity = value; }

        public void Update()
        {
            if (itemBeingHeld == this) { 
                transform.position = Input.mousePosition;
            }
        }

        /// <summary>
        /// Thiết lập các thông tin của item.
        /// </summary>
        public void InitializeItem(ItemScriptableObject newItem, int quantity)
        {
            _itemScriptableObj = newItem;
            _quantity = quantity;
            _image.sprite = _itemScriptableObj.icon;

            RefreshCount();
            UpdateBaitVisuals();
        }


        /// <summary>
        /// Cập nhật lại số lượng vật phẩm được hiển thị
        /// </summary>
        public void RefreshCount()
        {
            // if item's count is 1, hide the count text
            _countText.gameObject.SetActive(!(quantity == 1));

            _countText.SetText(_quantity.ToString());
        }


        public void OnBeginDrag(PointerEventData eventData)
        {
            if (GameplayUIManager.isAnyUIOpen == false) { 
                eventData.pointerDrag = null;
                if(_image != null)_image.raycastTarget = true;
                return;
            }
            if (_image == null || eventData.button != PointerEventData.InputButton.Left) return;

            itemBeingHeld = this;// Báo cho hệ thống biết "Tôi đang được cầm"

            _image.raycastTarget = false;
            parentBeforeDrag = transform.parent;   // backup slot cũ
            parentAfterDrag = parentBeforeDrag;
            parentAfterDrag = transform.parent;    // lưu lại slot cha
            transform.SetParent(transform.root);   // đưa ra root để dễ kéo
        }


        public void OnDrag(PointerEventData eventData)
        {
            //if (_image == null) return;
            //transform.position = Input.mousePosition;
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            itemBeingHeld = null;// Báo cho hệ thống "Tôi đã được thả"

            if (_image == null) return;
            _image.raycastTarget = true;

            if (parentAfterDrag != null)
            {
                transform.SetParent(parentAfterDrag);
                transform.localPosition = Vector3.zero;  // căn vào slot
            }
            else
            {
                Debug.LogWarning("parentAfterDrag bị null, trả về vị trí cũ");
                transform.SetParent(parentBeforeDrag);
                transform.localPosition = Vector3.zero;
            }
        }


        /// <summary>
        /// Cập nhật số lượng item (thêm vào hoặc bớt đi một số lượng). 
        /// </summary>
        /// <param name="count">Số lượng thêm vào hoặc bớt đi của item.</param>
        public void AddCount(int amount)
        {
            _quantity += amount;
            RefreshCount();
        }

        public void SubtractCount(int amount)
        {
            _quantity -= amount;
            if (_quantity < 0) _quantity = 0;
            RefreshCount();
        }


        /// <summary>
        /// Thử gắn một stack mồi vào vật phẩm này (nếu đây là cần câu).
        /// </summary>
        /// <returns>True nếu gắn thành công.</returns>
        public bool TryAttachBait(BaitSO baitData, int amount)
        {
            // Kiểm tra xem item này có phải cần câu không
            if (!(_itemScriptableObj is FishingRodSO rodData))
            {
                return false;
            }
            // Kiểm tra xem cần câu có thể sử dụng mồi không
            if (!rodData.canUseBait) 
            {
                Debug.Log("Cần câu này không thể sử dụng mồi!");
                return false;
            }
            // Kiểm tra xem cần câu đã gắn mồi chưa
            if (attachedBait != null && attachedBait != baitData) 
            { 
                Debug.Log("Cần câu này đã gắn mồi khác!");
                return false;
            }
            // Gắn mồi vào cần câu
            attachedBait = baitData;
            baitQuantity += amount;
            Debug.Log($"Đã gắn {amount} mồi {baitData.displayName} vào cần câu {rodData.displayName}. Tổng mồi: {baitQuantity}.");

            UpdateBaitVisuals();
            // Thêm UI hiển thị mồi nếu cần thiết
            return true;
        }
        /// <summary>
        /// Tiêu thụ 1 mồi. Trả về SO của mồi đã dùng (để PlayerFishing biết hiệu ứng).
        /// </summary>
        public BaitSO ConsumeBait()
        {
            if (attachedBait == null || baitQuantity <= 0)
            {
                Debug.Log("Không có mồi để tiêu thụ!");
                return null;
            }
            if (baitQuantity > 0 && attachedBait != null) { 

            baitQuantity--;
            BaitSO consumedBaitType = attachedBait;
            Debug.Log($"Đã sử dụng 1 mồi {attachedBait.displayName}. Còn lại: {baitQuantity}.");

            // Nếu hết mồi thì gỡ bỏ
            if (baitQuantity == 0)
            {
                Debug.Log("Mồi đã hết, gỡ bỏ khỏi cần câu.");
                attachedBait = null;
                UpdateBaitVisuals();
            }
            return consumedBaitType;
            }
            return null;
        }

        public void UpdateBaitVisuals() { 
            if(_baitIconImage == null || _baitCountText == null) return;
            if(attachedBait != null && baitQuantity > 0)
            {
                _baitIconImage.gameObject.SetActive(true);
                _baitCountText.gameObject.SetActive(true);
                _baitIconImage.sprite = attachedBait.icon;
                _baitCountText.SetText(baitQuantity.ToString());
            }
            else
            {
                _baitIconImage.gameObject.SetActive(false);
                _baitCountText.gameObject.SetActive(false);
            }
        }
    }
}