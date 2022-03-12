using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.Inventory.View;
using DudeRescueSquad.Core.Weapons;
using UnityEngine;

namespace DudeRescueSquad.Core.LevelManagement
{
    public class LevelInventoryLoader : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        [SerializeField] private ViewInventory _inventoryView = null;
        [SerializeField] private ViewItemPicker[] _pickers = null;
        [SerializeField] private InventoryCatalogItems _catalog = default;

        private InventoryEntry _inventory = null;

        #region GameEventListener<GameLevelEvent> implementation

        /// <summary>
        /// Check different events related with game level
        /// </summary>
        /// <param name="eventData">Inventory event.</param>
        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.LevelLoaded:
                    Initialization();
                    break;
            }
        }

        #endregion

        #region Unity Events

        /// <summary>
        /// On enable, we start listening for GameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for GameEvents. You may want to extend that to stop listening to other types of events.
        /// </summary>
        protected void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        #endregion

        private void Initialization()
        {
            _catalog.Init();

            // Create Inventory entry
            _inventory = new InventoryEntry(10, 3);

            // Get pickers
            _pickers = FindObjectsOfType<ViewItemPicker>();

            // Load item pickers
            foreach (var picker in _pickers)
            {
                picker.Setup(_inventory);
            }

            // Get inventory view
            _inventoryView = FindObjectOfType<ViewInventory>();

            // Init inventory view
            _inventoryView.Init();

            // Communicate that inventory was loaded
            InventoryEvent.Trigger(InventoryEventType.InventoryLoaded, string.Empty, null, 0, _inventory);
        }
    }
}