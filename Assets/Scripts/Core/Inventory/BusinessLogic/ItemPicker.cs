using System;

namespace DudeRescueSquad.Core.Inventory
{
    /// <summary>
    /// Add an instance of this logic to an object so it can be picked and added to an inventory
    /// </summary>
    public class ItemPicker
    {
        private string _itemId = string.Empty;
        private int _amount = 0;
        private InventoryEntry _inventory = null;

        public ItemPicker(string id, int amount, InventoryEntry inventory)
        {
            _itemId = id;
            _amount = amount;
            _inventory = inventory;
        }

        /// <summary>
        /// Tries to Pick this item and adds it to the inventory
        /// </summary>
        public void Pick(Action onSuccess, Action<string> onFail)
        {
            if (!CanPick(out var message))
            {
                onFail(message);

                return;
            }

            int pickedQuantity = _amount; //  TODO: DetermineMaxQuantity();

            InventoryEvent.Trigger(InventoryEventType.Pick, _itemId, null, pickedQuantity);

            onSuccess();
        }

        /// <summary>
        /// Checks if it is possible pick the current item, else it returns false and the message error
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        private bool CanPick(out string message)
        {
            message = string.Empty;

            if (_inventory == null)
            {
                message = "no_inventory";
                return false;
            }

            bool isFull = _inventory.IsFull();

            if (isFull)
            {
                message = "full_inventory";
            }

            return !isFull;
        }
    }
}