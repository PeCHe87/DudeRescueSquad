using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeResqueSquad
{
    public class PlayerAttackController : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private FieldOfView _fov = null;

        #endregion

        #region Private properties

        private ItemWeaponData _currentItemEquipped = null;
        private Character _character = null;
        private ICharacterMovement _movement = null;
        private Animator _animator = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _character = GetComponent<Character>();

            if (_character != null)
                _character.OnEquipItem += EquipItem;

            _movement = GetComponent<ICharacterMovement>();

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
            // Update values for Field of View based on current weapon configuration
            if (_fov == null)
                return;

            _currentItemEquipped = _character.Data.CurrentWeaponEquipped;

            if (_currentItemEquipped == null)
            {
                _fov.enabled = false;

                return;
            }

            _fov.Radius = _currentItemEquipped.DetectionAreaRadius;
            _fov.ViewAngle = _currentItemEquipped.AngleAttackArea;
            _fov.enabled = true;
        }

        private void Attack(object sender, EventArgs e)
        {
            if (_fov == null)
                return;

            if (_character == null)
                return;

            if (_currentItemEquipped == null)
                return;

            // Check weapon equipped to cause damage based on its configuration
            _character.State.SetState(Enums.CharacterStates.ATTACKING);

            float delayAttackTime = _currentItemEquipped.AttackDelayTime;

            Invoke("AttackFinished", delayAttackTime);

            StartCoroutine(ApplyDamage());
        }

        private void AttackFinished()
        {
            _character.AttackFinished();
        }

        private IEnumerator ApplyDamage()
        {
            float damage = _currentItemEquipped.Damage;

            // Get all target visibles by weapon configuration and apply damage to all of them in the attack area detection
            Transform[] targets = _fov.VisibleTargets.ToArray();

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                yield return new WaitForSeconds(_currentItemEquipped.DelayToApplyDamage);

                for (int i = 0; i < targetsAmount; i++)
                {
                    IDamageable target = targets[i].GetComponent<IDamageable>();

                    if (target == null)
                        continue;

                    Debug.Log($"Attack - Target: '{targets[i].name}', Health: {target.Health}, Damage: {damage}");

                    target.TakeDamage(damage);
                }

                // TODO: apply weapon usability based on type of weapon: melee (uses) or assault (ammo)
            }
        }

        #endregion
    }
}