
using DudeRescueSquad.Core.Events;

namespace DudeRescueSquad.Core.Inventory
{
    public struct InventoryEvent
    {
        static InventoryEvent e;

        public InventoryEventType EventType { get; private set; }
        public string ItemId { get; private set; }
        public InventorySlotItem Slot { get; private set; }
        public int Quantity { get; private set; }
        public InventoryEntry Inventory { get; private set; }
        public BaseItem ItemInstance { get; private set; }

        public InventoryEvent(InventoryEventType eventType, string itemId, InventorySlotItem slot, int quantity, InventoryEntry inventory = null, BaseItem itemInstance = null)
        {
            EventType = eventType;
            ItemId = itemId;
            Slot = slot;
            Quantity = quantity;
            Inventory = inventory;
            ItemInstance = itemInstance;
        }

        public static void Trigger(InventoryEventType eventType, string itemId, InventorySlotItem slot, int quantity, InventoryEntry inventory = null)
        {
            e.EventType = eventType;
            e.ItemId = itemId;
            e.Slot = slot;
            e.Quantity = quantity;
            e.Inventory = inventory;

            GameEventsManager.TriggerEvent(e);
        }

        public static void Trigger(InventoryEventType eventType, InventorySlotItem slot, int quantity, BaseItem itemInstance)
        {
            e.EventType = eventType;
            e.Slot = slot;
            e.Quantity = quantity;
            e.ItemInstance = itemInstance;

            GameEventsManager.TriggerEvent(e);
        }
    }
}