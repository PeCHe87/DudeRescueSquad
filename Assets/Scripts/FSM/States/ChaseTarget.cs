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
        private NavMeshAgent _navMeshAgent = null;
        private NavMeshObstacle _navMeshObstacle = null;
        private FieldOfView _fov = null;
        private float _speedChaseMovement = 0;

        #endregion

        public ChaseTarget(EnemyData data, FieldOfView fov, NavMeshAgent agent, Animator animator, NavMeshObstacle obstacle)
        {
            _data = data;

            _navMeshAgent = agent;
            _navMeshObstacle = obstacle;

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
            //if (!_isChasing)
            //    return;

            // If there isn't any detected target stop chasing
            if (_fov.NearestTarget == null)
            {
                _isChasing = false;

                _navMeshAgent.isStopped = true;

                return;
            }

            //Debug.Log($"<color=magenta>Chasing</color> - distance: {_agent.remainingDistance}");

            // Check if distance to target is enough or it should continue chasing it
            if (_navMeshAgent.remainingDistance <= _data.MinChasingDistance)
            {
                // If nav agent is moving then stop it
                if (!_navMeshAgent.isStopped)
                {
                    _navMeshAgent.isStopped = true;

                    _navMeshObstacle.enabled = true;
                }

                return;
            }

            // Check if it should resume nav agent movement
            if (_navMeshAgent.isStopped)
            {
                _navMeshAgent.isStopped = false;

                _navMeshObstacle.enabled = false;
            }

            // If it isn't closed enough chase the target
            _navMeshAgent.SetDestination(_fov.NearestTarget.position);
        }

        public void OnEnter()
        {
            _isChasing = true;

            // Activate agent
            _navMeshAgent.enabled = true;
            _navMeshAgent.SetDestination(_fov.NearestTarget.position);
            _navMeshAgent.isStopped = false;
            _navMeshAgent.speed = _speedChaseMovement;

            // Activate obstacle
            _navMeshObstacle.enabled = false;

            // TODO: move animator to "CHASE" state

            Debug.Log($"<b>CHASE TARGET</b> - <color=green>OnEnter</color> - target: {_fov.NearestTarget.name}");
        }

        public void OnExit()
        {
            // Disables Nav mesh Agent
            if (_navMeshAgent.enabled)
                _navMeshAgent.isStopped = true;

            Debug.Log("<b>CHASE TARGET</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}