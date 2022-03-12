using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core.Inventory
{
    public class InventoryCatalogItems : MonoBehaviour
    {
        [SerializeField] private ItemData[] _items = null;

        private static Dictionary<string, ItemData> _dictionaryItems = null;

        public void Init()
        {
            _dictionaryItems = new Dictionary<string, ItemData>(_items.Length);

            for (int i = 0; i < _items.Length; i++)
            {
                var item = _items[i];
                _dictionaryItems.Add(item.id, item);
            }
        }

        public static ItemData GetItemDataById(string id)
        {
            _dictionaryItems.TryGetValue(id, out var item);

            return item;
        }

        public static T GetItemPrefabById<T>(string id)
        {
            if (_dictionaryItems.TryGetValue(id, out var item))
            {
                return item.prefab.GetComponent<T>();
            }

            return default(T);
        }
    }
}