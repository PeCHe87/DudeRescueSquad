using UnityEngine;

namespace DudeResqueSquad.Inventory
{
    /// <summary>
    /// An inventory item that can be equipped to the player. Weapons could be a subclass of this.
    /// </summary>
    [CreateAssetMenu(menuName = ("Data/Inventory/Equipable Item"))]
    public class EquipableItem : InventoryItem
    {
        #region Inspector properties

        [Tooltip("Where are we allowed to put this item.")]
        [SerializeField] EquipLocation allowedEquipLocation = EquipLocation.Weapon;

        #endregion

        #region Public methods

        public EquipLocation GetAllowedEquipLocation()
        {
            return allowedEquipLocation;
        }

        #endregion
    }
}
