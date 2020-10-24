using System;
using System.ComponentModel;
using UnityEngine;

namespace DudeResqueSquad
{
    public class Entity : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private EntityData _data = null;
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

        #endregion

        #region Public Properties

        public EntityData Data { get => _data; }
        public EntityFollower Follower { get => _follower; }
        public float DistanceToStop { get => _distanceToStop; }
        public Enums.EnemyStates State { get => _state; }

        #endregion

        #region Private methods

        private void Awake()
        {
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
                _follower.Agent.speed = 0;

                //_follower.Stop();

                if (_follower.Target != null)
                    _visuals.StartLookAtTarget(_follower.Target);
            }
            else if (_state == Enums.EnemyStates.PATROLLING)
            {
                _distanceToStop = _data.PatrollingDistanceToStop;
                _follower.Agent.speed = _data.SpeedPatrollingMovement;

                //_follower.ResumeMovement();
            }
            else if (_state == Enums.EnemyStates.CHASING)
            {
                _distanceToStop = _data.ChasingDistanceToStop;
                _follower.Agent.speed = _data.SpeedChasingMovement;

                //_follower.ResumeMovement();
            }

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
            //var stateAttacking = new EntityStateAttacking(this);
            //var stateTakingDamage = new EntityStateTakingDamage(this);
            //var stateDead = new EntityStateDead(this);

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

            #region Any transitions

            // If there is a detected target and not in attacking range then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => _fov.NearestTarget != null && _follower.Target != null && !IsOnChasingRange());

            //_stateMachine.AddAnyTransition(stateIdle, () => stateIdle.RemainingWaitingTime == 0 && IsOnChasingRange());

            #endregion

            /*

            #region Create transitions from "ATTACK" state

            // If is attacking but there isn't any target anymore
            _stateMachine.AddTransition(stateAttacking, stateIdle, () => _fov.NearestTarget == null);

            _stateMachine.AddTransition(stateAttacking, statePatrolling, () => _fov.NearestTarget != null && !stateAttacking.IsOnRange && !IsOnChasingRange());

            // If is attacking but target is in detection area yet
            _stateMachine.AddTransition(stateAttacking, stateChasing, () => _fov.NearestTarget != null && (!stateAttacking.IsOnRange) && IsOnChasingRange());

            #endregion

            #region Create transitions from "TAKING DAMAGE" state

            // Taking damage over -> Patrolling
            _stateMachine.AddTransition(stateTakingDamage, statePatrolling, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget == null);

            // Taking damage over -> Chasing
            _stateMachine.AddTransition(stateTakingDamage, stateChasing, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && !IsOnAttackingRange());

            // Taking damage over -> Attacking
            _stateMachine.AddTransition(stateTakingDamage, stateAttacking, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && IsOnAttackingRange());

            #endregion

            #region Create transitions from ANY state

            // If it is dead
            _stateMachine.AddAnyTransition(stateDead, () => IsDead);

            // If there is a detected target and not in attacking range then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && !IsOnAttackingRange());

            // If it is in range to Attack
            _stateMachine.AddAnyTransition(stateAttacking, () => !IsDead && !stateTakingDamage.IsRecovering && _fov.NearestTarget != null && IsOnAttackingRange());

            // If takes damage then move to this state
            _stateMachine.AddAnyTransition(stateTakingDamage, () => !IsDead && _isTakingDamage);

            #endregion

            // Set last state as "DEAD"
            _stateMachine.SetLastState(stateDead);
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

            if (_follower.Target == null)
                return false;

            var remainingDistance = (_follower.Target.position - _follower.Agent.transform.position).magnitude;

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