using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityFollower : MonoBehaviour
    {
        #region Private properties

        private NavMeshAgent _agent = null;
        private NavMeshObstacle _obstacle = null;
        private Vector3 _obstacleSize = Vector3.zero;

        #endregion

        #region Public properties

        public Transform Target { get; private set; }
        public NavMeshAgent Agent { get => _agent; }
        public NavMeshObstacle Obstacle { get => _obstacle; }
        public Vector3 ObstacleSize { get => _obstacleSize; }

        #endregion

        #region Private methods

        private void Awake()
        {
            _agent = GetComponent<NavMeshAgent>();
            _obstacle = GetComponent<NavMeshObstacle>();
        }

        #endregion

        #region Public methods

        public void Config(EntityData data)
        {
            _obstacleSize = _obstacle.size;

            // TODO: config all attributes, like agent speed from entity's data
            _agent.speed = data.SpeedChasingMovement;
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }

        public void StopSpeed()
        {
            _agent.speed = 0;
        }

        #endregion
    }
}