using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading;
using System.Threading.Tasks;
using DudeResqueSquad.Weapons;
using UnityEngine;
using Random = UnityEngine.Random;

namespace DudeResqueSquad
{
    /*
     * Source: https://www.youtube.com/watch?v=onpteKMsE84 
     */
    public class EntityAttackAssault : IEntityAttack
    {
        #region Actions

        private System.Action<int> OnInitialized;
        private System.Action<int> OnAttack;
        private System.Action OnStartReloading;
        private System.Action<int> OnStopReloading;

        #endregion

        #region Private properties

        private bool _isReloading = false;
        private float _remainingReloadTime = 0;
        private int _currentBullets = 0;
        private Ray _ray;
        private RaycastHit _hitInfo;
        private readonly List<ProjectileRaycast> _projectiles = new List<ProjectileRaycast>();
        private CancellationToken _taskCancellationToken = default;
        private CancellationTokenSource _token = default;

        #endregion

        #region Unity methods

        protected override void OnDestroy()
        {
            _token.Cancel();
        }

        private void Update()
        {
            if (_entity.IsDead)
            {
                enabled = false;
                return;
            }
            
            if (_weapon == null)
                return;

            if (_isReloading)
            {
                ProcessReloading(Time.deltaTime);

                return;
            }

            // If it isn't idle, chasing or attacking skip possible attack
            if (!IsPossibleAttack())
                return;

            if (IsOnAttackingRange())
                Attack();
        }

        private void ProcessReloading(float deltaTime)
        {
            _remainingReloadTime = Mathf.Clamp(_remainingReloadTime - deltaTime, 0, _weapon.ReloadTime);

            if (_canDebugReloading)
            {
                Debug.Log($"<color=orange>RELOADING</color> - remaining time: {_remainingReloadTime}");
            }

            if (_remainingReloadTime <= 0)
                StopReloading();
        }

        /// <summary>
        /// Check if current state is allowing to continue attacking
        /// </summary>
        /// <returns></returns>
        private bool IsPossibleAttack()
        {
            var state = _entity.State;
            return state == Enums.EnemyStates.CHASING || state == Enums.EnemyStates.ATTACKING || state == Enums.EnemyStates.IDLE;
        }
        
        #endregion

        #region Private methods

        private void StartReloading()
        {
            _remainingReloadTime = _weapon.ReloadTime;

            _isReloading = true;

            _entity.Animations.StopAttack();
            
            _entity.Animations.Reloading(true);

            OnStartReloading?.Invoke();

            if (_canDebugReloading)
            {
                Debug.Log($"Entity <b>{_entity.name}</b> <color=orange>RELOADING</color>, remaining time: {_remainingReloadTime}");
            }
        }

        private void StopReloading()
        {
            _isReloading = false;

            _entity.Animations.Reloading(false);

            _currentBullets = _weapon.BulletsMagazine;

            OnStopReloading?.Invoke(_currentBullets);

            if (_canDebugReloading)
            {
                Debug.Log($"Entity <b>{_entity.name}</b> <color=green>STOP RELOADING</color>");
            }
        }

        private async void FireProjectile()
        {
            // Wait until create the projectile
            await Task.Delay(Mathf.FloorToInt(_weapon.DelayFireEffect * 1000));

            if (_taskCancellationToken.IsCancellationRequested) return;

            // Check if unity is dead
            if (_entity.IsDead)
            {
                return;
            }

            var direction = _entity.transform.forward;
            
            var target = _entity.FieldOfView.NearestTarget;

            if (target != null)
            {
                var targetPosition = target.position + Random.insideUnitSphere * _weapon.projectileSpread;
                direction = targetPosition - _entity.transform.position;
            }

            var positionInitial = _originPojectile.position;
            var velocity = direction.normalized * _weapon.projectileSpeed;
            
            // Show Muzzle
            var muzzle = Instantiate(_weapon.muzzleVFX);

            muzzle.transform.position = positionInitial;
            muzzle.transform.rotation *= transform.rotation;
            
            // Create projectile
            GameEvents.OnSpawnProjectile?.Invoke(this, new CustomEventArgs.SpawnProjectileEventArgs(_weapon.projectileVisualPrefab, positionInitial, velocity, _weapon.projectileDropSpeed, _weapon.projectileLifetime, _weapon.Damage, _entity.UID, _weapon.HitVFX, _targetLayerMask, transform.rotation, false, transform));
        }

        #endregion

        #region IEntityAttack implementation

        protected override void Initialized()
        {
            base.Initialized();

            _token = new CancellationTokenSource();
            _taskCancellationToken = _token.Token;

            if (_weapon == null)
                Debug.LogError($"Entity {_entity.name} doesn't have Weapon information");
            else
                _currentBullets = _weapon.BulletsMagazine;

            OnInitialized?.Invoke(_currentBullets);
        }

        protected override async void Attack()
        {
            // If an attack is in progress then skip attack until it will be available again
            if (_attackStarted)
                return;

            _attackStarted = true;

            // It should instantiate a new projectile each time it can shoot after weapon firing delay time. Use pooling system for performance.
            FireProjectile();

            // Decrease amount of bullets
            _currentBullets--;

            // Trigger shooting entity's animation
            _entity.Animations.Attack(_weapon.AutoFire);

            if (_canDebug)
            {
                Debug.Log($"Entity <b>{_entity.name}</b> attacks to <color=magenta>{_entity.Follower.Target.name}</color>, remaining bullets: {_currentBullets}!");
            }

            OnAttack?.Invoke(_currentBullets);

            // Stop current attack after weapon attack delay based on its fire rate
            //await Task.Delay(Mathf.CeilToInt(_entity.Data.DelayBetweenAttacks * 1000));
            await Task.Delay(Mathf.CeilToInt(_weapon.FireRate * 1000));

            _attackStarted = false;

            // Check if it should start reloading after shooting
            if (_currentBullets <= 0)
                StartReloading();
        }

        protected override void StateMachineHasChanged(object sender, PropertyChangedEventArgs e)
        {
            // If dead then disable this component automatically
            if (_entity.State == Enums.EnemyStates.DEAD)
            {
                this.enabled = false;

                return;
            }

            // TODO: if it is Chasing, check distance to target and check if it is possible attack it
        }

        #endregion
    }
}