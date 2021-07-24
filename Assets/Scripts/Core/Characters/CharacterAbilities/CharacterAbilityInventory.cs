using UnityEngine;
using DudeRescueSquad.Tools;
using DudeRescueSquad.InventoryEngine;
using System.Collections.Generic;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// Add this component to a character and it'll be able to control an inventory
    /// </summary>
    [HiddenProperties("AbilityStopFeedbacks")]
    [AddComponentMenu("Dude Rescue Squad/Character/Abilities/Character Ability Inventory")]
    public class CharacterAbilityInventory : CharacterAbility, GameEventListener<InventoryEvent>
    {
        #region Inspector properties

        public enum WeaponRotationModes { Normal, AddEmptySlot, AddInitialWeapon }

        /// the name of the main inventory for this character
        [Tooltip("the name of the main inventory for this character")]
        public string MainInventoryName;
        /// the name of the inventory where this character stores weapons
        [Tooltip("the name of the inventory where this character stores weapons")]
        public string WeaponInventoryName;
        /// the name of the hotbar inventory for this character
        [Tooltip("the name of the hotbar inventory for this character")]
        public string HotbarInventoryName;
        /// the rotation mode for weapons : Normal will cycle through all weapons, AddEmptySlot will return to empty hands, AddOriginalWeapon will cycle back to the original weapon
		[Tooltip("if this is true, will add an empty slot to the weapon rotation")]
        public WeaponRotationModes WeaponRotationMode = WeaponRotationModes.Normal;

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
            GrabInventories();

            _characterHandleWeapon = GetComponent<CharacterAbilityHandleWeapon>();

            FillAvailableWeaponsLists();
        }

        /// <summary>
        /// Grabs any inventory it can find that matches the names set in the inspector
        /// </summary>
		protected virtual void GrabInventories()
        {
            if (MainInventory == null)
            {
                GameObject mainInventoryTmp = GameObject.Find(MainInventoryName);
                if (mainInventoryTmp != null) { MainInventory = mainInventoryTmp.GetComponent<InventoryEngine.Inventory>(); }
            }

            if (WeaponInventory == null)
            {
                GameObject weaponInventoryTmp = GameObject.Find(WeaponInventoryName);
                if (weaponInventoryTmp != null) { WeaponInventory = weaponInventoryTmp.GetComponent<InventoryEngine.Inventory>(); }
            }

            if (HotbarInventory == null)
            {
                GameObject hotbarInventoryTmp = GameObject.Find(HotbarInventoryName);
                if (hotbarInventoryTmp != null) { HotbarInventory = hotbarInventoryTmp.GetComponent<InventoryEngine.Inventory>(); }
            }

            if (MainInventory != null) { MainInventory.SetOwner(this.gameObject); MainInventory.TargetTransform = this.transform; }
            if (WeaponInventory != null) { WeaponInventory.SetOwner(this.gameObject); WeaponInventory.TargetTransform = this.transform; }
            if (HotbarInventory != null) { HotbarInventory.SetOwner(this.gameObject); HotbarInventory.TargetTransform = this.transform; }
        }

        /// <summary>
        /// Fills the weapon list. The weapon list will be used to determine what weapon we can switch to
        /// </summary>
		protected virtual void FillAvailableWeaponsLists()
        {
            _availableWeaponsIDs = new List<string>();

            if ((_characterHandleWeapon == null) || (WeaponInventory == null)) return;

            _availableWeapons = MainInventory.InventoryContains(ItemClasses.Weapon);

            foreach (int index in _availableWeapons)
            {
                _availableWeaponsIDs.Add(MainInventory.Content[index].ItemID);
            }

            if (!InventoryItem.IsNull(WeaponInventory.Content[0]))
            {
                _availableWeaponsIDs.Add(WeaponInventory.Content[0].ItemID);
            }

            _availableWeaponsIDs.Sort();
        }

        /// <summary>
        /// Equips the weapon with the name passed in parameters
        /// </summary>
        /// <param name="weaponID"></param>
		protected virtual void EquipWeapon(string weaponID)
        {
            if ((weaponID.Equals(_emptySlotWeaponName)) && (_characterHandleWeapon != null))
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
            }
        }

        #endregion

        #region ICharacterAbility implementation

        public override void Initialization()
        {
            base.Initialization();

            this.Setup();
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
        /// <param name="inventoryEvent">Inventory event.</param>
        public virtual void OnGameEvent(InventoryEvent inventoryEvent)
        {
            if (inventoryEvent.InventoryEventType == InventoryEventType.InventoryLoaded)
            {
                if (inventoryEvent.TargetInventoryName == WeaponInventoryName)
                {
                    this.Setup();

                    if (WeaponInventory != null)
                    {
                        if (!InventoryItem.IsNull(WeaponInventory.Content[0]))
                        {
                            _characterHandleWeapon.Setup();
                            WeaponInventory.Content[0].Equip();
                        }
                    }
                }
            }

            if (inventoryEvent.InventoryEventType == InventoryEventType.Pick)
            {
                bool isSubclass = (inventoryEvent.EventItem.GetType().IsSubclassOf(typeof(InventoryWeapon)));
                bool isClass = (inventoryEvent.EventItem.GetType() == typeof(InventoryWeapon));
                if (isClass || isSubclass)
                {
                    InventoryWeapon inventoryWeapon = (InventoryWeapon)inventoryEvent.EventItem;
                    switch (inventoryWeapon.AutoEquipMode)
                    {
                        case InventoryWeapon.AutoEquipModes.NoAutoEquip:
                            // we do nothing
                            break;

                        case InventoryWeapon.AutoEquipModes.AutoEquip:
                            _nextFrameWeapon = true;
                            _nextFrameWeaponName = inventoryEvent.EventItem.ItemID;
                            break;

                        case InventoryWeapon.AutoEquipModes.AutoEquipIfEmptyHanded:
                            if (_characterHandleWeapon.CurrentWeapon == null)
                            {
                                _nextFrameWeapon = true;
                                _nextFrameWeaponName = inventoryEvent.EventItem.ItemID;
                            }
                            break;
                    }
                }
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
