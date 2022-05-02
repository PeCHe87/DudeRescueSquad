using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Events;
using UnityEngine;

namespace DudeRescueSquad.Core.Inventory.View
{
    /// <summary>
    /// Visual representation of InventoryEntry:
    ///   - quick slots
    ///   - inventory slots
    /// </summary>
    public class ViewInventory : MonoBehaviour, IGameEventListener<InventoryEvent>
    {
        [SerializeField] private ViewInventoryQuickSlot[] _quickSlots = default;

        #region Private properties

        private CharacterAbilityHandleWeapon _characterHandleWeapon = default;

        #endregion

        #region Public properties

        public CharacterAbilityHandleWeapon CharacterHandleWeapon => _characterHandleWeapon;

        #endregion

        #region Public methods

        public void Init(Character character)
        {
            _characterHandleWeapon = character.GetComponent<CharacterAbilityHandleWeapon>();

            for (int i = 0; i < _quickSlots.Length; i++)
            {
                var slot = _quickSlots[i];
                slot.Init(this);
            }
        }

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

        private void OnDestroy()
        {
            // Teardown all quick slots
            for (int i = 0; i < _quickSlots.Length; i++)
            {
                var slot = _quickSlots[i];

                slot.Teardown();
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
                case InventoryEventType.PickSuccess:
                    CheckQuickSlots(eventData.ItemInstance.TemplateId, eventData.ItemInstance);

                    break;
            }
        }

        #endregion

        #region Private methods

        private void CheckQuickSlots(string itemId, BaseItem itemInstance)
        {
            for (int i = 0; i < _quickSlots.Length; i++)
            {
                var slot = _quickSlots[i];

                if (!slot.IsEmpty) continue;

                slot.Setup(itemId, itemInstance);

                break;
            }

            // TODO: all quick slots are filled
        }

        #endregion
    }
}