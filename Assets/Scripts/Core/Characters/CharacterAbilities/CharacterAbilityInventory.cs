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

        [SerializeField] private bool _canEquipPickedItemWhenUnequip = false;
        [SerializeReference] private BaseItem _equippedItemInstance = default;

        #endregion

        #region Public properties

        public InventoryEngine.Inventory MainInventory { get; set; }
        public InventoryEngine.Inventory WeaponInventory { get; set; }
        public InventoryEngine.Inventory HotbarInventory { get; set; }

        #endregion

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
        }

        /// <summary>
        /// Equips the weapon with the name passed in parameters
        /// </summary>
        /// <param name="weaponID"></param>
		protected virtual void EquipWeapon(bool fromQuickSlot, BaseItem itemInstance)
        {
            var previousWeapon = _characterHandleWeapon.CurrentWeapon;

            var weapon = InventoryCatalogItems.GetItemPrefabById<Weapons.BaseWeapon>(itemInstance.TemplateId);

            _characterHandleWeapon.ChangeWeapon(weapon, false, itemInstance);

            if (previousWeapon == null) return;

            if (fromQuickSlot) return;

            _inventory.Unequip(previousWeapon.WeaponData.Id);
        }

        #endregion

        #region Private methods

        private void EquipPickedItem(string itemId, InventorySlotItem slot, BaseItem itemInstance)
        {
            if (_canEquipPickedItemWhenUnequip)
            {
                var itemData = InventoryCatalogItems.GetItemDataById(itemId);

                // Check if current item is a weapon, else skip
                if (itemData.type != Enums.ItemTypes.WEAPON_ASSAULT && itemData.type != Enums.ItemTypes.WEAPON_MELEE) return;

                EquipWeapon(false, itemInstance);

                _equippedItemInstance = itemInstance;

                _inventory.Equip(slot, itemInstance);
            }
        }

        private void EquipQuickSlotSelected(BaseItem itemInstance)
        {
            var itemData = InventoryCatalogItems.GetItemDataById(itemInstance.TemplateId);

            // Check if current item is a weapon, else skip
            if (itemData.type != Enums.ItemTypes.WEAPON_ASSAULT && itemData.type != Enums.ItemTypes.WEAPON_MELEE) return;

            EquipWeapon(true, itemInstance);

            _equippedItemInstance = itemInstance;

            _inventory.EquipFromQuickSlot(itemInstance);
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
                EquipWeapon(false, null);
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
                    EquipPickedItem(eventData.ItemInstance.TemplateId, eventData.Slot, eventData.ItemInstance);
                    break;

                case InventoryEventType.EquipFromQuickSlot:
                    EquipQuickSlotSelected(eventData.ItemInstance);
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
