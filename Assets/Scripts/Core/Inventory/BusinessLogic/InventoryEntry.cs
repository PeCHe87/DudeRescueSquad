using System.Linq;

namespace DudeRescueSquad.Core.Inventory
{
    [System.Serializable]
    public class InventoryEntry
    {
        #region Private properties

        /// <summary>
        /// List of all slots on Inventory
        /// </summary>
        private InventorySlotItem[] _slots = null;

        /// <summary>
        /// List of slots for quick item access like equipped weapons
        /// </summary>
        private InventorySlotItem[] _quickSlots = null;

        private InventoryInstanceItems _instances = default;

        #endregion

        #region Private methods

        private InventorySlotItem GetEmptyQuickSlot()
        {
            return _quickSlots.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemId));
        }

        private InventorySlotItem GetEmptyRegularSlot()
        {
            return _slots.FirstOrDefault(x => string.IsNullOrEmpty(x.ItemId));
        }

        private InventorySlotItem GetQuickSlotBasedOnItemId(string itemId)
        {
            return _quickSlots.FirstOrDefault(x => x.ItemId.Equals(itemId));
        }

        #endregion

        #region Public methods

        public InventoryEntry(int slotsAmount, int quickSlotsAmount, InventoryInstanceItems instances)
        {
            _instances = instances;

            // Create slots
            _slots = new InventorySlotItem[slotsAmount];

            for (int i = 0; i < slotsAmount; i++)
            {
                _slots[i] = new InventorySlotItem(i);
            }

            // Create quick slots
            _quickSlots = new InventorySlotItem[quickSlotsAmount];

            for (int i = 0; i < quickSlotsAmount; i++)
            {
                _quickSlots[i] = new InventorySlotItem(i);
            }
        }

        public bool IsFull()
        {
            var emptySlots = _slots.Count(x => string.IsNullOrEmpty(x.ItemId));

            return emptySlots == 0;
        }

        public void Pick(string itemId, int amount)
        {
            var emptySlot = GetEmptyRegularSlot();

            emptySlot?.Setup(itemId, amount);

            _instances.AddItem(itemId, out var itemInstance);

            InventoryEvent.Trigger(InventoryEventType.PickSuccess, emptySlot, amount, itemInstance);
        }

        public void Equip(InventorySlotItem slot, BaseItem itemInstance)
        {
            // Remove it from inventory, cleaning the slot
            slot.Clean();

            // Move the current item to the quick slots, the first empty slot it finds
            var emptySlot = GetEmptyQuickSlot();

            if (emptySlot != null)
            {
                emptySlot.Setup(itemInstance.TemplateId, 0);

                InventoryEvent.Trigger(InventoryEventType.EquipOnQuickSlot, emptySlot, 0, itemInstance);
            }

            // Communicates about the success of the action
            InventoryEvent.Trigger(InventoryEventType.ItemEquipped, slot, 0, itemInstance);
        }

        public void EquipFromQuickSlot(BaseItem itemInstance)
        {
            // Communicates about the success of the action
            InventoryEvent.Trigger(InventoryEventType.ItemEquipped, null, 0, itemInstance);
        }

        public void Unequip(string itemId)
        {
            // Move the current item to the first empty regular slot
            var emptySlot = GetEmptyRegularSlot();

            if (emptySlot != null)
            {
                emptySlot.Setup(itemId, 0);
                InventoryEvent.Trigger(InventoryEventType.ItemUnEquipped, itemId, emptySlot, 0);
            }

            // Remove it from quick slots if it is there
           var quickSlot = GetQuickSlotBasedOnItemId(itemId);

            quickSlot?.Clean();
        }

        public bool GetItemInstance(string itemId, out BaseItem itemInstance)
        {
            return _instances.GetItemById(itemId, out itemInstance);
        }

        #endregion
    }
}