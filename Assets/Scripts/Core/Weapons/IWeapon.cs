using DudeRescueSquad.Core.Inventory;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    public interface IWeapon
    {
        IWeaponDefinition WeaponData { get; }
        string DisplayName { get; }
        /// The name of the inventory item corresponding to this weapon. Automatically set (if needed) by InventoryEngineWeapon
        string WeaponID { get; set; }
        Enums.ItemTypes WeaponType { get; }
        Vector3 WeaponAttachmentOffset { get; }
        void WeaponInputStart();
        bool CanBeUsed();
    }
}