
namespace DudeRescueSquad.Core.Inventory
{
    [System.Serializable]
    public class InventorySlotItem
    {
        public string ItemId { get; private set; }

        private int _currentAmount = 0;
        private int _maxAmount = 0;
        private bool _isStackable = false;

        public void Setup(string id, int amount)
        {
            ItemId = id;
            _currentAmount = amount;
        }

        public void Clean()
        {
            ItemId = string.Empty;
            _currentAmount = 0;
        }
    }
}