using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityStatePatrolling : IState
    {
        #region Private properties

        private Entity _entity = null;
        private bool _isWaiting = false;
        private Transform[] _points = null;
        private int _currentPointIndex = 0;
        private float _patrollingTime = 0;
        private float _distanceToStop = 1.5f;

        #endregion

        public bool IsWaiting { get => _isWaiting; }
        public float RemainingTime { get => _patrollingTime; }

        public EntityStatePatrolling(Entity entity, Transform[] points)
        {
            _entity = entity;

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
            if (_entity.Follower == null)
                return;

            if (!_entity.Follower.Agent.enabled)
                return;

            if (_entity.Follower.Agent.isStopped)
                return;

            // Decrease patrolling time
            _patrollingTime = Mathf.Clamp(_patrollingTime - Time.deltaTime, 0, _patrollingTime);

            // Check if it reaches the next goal point
            var remainingDistance = (_entity.Follower.Target.position - _entity.Follower.Agent.transform.position).magnitude;

            if (remainingDistance <= _distanceToStop)
            {
                _currentPointIndex = (_currentPointIndex + 1) % _points.Length;

                // If time patrolling is zero then stop moving when current goal is reached
                if (_patrollingTime > 0)
                {
                    _entity.Follower.SetTarget(_points[_currentPointIndex]);

                    Debug.Log($"<b>PATROLLING</b> - <b>Change waypoint</b> - new waypoint: {_currentPointIndex}");
                }
                else
                {
                    _isWaiting = true;
                }
            }

            //Debug.Log($"<b>PATROLLING</b> - <color=yellow>Tick</color> - Patrolling time: {_patrollingTime}, current point index: {_currentPointIndex}, remaining distance: {remainingDistance}");
        }

        public void OnEnter()
        {
            _isWaiting = false;
            _patrollingTime = Random.Range(_entity.Data.MinPatrollingTime, _entity.Data.MaxPatrollingTime);

            _currentPointIndex = Random.Range(0, _points.Length);

            _distanceToStop = _entity.Data.PatrollingDistanceToStop + 0.25f;
            
            // Get current point and start nave mesh agent movement
            _entity.Follower.SetTarget(_points[_currentPointIndex]);

            Debug.Log($"<b>PATROLLING</b> - <color=green>OnEnter</color> - current point: {_currentPointIndex}, patrolling time: {_patrollingTime}");
        }

        public void OnExit()
        {
            Debug.Log("<b>PATROLLING</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}