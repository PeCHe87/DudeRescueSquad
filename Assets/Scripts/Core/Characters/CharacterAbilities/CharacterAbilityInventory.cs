using UnityEngine;
using DudeRescueSquad.Tools;
using System.Collections.Generic;
using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.Events;
using System;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// Add this component to a character and it'll be able to control an inventory
    /// </summary>
    [HiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Dude Rescue Squad/Character/Abilities/Character Ability Inventory")]
    public class CharacterAbilityInventory : CharacterAbility, IGameEventListener<InventoryEvent>
    {
        #region Inspector properties

        public enum WeaponRotationModes { Normal, AddEmptySlot, AddInitialWeapon }

        [Serializable]
        public struct ItemData
        {
            public string id;
            public Enums.ItemTypes type;
            public GameObject prefab;
        }

        [SerializeField] private ItemData[] _items = null;      // TODO: this should be moved outside and be more informative
        [SerializeField] private bool _canEquipPickedItemWhenUnequip = false;

        #endregion

        #region Public properties

        public InventoryEngine.Inventory MainInventory { get; set; }
        public InventoryEngine.Inventory WeaponInventory { get; set; }
        public InventoryEngine.Inventory HotbarInventory { get; set; }

        #endregion

        private Dictionary<string, ItemData> _dictionaryItems = null;

        #region Protected properties

        protected List<int> _availableWeapons;
        protected List<string> _availableWeaponsIDs;
        protected CharacterAbilityHandleWeapon _characterHandleWeapon;
        protected string _nextWeaponID;
        protected bool _nextFrameWeapon = false;
        protected string _nextFrameWeaponName;
        protected const string _emptySlotWeaponName = "_EmptySlotWeaponName";
        protected const string _initialSlotWeaponName = "_InitialSlotWeaponName";
        private InventoryEntry _inventory = null;

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

        #region Protected methods

        /// <summary>
        /// Grabs all inventories, and fills weapon lists
        /// </summary>
		protected virtual void Setup()
        {
            _characterHandleWeapon = GetComponent<CharacterAbilityHandleWeapon>();

            FillAvailableWeaponsLists();
        }

        /// <summary>
        /// Fills the weapon list. The weapon list will be used to determine what weapon we can switch to
        /// </summary>
		protected virtual void FillAvailableWeaponsLists()
        {
            _dictionaryItems = new Dictionary<string, ItemData>(_items.Length);
            foreach (var item in _items)
            {
                _dictionaryItems.Add(item.id, item);
            }
        }

        /// <summary>
        /// Equips the weapon with the name passed in parameters
        /// </summary>
        /// <param name="weaponID"></param>
		protected virtual void EquipWeapon(string itemId)
        {
            var previousWeapon = _characterHandleWeapon.CurrentWeapon;

            var weapon = GetItemPrefabById<Weapons.BaseWeapon>(itemId);

            _characterHandleWeapon.ChangeWeapon(weapon, itemId);

            if (previousWeapon == null) return;

            _inventory.Unequip(previousWeapon.WeaponData.Id);

            /*if ((weaponID.Equals(_emptySlotWeaponName)) && (_characterHandleWeapon != null))
            {
                InventoryEvent.Trigger(InventoryEventType.UnEquipRequest, null, WeaponInventoryName, WeaponInventory.Content[0], 0, 0);
                _characterHandleWeapon.ChangeWeapon(null, _emptySlotWeaponName, false);
                InventoryEvent.Trigger(InventoryEventType.Redraw, null, WeaponInventory.name, null, 0, 0);
                return;
            }

            if ((weaponID.Equals(_initialSlotWeaponName)) && (_characterHandleWeapon != null))
            {
                InventoryEvent.Trigger(InventoryEventType.UnEquipRequest, null, WeaponInventoryName, WeaponInventory.Content[0], 0, 0);
                _characterHandleWeapon.ChangeWeapon(_characterHandleWeapon.InitialWeapon, _emptySlotWeaponName, false);
                InventoryEvent.Trigger(InventoryEventType.Redraw, null, WeaponInventory.name, null, 0, 0);
                return;
            }

            for (int i = 0; i < MainInventory.Content.Length; i++)
            {
                if (InventoryItem.IsNull(MainInventory.Content[i])) continue;

                if (MainInventory.Content[i].ItemID.Equals(weaponID))
                {
                    InventoryEvent.Trigger(InventoryEventType.EquipRequest, null, MainInventory.name, MainInventory.Content[i], 0, i);
                    break;
                }
            }*/
        }

        #endregion

        #region Private methods

        private void EquipPickedItem(string itemId, InventorySlotItem slot)
        {
            if (_canEquipPickedItemWhenUnequip)
            {
                var itemData = GetItemDataById(itemId);

                // Check if current item is a weapon, else skip
                if (itemData.type != Enums.ItemTypes.WEAPON_ASSAULT && itemData.type != Enums.ItemTypes.WEAPON_MELEE) return;

                _inventory.Equip(itemId, slot);

                EquipWeapon(itemId);
            }
        }

        private ItemData GetItemDataById(string id)
        {
            _dictionaryItems.TryGetValue(id, out var item);

            return item;
        }

        private T GetItemPrefabById<T>(string id)
        {
            for (int i = 0; i < _items.Length; i++)
            {
                var item = _items[i];

                if (item.id.Equals(id))
                {
                    return item.prefab.GetComponent<T>();
                }
            }

            return default(T);
        }

        #endregion

        #region ICharacterAbility implementation

        public override void Initialization()
        {
            base.Initialization();

            Setup();
        }

        /// <summary>
        /// Every frame we check if it's needed to update the ammo display
        /// </summary>
        public override void Process()
        {
            if (_nextFrameWeapon)
            {
                EquipWeapon(_nextFrameWeaponName);
                _nextFrameWeapon = false;
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
                case InventoryEventType.InventoryLoaded:
                    _inventory = eventData.Inventory;
                    break;

                case InventoryEventType.Pick:
                    _inventory?.Pick(eventData.ItemId, eventData.Quantity);
                    break;

                case InventoryEventType.PickSuccess:
                    EquipPickedItem(eventData.ItemId, eventData.Slot);

                    break;
            }
        }

        #endregion

        /*

        /// <summary>
        /// On handle input, we watch for the switch weapon button, and switch weapon if needed
        /// </summary>
		protected override void HandleInput()
        {
            if (!AbilityPermitted
                || (_condition.CurrentState != CharacterStates.CharacterConditions.Normal))
            {
                return;
            }
            if (_inputManager.SwitchWeaponButton.State.CurrentState == MMInput.ButtonStates.ButtonDown)
            {
                SwitchWeapon();
            }
        }

        /// <summary>
        /// Determines the name of the next weapon in line
        /// </summary>
		protected virtual void DetermineNextWeaponName()
        {
            if (InventoryItem.IsNull(WeaponInventory.Content[0]))
            {
                _nextWeaponID = _availableWeaponsIDs[0];
                return;
            }

            if ((_nextWeaponID == _emptySlotWeaponName) || (_nextWeaponID == _initialSlotWeaponName))
            {
                _nextWeaponID = _availableWeaponsIDs[0];
                return;
            }

            for (int i = 0; i < _availableWeaponsIDs.Count; i++)
            {
                if (_availableWeaponsIDs[i] == WeaponInventory.Content[0].ItemID)
                {
                    if (i == _availableWeaponsIDs.Count - 1)
                    {
                        switch (WeaponRotationMode)
                        {
                            case WeaponRotationModes.AddEmptySlot:
                                _nextWeaponID = _emptySlotWeaponName;
                                return;
                            case WeaponRotationModes.AddInitialWeapon:
                                _nextWeaponID = _initialSlotWeaponName;
                                return;
                        }

                        _nextWeaponID = _availableWeaponsIDs[0];
                    }
                    else
                    {
                        _nextWeaponID = _availableWeaponsIDs[i + 1];
                    }
                }
            }
        }

        

        /// <summary>
        /// Switches to the next weapon in line
        /// </summary>
		protected virtual void SwitchWeapon()
        {
            // if there's no character handle weapon component, we can't switch weapon, we do nothing and exit
            if ((_characterHandleWeapon == null) || (WeaponInventory == null))
            {
                return;
            }

            FillAvailableWeaponsLists();

            // if we only have 0 or 1 weapon, there's nothing to switch, we do nothing and exit
            if (_availableWeaponsIDs.Count <= 0)
            {
                return;
            }

            DetermineNextWeaponName();
            EquipWeapon(_nextWeaponID);
            PlayAbilityStartFeedbacks();
            PlayAbilityStartSfx();
        }

        

        protected override void OnDeath()
        {
            base.OnDeath();
            if (WeaponInventory != null)
            {
                MMInventoryEvent.Trigger(MMInventoryEventType.UnEquipRequest, null, WeaponInventoryName, WeaponInventory.Content[0], 0, 0);
            }
        }

        
        */
    }
}
