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

        #endregion

        #region Public properties

        public Transform Target { get; private set; }
        public NavMeshAgent Agent { get => _agent; }
        public NavMeshObstacle Obstacle { get => _obstacle; }

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
            // TODO: config all attributes, like agent speed from entity's data
            _agent.speed = data.SpeedChasingMovement;
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }

        #endregion
    }
}