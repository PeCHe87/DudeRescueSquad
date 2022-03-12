using DudeRescueSquad.Core.Weapons;
using System;
using UnityEngine;

namespace DudeRescueSquad.Core.Inventory
{
    [Serializable]
    public struct ItemData
    {
        public string id;
        public Enums.ItemTypes type;
        public GameObject prefab;
        public BaseWeaponDefinition data;
    }
}