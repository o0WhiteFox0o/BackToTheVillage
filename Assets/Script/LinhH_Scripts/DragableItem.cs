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
        // private properties
        private ItemScriptableObject _itemScriptableObj;
        private Transform _parent;
        private int _quantity;

        [SerializeField] private Image _image;
        [SerializeField] private TMP_Text _countText;

        // public fields
        public Transform parent { get => _parent; set => _parent = value; }
        public ItemScriptableObject itemScriptableObj { get => _itemScriptableObj; set => _itemScriptableObj = value; }
        public int quantity { get => _quantity; set => _quantity = value; }


        void Start()
        {
            CheckPropertiesValue();
        }


        /// <summary>
        /// Thiết lập các thông tin của item.
        /// </summary>
        /// <param name="newItem"> Scriptable Object của item. </param>
        /// <param name="quantity"> Số lượng item. </param>
        public void InitializeItem(ItemScriptableObject newItem, int quantity)
        {
            _itemScriptableObj = newItem;
            _quantity = quantity;
            _image.sprite = _itemScriptableObj.icon;

            RefreshCount();
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
            _parent = transform.parent;
            _image.raycastTarget = false;
            transform.SetParent(transform.root);

        }


        public void OnDrag(PointerEventData eventData)
        {
            transform.position = Input.mousePosition;
        }


        public void OnEndDrag(PointerEventData eventData)
        {
            _image.raycastTarget = true;
            transform.SetParent(_parent);
        }


        /// <summary>
        /// Cập nhật số lượng item (thêm vào hoặc bớt đi một số lượng).
        /// </summary>
        /// <param name="count">Số lượng thêm vào hoặc bớt đi của item.</param>
        public void UpdateCount(int count)
        {
            _quantity += count;
            RefreshCount();
        }


        private void CheckPropertiesValue()
        {
            if (_image == null ||
                _countText == null)
            {
                Debug.LogError("There is a component was not assigned in " + gameObject.name + ".");
            }
        }
    }
}