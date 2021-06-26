using System;

namespace DudeRescueSquad.Core.Inventory
{
    public interface IEquipment
    {
        event Action<Item> OnEquipItem;
        event Action<Item> OnUnequipItem;

        bool HasItemEquipped();

        Item GetCurrentItem();

        void EquipItem(Item item);

        void UnequipItem(Item item);
    }
}