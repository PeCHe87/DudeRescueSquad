
namespace DudeRescueSquad.Core.Inventory
{
    [System.Serializable]
    public class InventorySlotItem
    {
        public int Id { get; private set; }
        public string ItemId { get; private set; }

        private int _currentAmount = 0;
        private int _maxAmount = 0;
        private bool _isStackable = false;

        public InventorySlotItem(int id)
        {
            this.Id = id;
        }

        public void Setup(string itemId, int amount)
        {
            ItemId = itemId;
            _currentAmount = amount;
        }

        public void Clean()
        {
            ItemId = string.Empty;
            _currentAmount = 0;
        }
    }
}