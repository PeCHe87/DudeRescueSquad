using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.Inventory.View;
using UnityEngine;

namespace DudeRescueSquad.Core.LevelManagement
{
    public class LevelInventoryLoader : MonoBehaviour
    {
        [SerializeField] private ViewInventory _inventoryView = null;
        [SerializeField] private ViewItemPicker[] _pickers = null;

        private InventoryEntry _inventory = null;

        private void Start()
        {
            // TODO: this should be called from an initializator for the whole level loaders
            Initialization();
        }

        public void Initialization()
        {
            _inventory = new InventoryEntry(10, 3);

            // Load item pickers
            foreach (var picker in _pickers)
            {
                picker.Setup(_inventory);
            }

            InventoryEvent.Trigger(InventoryEventType.InventoryLoaded, string.Empty, null, 0, _inventory);
        }
    }
}