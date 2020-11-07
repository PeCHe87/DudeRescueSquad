using System;
using System.ComponentModel;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Entity : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private EntityData _data = null;
        [SerializeField] private string _uid = string.Empty;
        [SerializeField] private Enums.EnemyStates _state;
        [SerializeField] private Transform[] _patrollingPoints = null;

        #endregion

        #region Private properties

        private EntityAnimations _animations = null;
        private EntityVisual _visuals = null;
        private EntityFollower _follower = null;
        private FieldOfView _fov = null;
        private float _health = 0;
        private StateMachine _stateMachine = null;
        private float _distanceToStop = 0;
        private IDamageable _damageable = null;

        #endregion

        #region Public Properties

        public string UID { get => _uid; }
        public EntityData Data { get => _data; }
        public EntityFollower Follower { get => _follower; }
        public float DistanceToStop { get => _distanceToStop; }
        public Enums.EnemyStates State { get => _state; }
        public EntityVisual Visuals { get => _visuals; }

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
        }

        private void Update()
        {
            if (_stateMachine == null)
                return;

            _stateMachine.Tick();
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

            // TODO: make changes related with animations

            // Stop visual following
            _visuals.StopFollowing();

            // Stop movement
            _follower.StopSpeed();

            // Re positionate the Agent and Obstacle in the same place where visual stops
            var position = _visuals.transform.position;
            _follower.Agent.transform.position = position;
            _follower.Obstacle.transform.position = position;
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

            _animations.ProcessUpdate(_state);

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
                //if (oldState == Enums.EnemyStates.TAKING_DAMAGE)
                //    _follower.Agent.transform.position = _visuals.transform.position;

                _visuals.ResumeFollowing();

                _distanceToStop = _data.PatrollingDistanceToStop;
                _follower.Agent.speed = _data.SpeedPatrollingMovement;
            }
            else if (_state == Enums.EnemyStates.CHASING)
            {
                //if (oldState == Enums.EnemyStates.TAKING_DAMAGE)
                //    _follower.Agent.transform.position = _visuals.transform.position;

                _visuals.ResumeFollowing();

                _distanceToStop = _data.ChasingDistanceToStop;
                _follower.Agent.speed = _data.SpeedChasingMovement;
            }
            else if (_state == Enums.EnemyStates.TAKING_DAMAGE)
            {
                _follower.StopSpeed();
            }
            else if (_state == Enums.EnemyStates.DEAD)
            {
                _follower.StopSpeed();
            }

            // No need to look at Target if it isn't in Idle state
            if (_state != Enums.EnemyStates.IDLE)
                _visuals.StopLookAtTarget();
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
            //var stateAttacking = new EntityStateAttacking(this);

            #region Create transitions from "IDLE" state

            _stateMachine.AddTransition(stateIdle, statePatrolling, () => stateIdle.RemainingWaitingTime == 0 && !IsOnChasingRange());

            #endregion

            #region Create transitions from "PATROLLING" state

            _stateMachine.AddTransition(statePatrolling, stateIdle, () => (statePatrolling.IsWaiting || statePatrolling.RemainingTime <= 0));

            #endregion

            #region Create transitions from "CHASE TARGET" state

            // If it isn't chasing and not in attack range or there isn't target any more
            //_stateMachine.AddTransition(stateChasing, stateIdle, () => (!stateChasing.IsChasing && !stateAttacking.IsOnRange) || _fov.NearestTarget == null);

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

            #region Any transitions

            // If there is a detected target and not in attacking range then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => !_damageable.IsDead && !_damageable.IsTakingDamage && _fov.NearestTarget != null && _follower.Target != null && !IsOnChasingRange());

            // If takes damage then move to this state
            _stateMachine.AddAnyTransition(stateTakingDamage, () => !_damageable.IsDead && _damageable.IsTakingDamage);

            // If it is dead
            _stateMachine.AddAnyTransition(stateDead, () => _damageable.IsDead);

            #endregion

            // Set last state as "DEAD"
            _stateMachine.SetLastState(stateDead);

            /*
            #region Create transitions from "ATTACK" state

            // If is attacking but there isn't any target anymore
            _stateMachine.AddTransition(stateAttacking, stateIdle, () => _fov.NearestTarget == null);

            _stateMachine.AddTransition(stateAttacking, statePatrolling, () => _fov.NearestTarget != null && !stateAttacking.IsOnRange && !IsOnChasingRange());

            // If is attacking but target is in detection area yet
            _stateMachine.AddTransition(stateAttacking, stateChasing, () => _fov.NearestTarget != null && (!stateAttacking.IsOnRange) && IsOnChasingRange());

            #endregion

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
        }

        public void InitMovement(EntityFollower followerTemplate)
        {
            _follower = Instantiate(followerTemplate);
            _follower.name = $"{this.name}_follower";
            _follower.transform.SetParent(this.transform.parent);
            _follower.transform.position = transform.position;
            _follower.Config(_data);

            var visualBehavior = GetComponent<EntityVisual>();

            if (visualBehavior != null)
            {
                visualBehavior.Target = _follower.transform;
                visualBehavior.Agent = _follower.Agent;
            }
        }

        #endregion
    }
}