using UnityEngine;
using UnityEngine.AddressableAssets;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "ItemData", menuName = "Data/Item Data")]
    public class ItemData : ScriptableObject
    {
        public string UID;
        public string DisplayName;
        public Enums.ItemType Type;

        [Header("Equip properties")]
        public GameObject PrefabEquipable;

        public AssetReference AssetReference;
    }
}