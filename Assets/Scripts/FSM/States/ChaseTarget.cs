using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class ChaseTarget : IState
    {
        #region Public properties

        public bool IsChasing { get => _isChasing; }
        public bool IsOnAttackRange { get => _isOnAttackRange; }

        #endregion

        #region Private properties

        private Transform _transform = null;
        private Transform _currentTarget = null;
        private float _distanceChaseDetection = 0;
        private float _distanceAttackDetection = 0;
        private bool _isChasing = false;
        private bool _isOnAttackRange = false;
        private NavMeshAgent _agent = null;
        private FieldOfView _fov = null;
        private float _speedChaseMovement = 0;

        #endregion

        public ChaseTarget(EnemyData data, FieldOfView fov, NavMeshAgent agent, Animator animator)
        {
            _agent = agent;
            _fov = fov;
            _speedChaseMovement = data.SpeedChasingMovement;
        }

        #region IState implementation

        public void Tick()
        {
            if (!_isChasing)
                return;

            if (_fov.NearestTarget == null)
            {
                _isChasing = false;

                _agent.isStopped = true;

                return;
            }

            _agent.SetDestination(_fov.NearestTarget.position);
        }

        public void OnEnter()
        {
            _isChasing = true;

            _agent.enabled = true;
            _agent.speed = _speedChaseMovement;
            _agent.SetDestination(_fov.NearestTarget.position);
            _agent.isStopped = false;

            // TODO: move animator to "CHASE" state

            Debug.Log($"<b>CHASE TARGET</b> - <color=green>OnEnter</color> - target: {_fov.NearestTarget.name}");
        }

        public void OnExit()
        {
            // Disables Nav mesh Agent
            _agent.enabled = false;

            Debug.Log("<b>CHASE TARGET</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}