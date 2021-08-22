using System.Collections.Generic;

namespace DudeRescueSquad.Core.Inventory
{
    public class InventoryItemDataProvider
    {
        private Dictionary<string, InventoryItemData> _items = null;

        public InventoryItemData GetItemById(string id)
        {
            _items.TryGetValue(id, out var item);

            return item;
        }
    }
}