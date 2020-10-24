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

        private IEnumerator Resume()
        {
            _obstacle.carving = false;
            _obstacle.enabled = false;

            yield return new WaitForEndOfFrame();

            _agent.enabled = true;
            _agent.isStopped = false;
        }

        #endregion

        #region Public methods

        public void Config(EntityData data)
        {
            // TODO: config all attributes, like agent speed from entity's data
            _agent.speed = data.SpeedChasingMovement;
        }

        public void Stop()
        {
            if (_agent.enabled)
            {
                _agent.isStopped = true;
                _agent.enabled = false;
            }

            _obstacle.transform.position = _agent.transform.position;
            _obstacle.enabled = true;
            _obstacle.carving = true;
        }

        public void SetTarget(Transform target)
        {
            Target = target;
        }

        public void ResumeMovement()
        {
            StartCoroutine(Resume());
        }

        #endregion
    }
}