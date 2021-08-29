
namespace DudeRescueSquad.Core.Inventory
{
    /// <summary>
    /// The possible inventory related events
    /// </summary>
    public enum InventoryEventType
    {
        Pick,
        PickSuccess,
        Select,
        Click,
        Move,
        UseRequest,
        ItemUsed,
        EquipRequest,
        ItemEquipped,
        UnEquipRequest,
        ItemUnEquipped,
        Drop,
        Destroy,
        Error,
        Redraw,
        ContentChanged,
        InventoryOpens,
        InventoryCloseRequest,
        InventoryCloses,
        InventoryLoaded,
        MoveToQuickSlot,
        EquipOnQuickSlot
    }
}