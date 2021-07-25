using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.Inventory.Items.Weapons;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class Character : MonoBehaviour
    {
        #region Private properties

        private IEquipment _equipment = null;
        private ICharacterAbility[] _abilities = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            // Equipment
            _equipment = new CharacterEquipment();

            // Abilities
            _abilities = this.GetComponents<ICharacterAbility>();

            foreach (var ability in _abilities)
            {
                if (!ability.IsEnabled()) continue;

                ability.Initialization();
            }

            Debug.Log($"<color=yellow>Character abilities</color>: {_abilities.Length}");
        }

        private void Update()
        {
            EveryFrame();
        }

        #endregion

        #region Public methods

        public bool HasWeaponEquipped()
        {
            if (!_equipment.HasItemEquipped()) return false;

            var item = _equipment.GetCurrentItem() as WeaponItem;

            return item != null;
        }

        public WeaponItem GetWeaponEquipped()
        {
            return _equipment.GetCurrentItem() as WeaponItem;
        }

        #endregion

        #region Protected methods

        /// <summary>
		/// We do this every frame. This is separate from Update for more flexibility.
		/// </summary>
		protected virtual void EveryFrame()
        {
            // we process our abilities
            EarlyProcessAbilities();
            ProcessAbilities();
            
            // TODO: 
            // LateProcessAbilities();

            // we send our various states to the animator.		 
            // UpdateAnimators();
        }

        /// <summary>
		/// Calls all registered abilities' Early Process methods
		/// </summary>
		protected virtual void EarlyProcessAbilities()
        {
            foreach (var ability in _abilities)
            {
                if (ability.IsEnabled() && ability.WasInitialized())
                {
                    ability.EarlyProcessAbility();
                }
            }
        }

        /// <summary>
		/// Calls all registered abilities' Process methods
		/// </summary>
		protected virtual void ProcessAbilities()
        {
            foreach (var ability in _abilities)
            {
                if (!ability.IsEnabled() || !ability.WasInitialized()) continue;

                ability.Process();
            }
        }

        #endregion
    }
}