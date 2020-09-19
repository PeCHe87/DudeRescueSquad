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


        private EnemyData _data = null;
        private float _minDistanceChasing = 0;
        private bool _isChasing = false;
        private bool _isOnAttackRange = false;
        private NavMeshAgent _agent = null;
        private FieldOfView _fov = null;
        private float _speedChaseMovement = 0;

        #endregion

        public ChaseTarget(EnemyData data, FieldOfView fov, NavMeshAgent agent, Animator animator)
        {
            _data = data;
            _agent = agent;
            _fov = fov;
            _speedChaseMovement = data.SpeedChasingMovement;
        }

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.CHASING;
        }

        public void Tick()
        {
            if (!_isChasing)
                return;

            // If there isn't any detected target stop chasing
            if (_fov.NearestTarget == null)
            {
                _isChasing = false;

                _agent.isStopped = true;

                return;
            }

            //Debug.Log($"<color=magenta>Chasing</color> - distance: {_agent.remainingDistance}");

            // Check if distance to target is enough or it should continue chasing it
            if (_agent.remainingDistance <= _data.MinChasingDistance)
            {
                // If nav agent is moving then stop it
                if (!_agent.isStopped)
                    _agent.isStopped = true;

                return;
            }

            // Check if it should resume nav agent movement
            if (_agent.isStopped)
                _agent.isStopped = false;

            // If it isn't closed enough chase the target
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