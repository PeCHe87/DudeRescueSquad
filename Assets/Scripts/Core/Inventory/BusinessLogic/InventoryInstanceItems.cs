using DudeRescueSquad.Core.Inventory.Items.Weapons;
using DudeRescueSquad.Core.Weapons;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core.Inventory
{
    /// <summary>
    /// Represents the list of player's items
    /// </summary>
    public class InventoryInstanceItems : MonoBehaviour
    {
        [SerializeReference] private List<BaseItem> _items = default;

        private Dictionary<string, BaseItem> _dictionaryItems = null;
        private int _instanceId = 0;

        public void Init()
        {
            _items = new List<BaseItem>(0);
            _dictionaryItems = new Dictionary<string, BaseItem>(0);
        }

        public bool GetItemById(string id, out BaseItem item)
        {
            if (!_dictionaryItems.TryGetValue(id, out item)) return false;

            return true;
        }

        public void AddItem(string templateId, out BaseItem item)
        {
            var instanceId = $"{templateId}_{_instanceId}";
            var itemAdded = false;

            var itemTemplate = InventoryCatalogItems.GetItemDataById(templateId);

            if (itemTemplate.type == Enums.ItemTypes.WEAPON_ASSAULT)
            {
                var itemWeaponData = (WeaponAssaultData)itemTemplate.data;
                var ammo = itemWeaponData.MaxAmmo;

                item = new AssaultWeaponItem(instanceId, templateId, ammo);

                _items.Add(item);
                _dictionaryItems.Add(item.InstanceId, item);

                itemAdded = true;
            }
            else if (itemTemplate.type == Enums.ItemTypes.WEAPON_MELEE)
            {
                item = new MeleeWeaponItem(instanceId, templateId);

                _items.Add(item);
                _dictionaryItems.Add(item.InstanceId, item);

                itemAdded = true;
            }
            else
            {
                item = new BaseItem();
            }

            if (!itemAdded) return;

            _instanceId++;
        }
    }
}