using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Weapons;
using DudeResqueSquad;
using System;
using System.Collections;
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

        [Header("Assault weapon info")]
        [SerializeField] private GameObject _assaultInfo = default;
        [SerializeField] private TextMeshProUGUI _txtAmmo = default;
        [SerializeField] private TextMeshProUGUI _txtMagazine = default;
        [SerializeField] private Image _imgReloading = default;

        #endregion

        #region Private properties

        private string _itemId = default;
        private bool _isReloading = false;
        private ViewInventory _inventory = default;
        private WeaponAssaultData _assaultWeaponData = default;
        private WeaponAssault _currentWeapon = default;
        private bool _isAssault = false;
        private BaseItem _itemInstance = default;

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

        public void Init(ViewInventory inventory)
        {
            _inventory = inventory;

            _btn.onClick.AddListener(Select);

            WeaponAssault.OnShoot += OnAssaultShooting;
            WeaponAssault.OnStartReloading += OnAssaultStartReloading;
            WeaponAssault.OnStopReloading += OnAssaultStopReloading;

            Setup(string.Empty, null);
        }

        public void Teardown()
        {
            _btn.onClick.RemoveAllListeners();

            WeaponAssault.OnShoot -= OnAssaultShooting;
            WeaponAssault.OnStartReloading -= OnAssaultStartReloading;
            WeaponAssault.OnStopReloading -= OnAssaultStopReloading;
        }

        public void Setup(string id, BaseItem itemInstance)
        {
            _itemId = id;
            _itemInstance = itemInstance;

            if (string.IsNullOrEmpty(id))
            {
                _txtName.text = string.Empty;
                _icon.sprite = null;
                _selectionBorder.enabled = false;

                _isAssault = false;

                HideAssaultInfo();
            }
            else
            {
                // Get item based on id
                var item = InventoryCatalogItems.GetItemDataById(id);

                _txtName.text = item.data.DisplayName;
                _icon.sprite = item.data.Icon;
                _selectionBorder.enabled = true;

                if (item.type == Enums.ItemTypes.WEAPON_ASSAULT)
                {
                    _isAssault = true;

                    _assaultWeaponData = (WeaponAssaultData)_inventory.CharacterHandleWeapon.CurrentWeapon.WeaponData;
                    _currentWeapon = (WeaponAssault)_inventory.CharacterHandleWeapon.CurrentWeapon;

                    StopReloading();

                    ShowAssaultInfo(_currentWeapon.CurrentAmmo);
                }
                else
                {
                    _isAssault = false;

                    HideAssaultInfo();
                }
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

                    if (_itemId.Equals(eventData.ItemInstance.TemplateId)) return;

                    Unselect();

                    break;

                case InventoryEventType.EquipOnQuickSlot:
                    if (string.IsNullOrEmpty(_itemId)) return;

                    if (_itemId.Equals(eventData.ItemInstance.TemplateId)) return;

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

            InventoryEvent.Trigger(InventoryEventType.EquipFromQuickSlot, null, 1, _itemInstance);
        }

        private void Unselect()
        {
            _selectionBorder.enabled = false;
        }

        #endregion

        #region Assault weapon information

        private void HideAssaultInfo()
        {
            _assaultInfo.Toggle(false);
        }

        private void ShowAssaultInfo(int ammo)
        {
            _txtMagazine.text = $"{_assaultWeaponData.MaxAmmo}";

            UpdateCurrentAmmo(ammo);

            _assaultInfo.Toggle(true);
        }

        private void UpdateCurrentAmmo(int amount)
        {
            _txtAmmo.text = $"{amount}";
        }

        private void StartReloading()
        {
            _isReloading = true;

            _imgReloading.fillAmount = 0;

            _imgReloading.enabled = true;

            StartCoroutine(Reloading());
        }

        private IEnumerator Reloading()
        {
            var duration = _assaultWeaponData.ReloadingTime;
            float progress = 0;

            while (progress < duration && _isReloading)
            {
                progress += Time.deltaTime;

                _imgReloading.fillAmount = progress / duration;

                yield return new WaitForEndOfFrame();
            }

            StopReloading();
        }

        private void StopReloading()
        {
            _isReloading = false;

            _imgReloading.enabled = false;
        }

        private void OnAssaultShooting(CustomEventArgs.WeaponShootEventArgs evt)
        {
            if (!IsAssaultWeapon()) return;

            if (!_itemId.Equals(evt.itemId)) return;

            UpdateCurrentAmmo(evt.currentAmmo);
        }

        private void OnAssaultStartReloading(CustomEventArgs.WeaponStartReloadingEventArgs evt)
        {
            if (!IsAssaultWeapon()) return;

            if (!_itemId.Equals(evt.itemId)) return;

            StartReloading();
        }

        private void OnAssaultStopReloading(CustomEventArgs.WeaponStopReloadingEventArgs evt)
        {
            if (!IsAssaultWeapon()) return;

            if (!_itemId.Equals(evt.itemId)) return;

            StopReloading();

            UpdateCurrentAmmo(evt.currentAmmo);
        }

        private bool IsAssaultWeapon()
        {
            if (string.IsNullOrEmpty(_itemId)) return false;

            if (!_isAssault) return false;

            return true;
        }

        #endregion
    }
}