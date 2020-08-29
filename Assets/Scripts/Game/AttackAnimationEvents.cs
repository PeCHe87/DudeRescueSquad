using UnityEngine;

namespace DudeResqueSquad
{
    public class AttackAnimationEvents : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Character _character = null;

        #endregion

        #region Private properties

        private PlayerAttackController _attackController = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _attackController = _character.GetComponent<PlayerAttackController>();
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Method invoked from key frame on RigAnimation from specific attack animation
        /// </summary>
        public void AttackHit()
        {
            ItemWeaponData currentWeapon = _character.Data.CurrentWeaponEquipped;

            Debug.Log($"<b>AttackHit</b> - Character: '{_character.Data.UID}', current weapon: <b>{((currentWeapon != null) ? currentWeapon.UID : "No Weapon")}</b>");

            if (currentWeapon != null)
            {
                if (currentWeapon.AttackType == Enums.WeaponAttackType.MELEE_1_HAND || currentWeapon.AttackType == Enums.WeaponAttackType.MELEE_2_HANDS)
                {
                    _attackController.ApplyMelee();
                }
                else if (currentWeapon.AttackType == Enums.WeaponAttackType.ASSAULT_1_HAND || 
                         currentWeapon.AttackType == Enums.WeaponAttackType.ASSAULT_2_HANDS ||
                         currentWeapon.AttackType == Enums.WeaponAttackType.ASSAULT_RIFLE)
                {
                    _attackController.ApplyShooting();
                }
            }
        }

        #endregion
    }
}