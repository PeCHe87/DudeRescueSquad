using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class PlayerAttackController : MonoBehaviour
    {
        #region Private properties

        private Character _character = null;
        private ICharacterMovement _movement = null;
        private FieldOfView _fov = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _character = GetComponent<Character>();

            if (_character != null)
                _character.OnEquipItem += EquipItem;

            _movement = GetComponent<ICharacterMovement>();

            _fov = GetComponent<FieldOfView>();

            if (_movement == null)
                return;

            _movement.OnDoAction += Attack;
        }

        private void OnDestroy()
        {
            if (_character != null)
                _character.OnEquipItem -= EquipItem;

            if (_movement == null)
                return;

            _movement.OnDoAction -= Attack;
        }

        private void EquipItem()
        {
            // TODO: update values for Field of View based on current weapon configuration
        }

        private void Attack(object sender, EventArgs e)
        {
            if (_character == null)
                return;

            if (_character.Data.CurrentWeaponEquipped == null)
                return;

            // TODO: check weapon equipped to cause damage based on its configuration
        }

        #endregion
    }
}