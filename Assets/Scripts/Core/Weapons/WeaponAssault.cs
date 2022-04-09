using UnityEngine;
using DudeRescueSquad.Tools;
using DudeRescueSquad.Core.Characters;
using DudeResqueSquad;
using Character = DudeRescueSquad.Core.Characters.Character;
using System.Threading.Tasks;

namespace DudeRescueSquad.Core.Weapons
{
    /// <summary>
    /// This base class, meant to be extended (see ProjectileWeapon.cs for an example of that) handles rate of fire (rate of use actually), and ammo reloading
    /// </summary>
    [SelectionBase]
    public class WeaponAssault : BaseWeapon
    {
        public static System.Action<CustomEventArgs.WeaponStartReloadingEventArgs> OnStartReloading;
        public static System.Action<CustomEventArgs.WeaponStopReloadingEventArgs> OnStopReloading;

        #region Inspector properties

        [SerializeField] private WeaponAssaultData _weaponData = null;

        [Header("IK")]
        [Information("This transform properties will be use if weapons implement IK.", InformationAttribute.InformationType.None, false)]
        [Tooltip("the transform to which the character's left hand should be attached to")]
        [SerializeField] private Transform _leftHandHandle = null;
        [Tooltip("the transform to which the character's right hand should be attached to")]
        [SerializeField] private Transform _rightHandHandle = null;

        [Header("Projectile Data")]
        [SerializeField] private Transform _originProjectile = null;
        [SerializeField] private Transform[] _projectileOrigins = null;

        [Header("Muzzle VFX")]
        [SerializeField] private ParticleSystem _muzzle = default;

        #endregion

        #region Private properties

        private int _currentAmmo = 0;
        private float _expirationTimeLastShot = -1;
        private bool _isReloading = false;
        private Transform _attackerBody;

        #endregion

        #region Private methods

        private void FireProjectile(Transform target = null)
        {
            int amountOfBullets = UnityEngine.Random.Range(_weaponData.MinAmountBulletsPerShot, _weaponData.MaxAmountBulletsPerShot);

            for (int i = 0; i < amountOfBullets; i++)
            {
                SpawnBullet(i);
            }

            /*// Update time to be able to shoot again
            _expirationTimeLastShot = Time.time + _weaponData.FireRate;

            _currentAmmo = Mathf.Max(_currentAmmo - _weaponData.AmmoConsumptionPerShot, 0);

            CheckAmmoForAutoReloading();*/
        }

        private void SpawnBullet(int index)
        {
            if (index >= _projectileOrigins.Length)
            {
                Debug.LogError($"Not enough amount of bullet origins for index: {index}");
                return;
            }

            var origin = _projectileOrigins[index];

            var direction = origin.forward * 10;

            var positionInitial = origin.position;
            var velocity = direction.normalized * _weaponData.ProjectileData.speed;

            DudeResqueSquad.Weapons.ProjectilesContainer.Instance.SpawnSimpleProjectile(_weaponData.ProjectileData, positionInitial, velocity, transform.rotation, _weaponData.CanPushBackOnHit, _characterOwner.transform);
        }

        private void CheckAmmoForAutoReloading()
        {
            if (_currentAmmo > 0) return;

            if (_isReloading) return;

            // TODO: check if it should wait some time to start reloading to connect with the respective animation

            StartReloading();
        }

        private void StartReloading()
        {
            _isReloading = true;

            Debug.Log("<color=orange>START reloading</color>");

            // Communicates through GameEvent that reloading has started
            var evtArgs = new CustomEventArgs.WeaponStartReloadingEventArgs(_weaponData.ReloadingTime);
            OnStartReloading?.Invoke(evtArgs);

            Invoke(nameof(StopReloading), _weaponData.ReloadingTime);
        }

