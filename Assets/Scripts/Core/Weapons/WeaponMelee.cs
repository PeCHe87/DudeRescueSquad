using DudeRescueSquad.Core.Characters;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    /// <summary>
    /// Represents a melee weapon that is used to apply damage on near targets inside its field of view and distance attack range
    /// </summary>
    public class WeaponMelee : BaseWeapon
    {
        #region Inspector properties

        [Header("Weapon Data")]
        [SerializeField] private WeaponMeleeData _weaponData = null;
        [SerializeField] private bool _canDebug = true;
        [SerializeField] private float _damage = 0;
        [SerializeField] private float _attackRange = 0;
        [SerializeField] private float _attackDuration = 0;

        #endregion

        #region Private properties

        private CharacterAbilityMovement _characterMovement = null;
        private CharacterAbilityOrientation _characterOrientation = null;
        private CancellationTokenSource _token = null;
        private bool _isUsing = false;
        private Transform _characterTransform = null;

        #endregion

        #region Unity methods

        private void OnDestroy()
        {
            if (_token != null)
            {
                _token.Cancel();
            }
        }

        #endregion

        #region BaseWeapon Implementation

        public override IWeaponDefinition WeaponData { get => _weaponData; }

        public override void WeaponInputStart()
        {
            // TODO: check if it can be used based on time between uses
            if (_isUsing) return;

            Debug.Log($"<color=green>Use weapon</color> '{_displayName}'");

            _isUsing = true;

            if (!_weaponData.CanMoveWhileAttacking)
            {
                // Cancel movement and rotation when melee attack is applied
                _characterMovement.Disable();
                _characterOrientation.Disable();
            }

            // Apply damage to all targets in the angle detection area
            ApplyDamage();

            // Resume movement and rotation when melee attack has finished
            ResumeCharacterAfterAttack();
        }

        public override void Initialization(Character characterOwner)
        {
            _characterOwner = characterOwner;
            _characterTransform = characterOwner.transform;

            _characterHandleWeapon = _characterOwner.GetComponent<CharacterAbilityHandleWeapon>();
            _characterMovement = _characterOwner.GetComponent<CharacterAbilityMovement>();
            _characterOrientation = _characterOwner.GetComponent<CharacterAbilityOrientation>();

            _token = new CancellationTokenSource();
        }

        public override void TurnWeaponOff() { }

        public override bool CanBeUsed()
        {
            return true;
        }

        #endregion

        #region Private methods

        private void ApplyDamage()
        {
            // Get all visible targets by weapon configuration and apply damage to all of them in the attack area detection
            Transform[] targets = _characterHandleWeapon.VisibleTargets;

            int targetsAmount = targets.Length;

            if (targetsAmount > 0)
            {
                var characterPosition = _characterOwner.transform.position;

                for (int i = 0; i < targetsAmount; i++)
                {
                    DudeResqueSquad.IDamageable target = targets[i].GetComponent<DudeResqueSquad.IDamageable>();

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    // Check if current target is in the attack range
                    var targetPosition = targets[i].position;

                    if (!InAttackRange(characterPosition, targetPosition, out var distance)) continue;

                    if (_canDebug)
                        Debug.Log($"Attack - Target: '{targets[i].name}', Health: {target.Health}, Damage: {_weaponData.AttackDamage}, distance: {distance}");

                    // Apply damage
                    target.TakeDamage(_weaponData.AttackDamage);
                }
            }
        }

        private bool InAttackRange(Vector3 characterPosition, Vector3 targetPosition, out float distance)
        {
            var dirTarget = (targetPosition - characterPosition);

            distance = dirTarget.magnitude;

            Debug.DrawRay(characterPosition, dirTarget.normalized * _weaponData.AttackRange, Color.blue, _weaponData.AttackDuration);

            return (distance <= _weaponData.AttackRange);
        }

        private async void ResumeCharacterAfterAttack()
        {
            await Task.Delay(Mathf.FloorToInt(1000 * _weaponData.AttackDuration));

            if (_token.Token.IsCancellationRequested) return;

            Debug.Log($"Weapon '{_displayName}' is resuming character after finishing attack");

            _characterMovement.Enable();
            _characterOrientation.Enable();

            _isUsing = false;
        }

        #endregion

        private void OnDrawGizmos()
        {
            if (_characterTransform == null) return;

            Gizmos.color = Color.blue;
            Gizmos.DrawWireSphere(_characterTransform.position + Vector3.up * 0.5f, _weaponData.AttackRange);
        }
    }
}