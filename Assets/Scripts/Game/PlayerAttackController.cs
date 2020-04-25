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
        private int _currentAmountAttacks = 0;
        private int _currentAttack = 0;
        private bool _attackInProgress = false;
        private List<IDamageable> _cacheTargets = null;

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

            _cacheTargets = new List<IDamageable>();
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

            if (_character.State.CurrentState == Enums.CharacterStates.ATTACKING)
                _currentAmountAttacks++;

            if (_currentAttack > 0)
                Debug.Log($"Attack! Current Attack: {_currentAttack}, total amount: {_currentAmountAttacks}");

            if (_attackInProgress)
                return;

            Debug.Log("<color=green>Attack!</color>");

            // Check weapon equipped to cause damage based on its configuration
            _character.State.SetState(Enums.CharacterStates.ATTACKING);

            _attackInProgress = true;

            _currentAttack = 1;

            _currentAmountAttacks = 1;

            float delayAttackTime = _currentItemEquipped.AttackDelayTime;

            Invoke("AttackFinished", delayAttackTime);

            StartCoroutine(ApplyDamage());
        }

        private void AttackFinished()
        {
            // If character start moving before attack was finished then skip all remaining attacks
            if (_character.State.CurrentState != Enums.CharacterStates.ATTACKING)
            {
                ResetAttackState();

                return;
            }
            else if (_currentAttack < _currentAmountAttacks) // Checks if amount of attacks is enough
            {
                ComboAttack();

                return;
            }

            ResetAttackState();
        }

        private void ResetAttackState()
        {
            _attackInProgress = false;

            _currentAmountAttacks = 0;

            _currentAttack = 0;

            _cacheTargets.Clear();

            _character.AttackFinished();
        }

        private void ComboAttack()
        {
            _character.State.SetState(Enums.CharacterStates.ATTACKING);

            _currentAttack++;

            Debug.Log($"<b>Attack</b> {_currentAttack} of {_currentAmountAttacks}");

            float delayAttackTime = _currentItemEquipped.AttackDelayTime + _currentItemEquipped.ComboDelayTime;

            Invoke("AttackFinished", delayAttackTime);

            StartCoroutine(ApplyDamageToCache());
        }

        private IEnumerator ApplyDamage()
        {
            float damage = _currentItemEquipped.Damage;

            // Get all target visibles by weapon configuration and apply damage to all of them in the attack area detection
            Transform[] targets = _fov.VisibleTargets.ToArray();

            _cacheTargets.Clear();

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                yield return new WaitForSeconds(_currentItemEquipped.DelayToApplyDamage);

                for (int i = 0; i < targetsAmount; i++)
                {
                    IDamageable target = targets[i].GetComponent<IDamageable>();

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    Debug.Log($"Attack - Target: '{targets[i].name}', Health: {target.Health}, Damage: {damage}");

                    target.TakeDamage(damage);

                    _cacheTargets.Add(target);
                }

                // TODO: apply weapon usability based on type of weapon: melee (uses) or assault (ammo)
            }
        }

        private IEnumerator ApplyDamageToCache()
        {
            float damage = _currentItemEquipped.Damage;

            // Get all target visibles by weapon configuration and apply damage to all of them in the attack area detection
            IDamageable[] targets = _cacheTargets.ToArray();

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                yield return new WaitForSeconds(_currentItemEquipped.DelayToApplyDamage + _currentItemEquipped.ComboDelayTime);

                for (int i = 0; i < targetsAmount; i++)
                {
                    var target = targets[i];

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    Debug.Log($"<color=red>Combo Attack</color> - Health: {target.Health}, Damage: {damage}");

                    target.TakeDamage(damage);
                }

                // TODO: apply weapon usability based on type of weapon: melee (uses) or assault (ammo)

                // TODO: Check what happen if it is broken in the middle of combo, it should be stopped
            }
        }

        #endregion
    }
}