        private void StopReloading()
        {
            AddAmmo(_weaponData.MaxAmmo);

            _isReloading = false;

            Debug.Log($"<color=orange> -- STOP -- reloading</color>. Current Ammo: {_currentAmmo}");

            // Communicates through GameEvent that reloading has finished
            var evtArgs = new CustomEventArgs.WeaponStopReloadingEventArgs();
            OnStopReloading?.Invoke(evtArgs);
        }

        private void AddAmmo(int ammo)
        {
            _currentAmmo = Mathf.Clamp(_currentAmmo + ammo, 0, _weaponData.MaxAmmo);
        }

        private async void ResumeCharacterAfterAttack()
        {
            var delayToResume = 500;

            await Task.Delay(Mathf.FloorToInt(delayToResume));

            //if (_token.Token.IsCancellationRequested) return;

            //Debug.Log($"Weapon '{_displayName}' is resuming character after finishing attack");

            // Update character state
            _characterOwner.StopAction(Enums.CharacterState.ATTACKING);
        }

        private void UpdateAmmoAfterFiring()
        {
            // Update time to be able to shoot again
            _expirationTimeLastShot = Time.time + _weaponData.FireRate;

            _currentAmmo = Mathf.Max(_currentAmmo - _weaponData.AmmoConsumptionPerShot, 0);

            CheckAmmoForAutoReloading();
        }

        #endregion

        #region Protected methods

        /// <summary>
        /// Initialize this weapon.
        /// </summary>
        public override void Initialization(Character characterOwner)
        {
            AddAmmo(_weaponData.InitialAmmo);

            _characterOwner = characterOwner;
            _characterHandleWeapon = _characterOwner.GetComponent<CharacterAbilityHandleWeapon>();

            _attackerBody = _characterOwner.transform;
        }

        /// <summary>
        /// Describes what happens when the weapon starts
        /// </summary>
        protected virtual void TurnWeaponOn()
        {
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Turns the weapon off.
        /// </summary>
        public override void TurnWeaponOff()
        {
        }

        /// <summary>
        /// Determines whether or not the weapon can fire
        /// </summary>
        public virtual void ShootRequest()
        {
        }

        /// <summary>
        /// When the weapon is used, plays the corresponding sound
        /// </summary>
        public virtual void WeaponUse()
        {
        }

        #endregion

        #region IWeapon implementations

        public override IWeaponDefinition WeaponData { get => _weaponData; }

        /// <summary>
        /// Called by input, turns the weapon on
        /// </summary>
        public override void WeaponInputStart()
        {
            if (_weaponData.InstantDamage)
            {
                var detectedTargets = _characterHandleWeapon.WeaponAreaDetection.GetEntitiesOnArea();

                var damage = _weaponData.ProjectileData.damage;
                var canPushBack = _weaponData.CanPushBackOnHit;
                var attackDirection = Vector3.zero;

                for (int i = 0; i < detectedTargets.Length; i++)
                {
                    var target = detectedTargets[i];

                    if (!target.TryGetComponent<IDamageable>(out var targetDamageable)) continue;

                    if (targetDamageable.IsDead) continue;

                    attackDirection = (target.position - _attackerBody.position).normalized;

                    targetDamageable.TakeDamage(damage, canPushBack, attackDirection);
                }
            }
            else
            {
                FireProjectile(_characterHandleWeapon.CurrentTarget);
            }

            // Update ammo after firing
            UpdateAmmoAfterFiring();

            // Show Muzzle
            _muzzle.Play();

            if (_weaponData.AttackOnRelease)
            {
                // Resume movement and rotation when attack has finished
                ResumeCharacterAfterAttack();
            }
        }

        public override bool CanBeUsed()
        {
            // Check if it is reloading
            if (_isReloading) return false;

            // Check if there is remaining ammo
            if (_currentAmmo <= 0)
            {
                Debug.Log("<color=red>No more bullets</color>");
                return false;
            }

            // Check fire rate between shots
            if (_expirationTimeLastShot > -1 && _expirationTimeLastShot > Time.time) return false;

            return true;
        }

        #endregion
    }
}