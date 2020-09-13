using System;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(FieldOfView))]
    public class EnemyPatroller : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private EnemyData _data = null;
        [SerializeField] private Transform[] _points = null;
        [SerializeField] private Animator _animator = null;

        #endregion

        #region Private properties

        private StateMachine _stateMachine = null;
        private bool _isDead = false;       // TODO: should be replaced with DamageableComponent

        #endregion

        #region Private methods

        private void Awake()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();
            FieldOfView fieldOfView = GetComponent<FieldOfView>();

            fieldOfView.Radius = _data.RadiusDetection;
            fieldOfView.ViewAngle = _data.AngleDetection;
            fieldOfView.TargetMask = _data.TargetMaskDetection;
            fieldOfView.ObstacleMask = _data.ObstacleMaskDetection;

            // Create State Machine
            _stateMachine = new StateMachine();

            // Create states
            var stateIdle = new Idle(_data);
            var stateMoveBetweenPoints = new MoveBetweenPoints(_data, navMeshAgent, _animator, _points);
            var stateChasing = new ChaseTarget(_data, fieldOfView, navMeshAgent, _animator);

            #region Create transitions from "IDLE" state

            _stateMachine.AddTransition(stateIdle, stateMoveBetweenPoints, () => stateIdle.RemainingWaitingTime == 0);

            #endregion

            #region Crate transitions from "MOVE BETWEEN POINTS" state

            _stateMachine.AddTransition(stateMoveBetweenPoints, stateIdle, () => stateMoveBetweenPoints.IsWaiting);

            #endregion

            #region Crate transitions from "CHASE TARGET" state

            // If there is a detected target then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => fieldOfView.NearestTarget != null);

            // If it isn't chasing and not in attack range or there isn't target any more
            _stateMachine.AddTransition(stateChasing, stateIdle, () => (!stateChasing.IsChasing && !stateChasing.IsOnAttackRange) || fieldOfView.NearestTarget == null);

            #endregion

            // Start State machine at "IDLE" state
            _stateMachine.SetState(stateIdle);
        }

        private void Update()
        {
            if (_isDead)
                return;

            _stateMachine.Tick();
        }

        #endregion
    }
}