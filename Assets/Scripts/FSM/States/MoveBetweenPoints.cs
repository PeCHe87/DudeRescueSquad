using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class MoveBetweenPoints : IState
    {
        #region Private properties

        private EnemyData _data = null;
        private NavMeshAgent _navMeshAgent = null;
        private NavMeshObstacle _navMeshObstacle = null;
        private Animator _animator = null;
        private bool _isWaiting = false;
        private Transform[] _points = null;
        private int _currentPointIndex = 0;
        private float _patrollingTime = 0;
        private readonly float _distanceToReachGoal = 1.5f;

        #endregion

        public bool IsWaiting { get => _isWaiting; }

        public MoveBetweenPoints(EnemyData data, NavMeshAgent agent, Animator animator, Transform[] points, NavMeshObstacle obstacle)
        {
            _data = data;

            _navMeshAgent = agent;
            _navMeshObstacle = obstacle;

            _animator = animator;

            _patrollingTime = 0;
            _isWaiting = false;
            _currentPointIndex = 0;
            _points = points;
        }

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.PATROLLING;
        }

        public void Tick()
        {
            if (_navMeshAgent.isStopped)
                return;

            // Decrease patrolling time
            _patrollingTime = Mathf.Clamp(_patrollingTime - Time.deltaTime, 0, _patrollingTime);

            // Check if it reaches the next goal point
            float remainingDistance = _navMeshAgent.remainingDistance;

            if (remainingDistance <= _distanceToReachGoal)
            {
                _currentPointIndex = (_currentPointIndex + 1) % _points.Length;

                // If time patrolling is zero then stop moving when current goal is reached
                if (_patrollingTime > 0)
                {
                    _navMeshAgent.SetDestination(_points[_currentPointIndex].position);
                }
                else
                {
                    _navMeshAgent.isStopped = true;

                    _isWaiting = true;
                }
            }

            //Debug.Log($"<b>MOVE BETWEEN POINTS</b> - <color=yellow>Tick</color> - Patrolling time: {_patrollingTime}, current point index: {_currentPointIndex}, remaining distance: {remainingDistance}");
        }

        public void OnEnter()
        {
            _isWaiting = false;
            _patrollingTime = Random.Range(_data.MinPatrollingTime, _data.MaxPatrollingTime);

            _currentPointIndex = Random.Range(0, _points.Length);
            
            // Get current point and start nave mesh agent movement
            _navMeshAgent.enabled = true;
            _navMeshAgent.speed = _data.SpeedPatrollingMovement;
            _navMeshAgent.isStopped = false;
            _navMeshAgent.SetDestination(_points[_currentPointIndex].position);

            // Activate obstacle
            _navMeshObstacle.enabled = false;


            // TODO: move animator state to "MOVE"

            Debug.Log($"<b>MOVE BETWEEN POINTS</b> - <color=green>OnEnter</color> - current point: {_currentPointIndex}, patrolling time: {_patrollingTime}");
        }

        public void OnExit()
        {
            // Disables Nav mesh Agent
            if (_navMeshAgent.enabled)
                _navMeshAgent.isStopped = true;

            Debug.Log("<b>MOVE BETWEEN POINTS</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}