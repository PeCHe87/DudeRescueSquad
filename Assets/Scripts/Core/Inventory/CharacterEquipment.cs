using System;
using System.Collections.Generic;

namespace DudeRescueSquad.Core.Inventory
{
    public class CharacterEquipment : IEquipment
    {
        #region Private properties

        private Item _equippedItem = null;
        private List<Item> _items = null;
        private bool _hasItemEquipped = false;

        #endregion

        #region IEquipment implementation

        public event Action<Item> OnEquipItem;
        public event Action<Item> OnUnequipItem;

        public Item GetCurrentItem()
        {
            return _equippedItem;
        }

        public bool HasItemEquipped()
        {
            return _hasItemEquipped;
        }

        public void EquipItem(Item item)
        {
            if (_hasItemEquipped)
            {
                var oldItem = _equippedItem;

                OnUnequipItem?.Invoke(oldItem);
            }

            _equippedItem = item;
            _hasItemEquipped = true;

            OnEquipItem?.Invoke(item);
        }

        public void UnequipItem(Item item)
        {
            _equippedItem = null;

            _hasItemEquipped = false;

            OnUnequipItem?.Invoke(item);
        }

        #endregion
    }
}