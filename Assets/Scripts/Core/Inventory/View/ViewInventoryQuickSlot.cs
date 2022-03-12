using DudeRescueSquad.Core.Events;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.Core.Inventory.View
{
    public class ViewInventoryQuickSlot : MonoBehaviour, IGameEventListener<InventoryEvent>
    {
        #region Inspector properties

        [SerializeField] private Button _btn = default;
        [SerializeField] private TextMeshProUGUI _txtName = default;
        [SerializeField] private Image _icon = default;
        [SerializeField] private Image _selectionBorder = default;

        #endregion

        #region Private properties

        private string _itemId = default;

        #endregion

        #region Public properties

        public bool IsEmpty => string.IsNullOrEmpty(_itemId);

        #endregion

        #region Unity methods

        /// <summary>
        /// On enable, we start listening for GameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        protected void OnEnable()
        {
            this.EventStartListening<InventoryEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for GameEvents. You may want to extend that to stop listening to other types of events.
        /// </summary>
        protected void OnDisable()
        {
            this.EventStopListening<InventoryEvent>();
        }

        #endregion

        #region Public methods

        public void Init()
        {
            _btn.onClick.AddListener(Select);

            Setup(string.Empty);
        }

        public void Teardown()
        {
            _btn.onClick.RemoveAllListeners();
        }

        public void Setup(string id)
        {
            _itemId = id;

            if (string.IsNullOrEmpty(id))
            {
                _txtName.text = string.Empty;
                _icon.sprite = null;
                _selectionBorder.enabled = false;
            }
            else
            {
                // Get item based on id
                var item = InventoryCatalogItems.GetItemDataById(id);

                _txtName.text = item.data.DisplayName;
                _icon.sprite = item.data.Icon;
                _selectionBorder.enabled = true;
            }
        }

        #endregion

        #region GameEventListener implementation

        /// <summary>
        /// Watches for InventoryLoaded events
        /// When an inventory gets loaded, if it's our WeaponInventory, we check if there's already a weapon equipped, and if yes, we equip it
        /// </summary>
        /// <param name="eventData">Inventory event.</param>
        public virtual void OnGameEvent(InventoryEvent eventData)
        {
            switch (eventData.EventType)
            {
                case InventoryEventType.EquipFromQuickSlot:
                    if (string.IsNullOrEmpty(_itemId)) return;

                    if (_itemId.Equals(eventData.ItemId)) return;

                    Unselect();

                    break;

                case InventoryEventType.EquipOnQuickSlot:
                    if (string.IsNullOrEmpty(_itemId)) return;

                    if (_itemId.Equals(eventData.ItemId)) return;

                    Unselect();

                    break;

            }
        }

        #endregion

        #region Private methods

        private void Select()
        {
            if (string.IsNullOrEmpty(_itemId)) return;

            _selectionBorder.enabled = true;

            InventoryEvent.Trigger(InventoryEventType.EquipFromQuickSlot, _itemId);
        }

        private void Unselect()
        {
            _selectionBorder.enabled = false;
        }

        #endregion
    }
}