using UnityEngine;
using DudeRescueSquad.Tools;
using System;
using DudeRescueSquad.InventoryEngine;

namespace DudeRescueSquad.Core
{
    /// <summary>
    /// Collectable resource item in the Game. Like: coins or other resources
    /// </summary>
    [CreateAssetMenu(fileName = "InventoryResource", menuName = "DudeRescueSquad/Items/InventoryResource", order = 3)]
    [Serializable]
    public class InventoryResource : InventoryItem
    {
        #region Inspector properties

        [Header("Resource")]
        [Information("Here you need to bind the Resource you want to equip when picking that item.", InformationAttribute.InformationType.Info, false)]
        [SerializeField] private ResourceData _resource = null;

        #endregion

        /// <summary>
        /// What happens when the resource is picked
        /// </summary>
        public override bool Pick()
        {
            // TODO: increase the amount of this item of player profile resource amount

            return true;
        }
    }
}