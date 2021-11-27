using System;
using System.ComponentModel;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Entity : MonoBehaviour
    {
        public Action OnInitialized; 

        #region Inspector properties

        [SerializeField] private EntityData _data = null;
        [SerializeField] private string _uid = string.Empty;
        [SerializeField] private Enums.EnemyStates _state;
        [SerializeField] private Transform[] _patrollingPoints = null;
        [SerializeField] private ItemWeaponData _weapon = null;
        [SerializeField] private bool _stopMovementTakingDaamge = false;

        #endregion

        #region Private properties

        private EntityAnimations _animations = null;
        private EntityVisual _visuals = null;
        private EntityFollower _follower = null;
        private FieldOfView _fov = null;
        private float _health = 0;
        private StateMachine _stateMachine = null;
        //private float _distanceToStop = 0;
        private IDamageable _damageable = null;
        private bool _checkWhenAgentEnabled = false;
        [SerializeReference]
        private bool _knockbackInProgress = false;
        private Rigidbody _rigidBody = default;

        #endregion

        #region Public Properties

        public string UID { get => _uid; }
        public EntityData Data { get => _data; }
        public EntityFollower Follower { get => _follower; }
        public Enums.EnemyStates State { get => _state; }
        public EntityVisual Visuals { get => _visuals; }
        public StateMachine StateMachine { get => _stateMachine; }
        public FieldOfView FieldOfView { get => _fov; }
        public EntityAnimations Animations { get => _animations; }
        public ItemWeaponData Weapon { get => _weapon; }
        public bool IsDead { get => _damageable.IsDead; }
        public bool KnockBackInProgress => _knockbackInProgress;

        #endregion

        #region Private methods

        private void Awake()
        {
            // Damageable
            _damageable = GetComponent<IDamageable>();

            if (_damageable != null)
            {
                ((EntityDamageable)_damageable).Init(_data.CurrentHealth, _data.MaxHealth, _uid, _data.TimeForRecoveringAfterDamage);

                _damageable.OnTakeDamage += TakingDamage;
                _damageable.OnDied += Dying;
            }

            _rigidBody = GetComponent<Rigidbody>();

            DisableRigidBody();

            // Animations
            _animations = GetComponent<EntityAnimations>();

            // Visuals
            _visuals = GetComponent<EntityVisual>();

            // Field of View events subscription
            _fov = GetComponent<FieldOfView>();

            if (_fov != null)
            {
                _fov.OnDetectNewTarget += DetectNewTarget;
                _fov.OnStopDetecting += StopDetection;
            }
        }

        private void Start()
        {
            Init();
        }

        private void OnDestroy()
        {
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= TakingDamage;
                _damageable.OnDied -= Dying;
            }

            if (_fov != null)
            {
                _fov.OnDetectNewTarget -= DetectNewTarget;
                _fov.OnStopDetecting -= StopDetection;
            }

            if (_visuals != null)
            {
                _visuals.OnFollowingStateChanged -= VisualMovementStateChanged;
            }

            if (_follower != null)
            {
                _follower.OnAgentStateChanged -= FollowerMovementStateChanged;
            }

            // Teardown Knockback component
            if (TryGetComponent<EntityKnockback>(out var knockback))
            {
                knockback.Teardown();
            }
        }

        private void Update()
        {
            if (_stateMachine == null) return;

            if (_knockbackInProgress) return;

            _stateMachine.Tick();

            // Check if it should detect when agent is enabled so it can process its current visual animation state
            if (_checkWhenAgentEnabled)
            {
                if (_follower.Agent.enabled)
                {
                    _checkWhenAgentEnabled = false;
                    _animations.ProcessUpdate(_state);
                }
            }
        }

        private void DetectNewTarget(Transform target)
        {
            if (IsOnChasingRange())
                return;

            _follower.SetTarget(target);
        }

        private void StopDetection()
        {
            if (_state == Enums.EnemyStates.PATROLLING)
                return;

            _follower.SetTarget(null);
        }

        private void TakingDamage(object sender, CustomEventArgs.DamageEventArgs e)
        {
            Debug.Log($"Entity: <b>{e.entityUID}</b> <color=red>takes damage: {e.damage}</color>, current Health: {_damageable.Health}/{_damageable.MaxHealth}");

            if (_stopMovementTakingDaamge)
            {
                // Stop visual following
                _visuals.StopFollowing();

                // Stop movement
                _follower.StopSpeed();

                // Re positionate the Agent and Obstacle in the same place where visual stops
                var position = _visuals.transform.position;
                _follower.Agent.transform.position = position;
                _follower.Obstacle.transform.position = position;
            }
        }

        private void Dying(object sender, CustomEventArgs.EntityDeadEventArgs e)
        {
            Debug.Log($"Entity: <b>{e.entityUID}</b> <color=red>has DEAD!</color>");

            // Stop movement
            _follower.enabled = false;

            // Stop detection
            _fov.enabled = false;

            // Stop visual following
            _visuals.StopFollowing();
        }

        /// <summary>
        /// Each time State Machine is updated then it is notified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateMachineHasChanged(object sender, PropertyChangedEventArgs e)
        {
            var oldState = _state;

            _state = _stateMachine.GetCurrentState();

            _animations.Aiming(_state == Enums.EnemyStates.CHASING);

            if (_follower == null)
                return;

            if (_state == Enums.EnemyStates.IDLE)
            {
                _follower.StopSpeed();

                // If there is a target, then look at it
                if (_follower.Target != null)
                    _visuals.StartLookAtTarget(_follower.Target);
            }
            else if (_state == Enums.EnemyStates.PATROLLING)
            {
                _visuals.ResumeFollowing();
                
                _follower.Agent.speed = _data.SpeedPatrollingMovement;
            }
            else if (_state == Enums.EnemyStates.CHASING)
            {
                _visuals.ResumeFollowing();
                
                _follower.Agent.speed = _data.SpeedChasingMovement;
            }
            /*else if (_state == Enums.EnemyStates.TAKING_DAMAGE)
            {
                _follower.StopSpeed();
            }*/
            else if (_state == Enums.EnemyStates.DEAD)
            {
                _follower.StopSpeed();
            }
            else if (_state == Enums.EnemyStates.ATTACKING)
            {
                // If there is a target, then look at it
                if (_follower.Target != null)
                    _visuals.StartLookAtTarget(_follower.Target);
            }

            // No need to look at Target if it isn't in Idle or Attacking state
            if (_state != Enums.EnemyStates.IDLE && _state != Enums.EnemyStates.ATTACKING)
                _visuals.StopLookAtTarget();

            // Check if it is going from stand to moving state
            CheckStateChangeFromStationaryToMoving(oldState);
        }

        /// <summary>
        /// Checks if entity is changing from stationary state to a movable state, considering if Agent is enabled or not
        /// </summary>
        /// <param name="oldState"></param>
        private void CheckStateChangeFromStationaryToMoving(Enums.EnemyStates oldState)
        {
            if ((oldState == Enums.EnemyStates.IDLE || oldState == Enums.EnemyStates.ATTACKING) && (_state == Enums.EnemyStates.CHASING || _state == Enums.EnemyStates.PATROLLING))
            {
                if (!_follower.Agent.enabled)
                {
                    _checkWhenAgentEnabled = true;
                    return;
                }
            }

            _checkWhenAgentEnabled = false;
            _animations.ProcessUpdate(_state);
        }

        private void InitStateMachine()
        {
            // Create State Machine
            _stateMachine = new StateMachine();
            _stateMachine.PropertyChanged += StateMachineHasChanged;

            // Create states
            var stateIdle = new EntityStateIdle(this);
            var statePatrolling = new EntityStatePatrolling(this, _patrollingPoints);
            var stateChasing = new EntityStateChasing(this);
            var stateTakingDamage = new EntityStateTakingDamage(this);
            var stateDead = new EntityStateDead(this);
            var stateAttacking = new EntityStateAttacking(this);

            #region Create transitions from "IDLE" state

            _stateMachine.AddTransition(stateIdle, statePatrolling, () => stateIdle.RemainingWaitingTime == 0 && !IsOnChasingRange());

            #endregion

            #region Create transitions from "PATROLLING" state

            _stateMachine.AddTransition(statePatrolling, stateIdle, () => (statePatrolling.IsWaiting || statePatrolling.RemainingTime <= 0));

            #endregion

            #region Create transitions from "CHASE TARGET" state
            
            _stateMachine.AddTransition(stateChasing, statePatrolling, () => _fov.NearestTarget == null);

            _stateMachine.AddTransition(stateChasing, stateIdle, () => !stateChasing.IsChasing);

            #endregion

            #region Create transitions from "TAKING DAMAGE" state

            // Taking damage over -> Patrolling
            //_stateMachine.AddTransition(stateTakingDamage, statePatrolling, () => !_damageable.IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget == null && !IsOnChasingRange());

            // Taking damage over -> Chasing
            _stateMachine.AddTransition(stateTakingDamage, stateChasing, () => !_damageable.IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && IsOnChasingRange());

            // Taking damage over -> Idle
            _stateMachine.AddTransition(stateTakingDamage, stateIdle, () => !_damageable.IsDead && !stateTakingDamage.IsRecovering && !IsOnChasingRange());

            // Taking damage over -> Attacking
            //_stateMachine.AddTransition(stateTakingDamage, stateAttacking, () => !_damageable.IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && IsOnAttackingRange());

            #endregion

            #region Create transitions from "ATTACK" state

            // If is attacking but there isn't any target anymore
            _stateMachine.AddTransition(stateAttacking, stateIdle, () => (_fov.NearestTarget == null || (!IsOnAttackingRange() && !IsOnChasingRange())));

            // If it is attacking but target is in detection area yet continue chasing it
            _stateMachine.AddTransition(stateAttacking, stateChasing, () => _fov.NearestTarget != null && !IsOnAttackingRange() && IsOnChasingRange());

            #endregion
            
            #region Any transitions

            // If there is a detected target and not in attacking range then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => !_damageable.IsDead && !_damageable.IsTakingDamage && _fov.NearestTarget != null && _follower.Target != null && !IsOnAttackingRange());

            // If there is a detected target and it is in attacking range then start Attacking it
            _stateMachine.AddAnyTransition(stateAttacking, () => !_damageable.IsDead && !_damageable.IsTakingDamage && _fov.NearestTarget != null  && IsOnAttackingRange());
            
            // If takes damage then move to this state
            _stateMachine.AddAnyTransition(stateTakingDamage, () => !_damageable.IsDead && _damageable.IsTakingDamage);

            // If it is dead
            _stateMachine.AddAnyTransition(stateDead, () => _damageable.IsDead);

            #endregion

            // Set last state as "DEAD"
            _stateMachine.SetLastState(stateDead);
            
            /*
            #region Create transitions from ANY state

            // If there is a detected target and not in attacking range then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && !IsOnAttackingRange());

            // If it is in range to Attack
            _stateMachine.AddAnyTransition(stateAttacking, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && IsOnAttackingRange());

            #endregion
            */

            // Start State machine at "IDLE" state
            _stateMachine.SetState(stateIdle);
        }

        /// <summary>
        /// Checks if entity is near enough to its target when chasing
        /// </summary>
        private bool IsOnChasingRange()
        {
            if (_follower == null)
                return false;

            var target = _fov.NearestTarget;

            if (target == null)
                return false;

            var remainingDistance = (target.position - _follower.Agent.transform.position).magnitude;

            return (remainingDistance <= _data.ChasingDistanceToStop);
        }

        /// <summary>
        /// Check if entity is near enough to its target to attack it
        /// </summary>
        /// <returns></returns>
        private bool IsOnAttackingRange()
        {
            if (_follower == null)
                return false;

            var target = _fov.NearestTarget;

            if (target == null)
                return false;

            var remainingDistance = (target.position - _follower.Agent.transform.position).magnitude;

            return (remainingDistance <= _weapon.AttackAreaRadius);
        }

        private void VisualMovementStateChanged(float deltaMovement)
        {
            if (deltaMovement == 0)
            {
                _animations.Idle();
            }
            else
            {
                if (_state == Enums.EnemyStates.CHASING)
                {
                    _animations.Run();
                }
                else if (_state == Enums.EnemyStates.PATROLLING)
                {
                    _animations.Walk();
                }
            }
        }
        
        private void FollowerMovementStateChanged(bool isFollowing)
        {
            /*if (isFollowing)
            {
                if (_state == Enums.EnemyStates.CHASING)
                {
                    _animations.Run();
                }
                else if (_state == Enums.EnemyStates.PATROLLING)
                {
                    _animations.Walk();
                }
            }*/
        }
        
        private void DisableRigidBody()
        {
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            _rigidBody.isKinematic = true;
        }

        #endregion

        #region Public Method

        public void Init()
        {
            _health = Mathf.Clamp(_data.CurrentHealth, 0, _data.MaxHealth);

            if (_fov != null)
            {
                _fov.Radius = _data.RadiusDetection;
                _fov.ViewAngle = _data.AngleDetection;
                _fov.TargetMask = _data.TargetMaskDetection;
                _fov.ObstacleMask = _data.ObstacleMaskDetection;
            }

            InitStateMachine();

            // Initializes the Knockback component
            if (TryGetComponent<EntityKnockback>(out var knockback))
            {
                knockback.Init(this, _damageable, _rigidBody);
            }

            OnInitialized?.Invoke();
        }

        public void InitMovement(EntityFollower followerTemplate)
        {
            _follower = Instantiate(followerTemplate);
            _follower.name = $"{this.name}_follower";

            var followerTransform = _follower.transform;
            followerTransform.SetParent(this.transform.parent);
            followerTransform.position = transform.position;
            
            _follower.Config(_data);
            _follower.OnAgentStateChanged += FollowerMovementStateChanged;

            if (_visuals != null)
            {
                _visuals.Target = _follower.transform;
                _visuals.Agent = _follower.Agent;
                
                _visuals.OnFollowingStateChanged += VisualMovementStateChanged;
            }
        }

        public void StartKnockBack()
        {
            _knockbackInProgress = true;

            //_follower.Agent.enabled = false;
            //_follower.SetAgentEnabledState(false);

            //_follower.Obstacle.enabled = false;
        }

        public void StopKnockBack()
        {
            DisableRigidBody();

            // Move follower to the current position of the entity
            //_follower.transform.position = transform.position;

            _knockbackInProgress = false;
        }

        #endregion
    }
}