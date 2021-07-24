using UnityEngine;
using DudeRescueSquad.Tools;
using System;
using DudeRescueSquad.InventoryEngine;
using DudeRescueSquad.Core.Weapons;
using DudeRescueSquad.Core.Characters;

namespace DudeRescueSquad.Core
{
    /// <summary>
    /// Weapon item in the Game
    /// </summary>
    [CreateAssetMenu(fileName = "InventoryWeapon", menuName = "DudeRescueSquad/Items/InventoryWeapon", order = 2)]
    [Serializable]
    public class InventoryWeapon : InventoryItem
    {
        /// The possible auto equip modes
        public enum AutoEquipModes {
            NoAutoEquip,
            AutoEquip,
            AutoEquipIfEmptyHanded
        }

        #region Inspector properties

        [Header("Weapon")]
        [Information("Here you need to bind the weapon you want to equip when picking that item.", InformationAttribute.InformationType.Info, false)]
        public Weapon EquippableWeapon;

        [Tooltip("how to equip this weapon when picked : not equip it, automatically equip it, or only equip it if no weapon is currently equipped")]
        public AutoEquipModes AutoEquipMode = AutoEquipModes.NoAutoEquip;

        #endregion

        #region InventoryItem override implementation

        /// <summary>
        /// When we grab the weapon, we equip it
        /// </summary>
        public override bool Equip()
        {
            EquipWeapon(EquippableWeapon);
            return true;
        }

        /// <summary>
        /// When dropping or unequipping a weapon, we remove it
        /// </summary>
        public override bool UnEquip()
        {
            // if this is a currently equipped weapon, we unequip it
            if (this.TargetEquipmentInventory == null) return false;

            if (this.TargetEquipmentInventory.InventoryContains(this.ItemID).Count > 0)
            {
                EquipWeapon(null);
            }

            return true;
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Grabs the CharacterHandleWeapon component and sets the weapon
        /// </summary>
        /// <param name="newWeapon">New weapon.</param>
        protected virtual void EquipWeapon(Weapon newWeapon)
        {
            if (EquippableWeapon == null) return;

            if (TargetInventory.Owner == null) return;

            CharacterAbilityHandleWeapon characterHandleWeapon = TargetInventory.Owner.GetComponent<CharacterAbilityHandleWeapon>();

            if (characterHandleWeapon != null)
            {
                characterHandleWeapon.ChangeWeapon(newWeapon, this.ItemID);
            }
        }

        #endregion
    }
}