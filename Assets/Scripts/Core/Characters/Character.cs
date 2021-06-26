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

            Debug.Log($"<color=yellow>Character abilities</color>: {_abilities.Length}");
        }

        private void Update()
        {
            foreach (var ability in _abilities)
            {
                if (!ability.IsEnabled() || !ability.WasInitialized()) continue;

                ability.Process();
            }
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
    }
}