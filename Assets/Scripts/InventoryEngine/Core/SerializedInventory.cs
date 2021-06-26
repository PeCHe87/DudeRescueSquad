using System;

namespace DudeRescueSquad.InventoryEngine
{
    /// <summary>
    /// Serialized class to help store / load inventories from files.
    /// </summary>
    [Serializable]
    public class SerializedInventory
    {
        public int NumberOfRows;
        public int NumberOfColumns;
        public string InventoryName = "Inventory";
        public Inventory.InventoryTypes InventoryType;
        public bool DrawContentInInspector = false;
        public string[] ContentType;
        public int[] ContentQuantity;
    }
}