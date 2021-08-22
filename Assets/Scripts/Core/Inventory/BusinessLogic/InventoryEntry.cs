
using System.Linq;

namespace DudeRescueSquad.Core.Inventory
{
    [System.Serializable]
    public class InventoryEntry
    {
        /// <summary>
        /// List of all slots on Inventory
        /// </summary>
        private InventorySlotItem[] _slots = null;

        /// <summary>
        /// List of slots for quick item access like equipped weapons
        /// </summary>
        private InventorySlotItem[] _quickSlots = null;

        /// <summary>
        /// Represents the current equipped slot from the quick ones
        /// </summary>
        private int _equippedQuickSlot = -1;

        public InventoryEntry(int slotsAmount, int quickSlotsAmount)
        {
            // Create slots
            _slots = new InventorySlotItem[slotsAmount];

            for (int i = 0; i < slotsAmount; i++)
            {
                _slots[i] = new InventorySlotItem();
            }

            // Create quick slots
            _quickSlots = new InventorySlotItem[quickSlotsAmount];

            for (int i = 0; i < quickSlotsAmount; i++)
            {
                _quickSlots[i] = new InventorySlotItem();
            }
        }

        public bool IsFull()
        {
            var emptySlots = _slots.Count(x => string.IsNullOrEmpty(x.ItemId));

            return emptySlots == 0;
        }

        public void Pick(string itemId, int amount)
        {
            var emptySlot = _slots.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemId));

            emptySlot?.Setup(itemId, amount);

            InventoryEvent.Trigger(InventoryEventType.PickSuccess, itemId, emptySlot, amount);
        }

        public void Equip(string itemId, InventorySlotItem slot)
        {
            // Remove it from inventory, cleaning the slot
            slot.Clean();

            // Move the current item to the quick slots, the first empty slot it finds
            var emptySlot = _quickSlots.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemId));

            if (emptySlot != null)
            {
                emptySlot.Setup(itemId, 0);
                InventoryEvent.Trigger(InventoryEventType.MoveToQuickSlot, itemId, emptySlot, 0);
            }

            // Communicates about the success of the action
            InventoryEvent.Trigger(InventoryEventType.ItemEquipped, itemId, slot, 0);
        }
    }
}