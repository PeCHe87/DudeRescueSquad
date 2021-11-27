using System;
using System.Collections;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityFollower : MonoBehaviour
    {
        public Action<bool> OnAgentStateChanged;
        
        #region Inspector properties
        
        [SerializeField] private bool _skipObstacleLogic = false;
        [SerializeField] private bool _skipAgentLogic = false;
        
        #endregion
        
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

        private IEnumerator CommunicatesAgentStateChanged(bool isEnabled)
        {
            yield return new WaitForEndOfFrame();
            
            OnAgentStateChanged?.Invoke(isEnabled);
        }
        
        #region Public methods

        public void Config(EntityData data)
        {
            _obstacleSize = _obstacle.size;

            // Config agent speed from entity's data
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

        public void SetObstacleEnabledState(bool isEnabled)
        {
            if (_skipObstacleLogic)
            {
                _obstacle.carving = false;
                _obstacle.enabled = false;
            }
            else
            {
                _obstacle.carving = isEnabled;
                _obstacle.enabled = isEnabled;
            }
        }
        
        public void SetAgentEnabledState(bool isEnabled)
        {
            if (_skipAgentLogic)
            {
                _agent.enabled = true;
            }
            else
            {
                _agent.enabled = isEnabled;
            }

            StartCoroutine(CommunicatesAgentStateChanged(isEnabled));
        }

        public void RotatesTowardTarget()
        {
            if (Target == null)
            {
                return;
            }

            var direction = Target.position - transform.position;
            transform.rotation = Quaternion.LookRotation(direction);
        }
        
        public void StopTakingDamage()
        {
            StopSpeed();
        }

        public void ResumeAfterTakingDamage()
        {

        }

        #endregion
    }
}