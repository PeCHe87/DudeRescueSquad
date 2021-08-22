﻿using UnityEngine;
using DudeRescueSquad.Tools;
using DudeRescueSquad.Core.Characters;
using DudeResqueSquad;
using Character = DudeRescueSquad.Core.Characters.Character;

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

        #endregion

        #region Private properties

        private int _currentAmmo = 0;
        private float _expirationTimeLastShot = -1;
        private bool _isReloading = false;

        #endregion

        #region Private methods

        private void FireProjectile(Transform target = null)
        {
            var direction = transform.forward;

            if (target != null)
            {
                var targetPosition = target.position + Random.insideUnitSphere * _weaponData.ProjectileData.spread;
                direction = (targetPosition - _characterOwner.transform.position).normalized;
            }

            var positionInitial = _originProjectile.position;
            var velocity = direction.normalized * _weaponData.ProjectileData.speed;

            // TODO: Show Muzzle

            // Create projectile based on ammo consumption per shot
            for (int i = 0; i < _weaponData.AmmoConsumptionPerShot; i++)
            {
                // TODO: check if origin/direction should be affected here before creation

                DudeResqueSquad.Weapons.ProjectilesContainer.Instance.SpawnSimpleProjectile(_weaponData.ProjectileData, positionInitial, velocity, transform.rotation);
            }

            // Update time to be able to shoot again
            _expirationTimeLastShot = Time.time + _weaponData.FireRate;

            _currentAmmo = Mathf.Max(_currentAmmo - _weaponData.AmmoConsumptionPerShot, 0);

            CheckAmmoForAutoReloading();
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
            Debug.Log($"<color=green>Shoot weapon</color> '{DisplayName}'");

            FireProjectile(_characterHandleWeapon.CurrentTarget);
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