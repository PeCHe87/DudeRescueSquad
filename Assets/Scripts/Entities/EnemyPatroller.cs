using System;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    [RequireComponent(typeof(NavMeshAgent))]
    public class EnemyPatroller : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private EnemyData _data = null;
        [SerializeField] private Transform[] _points = null;
        [SerializeField] private Animator _animator = null;

        #endregion

        #region Private properties

        private StateMachine _stateMachine = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();

            // Create State Machine
            _stateMachine = new StateMachine();

            // Create states
            var stateIdle = new Idle(_data);
            var stateMoveBetweenPoints = new MoveBetweenPoints(_data, navMeshAgent, _animator, _points);

            // Create transitions
            _stateMachine.AddTransition(stateIdle, stateMoveBetweenPoints, () => stateIdle.RemainingWaitingTime == 0);
            _stateMachine.AddTransition(stateMoveBetweenPoints, stateIdle, () => stateMoveBetweenPoints.IsWaiting);

            // Start State machine at "IDLE" state
            _stateMachine.SetState(stateIdle);
        }

        private void Update() => _stateMachine.Tick();

        #endregion
    }
}