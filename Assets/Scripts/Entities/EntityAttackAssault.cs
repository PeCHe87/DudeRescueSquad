using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityAttackAssault : IEntityAttack
    {
        #region Actions

        public System.Action<int> OnInitialized;
        public System.Action<int> OnAttack;
        public System.Action OnStartReloading;
        public System.Action<int> OnStopReloading;

        #endregion

        #region Private properties

        private bool _isReloading = false;
        private float _remainingReloadTime = 0;
        private int _currentBullets = 0;

        #endregion

        #region Unity methods

        private void Update()
        {
            if (_entity.Weapon == null)
                return;

            if (_isReloading)
            {
                _remainingReloadTime = Mathf.Clamp(_remainingReloadTime - Time.deltaTime, 0, _entity.Weapon.ReloadTime);

                if (_remainingReloadTime <= 0)
                    StopReloading();

                return;
            }

            // If it isn't chasing or attacking skip possible attack
            if (_entity.State != Enums.EnemyStates.CHASING && _entity.State != Enums.EnemyStates.ATTACKING)
                return;

            if (IsOnAttackingRange())
                Attack();
        }

        #endregion

        #region Private methods

        private void StartReloading()
        {
            _remainingReloadTime = _entity.Weapon.ReloadTime;

            _isReloading = true;

            _entity.Animations.Reloading(true);

            OnStartReloading?.Invoke();

            Debug.Log($"Entity <b>{_entity.name}</b> <color=orange>RELOADING</color>, remaining time: {_remainingReloadTime}");
        }

        private void StopReloading()
        {
            _isReloading = false;

            _entity.Animations.Reloading(false);

            _currentBullets = _entity.Weapon.BulletsMagazine;

            OnStopReloading?.Invoke(_currentBullets);

            Debug.Log($"Entity <b>{_entity.name}</b> <color=green>STOP RELOADING</color>");
        }

        private void FireProjectile()
        {
            // TODO:
        }

        #endregion

        #region IEntityAttack implementation

        protected override void Initialized()
        {
            base.Initialized();

            if (_entity.Weapon == null)
                Debug.LogError($"Entity {_entity.name} doesn't have Weapon information");
            else
                _currentBullets = _entity.Weapon.BulletsMagazine;

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
            _entity.Animations.Attack(_entity.Weapon.AutoFire);

            Debug.Log($"Entity <b>{_entity.name}</b> attacks to <color=magenta>{_entity.Follower.Target.name}</color>, remaining bullets: {_currentBullets}!");

            OnAttack?.Invoke(_currentBullets);

            // Stop current attack after weapon attack delay based on its fire rate
            await Task.Delay(Mathf.CeilToInt(_entity.Data.DelayBetweenAttacks * 1000));

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