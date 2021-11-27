using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityAttackMelee : IEntityAttack
    {
        #region Actions

        private Action OnInitialized;
        private Action OnStartAttack;
        private Action OnProcessAttack;

        #endregion

        #region Private properties
        
        private List<IDamageable> _cacheTargets = null;
        private FieldOfView _fov = null;
        private EntityAnimationEvents _animationEvents = null;

        #endregion

        #region Unity methods

        private void Update()
        {
            // TODO: this coding block could be replaced by an event to detect when it is near the target and avoid do it on each frame
            
            if (_entity.IsDead)
            {
                enabled = false;
                return;
            }
            
            if (_weapon == null)
                return;

            // If it isn't idle, chasing or attacking skip possible attack
            if (!IsPossibleAttack())
                return;

            if (IsOnAttackingRange())
                Attack();
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

        protected override void OnDestroy()
        {
            base.OnDestroy();
            
            if (_animationEvents != null)
            {
                _animationEvents.OnProcessAttack -= ProcessAttack;
            }
        }

        private void ProcessAttack()
        {
            if (_entity.IsDead)
                return;
            
            float damage = _weapon.Damage;
            
            // Get all visible targets by weapon configuration and apply damage to all of them in the attack area detection
            Transform[] targets = _fov.VisibleTargets.ToArray();

            _cacheTargets.Clear();

            int targetsAmount = targets.Length;

            var entityPosition = _entity.transform.position;
            
            if (targetsAmount > 0)
            {
                for (int i = 0; i < targetsAmount; i++)
                {
                    var targetTransform = targets[i];
                    
                    IDamageable target = targetTransform.GetComponent<IDamageable>();

                    if (target == null)
                        continue;

                    if (target.IsDead)
                        continue;

                    Entity entity = targetTransform.GetComponent<Entity>();

                    // Avoid another entities
                    if (entity != null)
                        continue;

                    // If current distance is on attack range
                    var distance = (entityPosition - targetTransform.position).magnitude;

                    if (distance > _weapon.AttackAreaRadius)
                        continue;
                    
                    if (_canDebug)
                        Debug.Log($"Attack - Target: '{targets[i].name}', Health: {target.Health}, Damage: {damage}");

                    target.TakeDamage(damage, false, Vector3.zero);

                    _cacheTargets.Add(target);
                }

                OnProcessAttack?.Invoke();
            }
        }

        #endregion

        #region IEntityAttack implementation

        protected override void Initialized()
        {
            base.Initialized();

            if (_weapon == null)
                Debug.LogError($"Entity {_entity.name} doesn't have Weapon information");
            
            _cacheTargets = new List<IDamageable>();

            _fov = GetComponent<FieldOfView>();
            
            if (_fov == null)
                Debug.LogError($"Entity {_entity.name} doesn't have Field of View component assigned");

            _animationEvents = GetComponentInChildren<EntityAnimationEvents>();

            if (_animationEvents != null)
            {
                _animationEvents.OnProcessAttack += ProcessAttack;
            }

            OnInitialized?.Invoke();
        }

        protected override async void Attack()
        {
            // If an attack is in progress then skip attack until it will be available again
            if (_attackStarted)
                return;

            _attackStarted = true;

            // Trigger melee attack entity's animation
            _entity.Animations.Attack(false);

            if (_canDebug)
            {
                Debug.Log(
                    $"Entity <b>{_entity.name}</b> attacks to <color=magenta>{_entity.Follower.Target.name}</color>");
            }
            
            OnStartAttack?.Invoke();

            // Stop current attack after weapon attack delay based on its fire rate
            await Task.Delay(Mathf.CeilToInt(_weapon.FireRate * 1000));

            _attackStarted = false;
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