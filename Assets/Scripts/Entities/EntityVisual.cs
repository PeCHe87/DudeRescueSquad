using System;
using UnityEngine;
using UnityEngine.AI;

// Reference: https://www.gamedev.net/articles/programming/general-and-gameplay-programming/pathfinding-and-local-avoidance-for-rpgrts-games-using-unity-r3703/

namespace DudeResqueSquad
{
    public class EntityVisual : MonoBehaviour
    {
        public Action<float> OnFollowingStateChanged;
        
        #region Inspector properties

        [SerializeField] private Transform _target = null;
        [SerializeField] private float _speed = 2;
        [SerializeField] private float _rotationSpeed = 8;
        [SerializeField] private float _offsetToStop = 0;

        #endregion

        #region Public properties

        public Transform Target { get => _target; set => _target = value; }
        public NavMeshAgent Agent { set => _agent = value; }
        public bool IsMoving { get => _isMoving; }

        public float DeltaMovement
        {
            get => _deltaMovement;
        }

        #endregion

        #region Private properties

        private Transform _transform = null;
        private Vector3 _lastPosition;
        private NavMeshAgent _agent = null;
        private bool _lookAtTarget = false;
        private Transform _entityTarget = null;
        private bool _isMoving = false;
        private bool _isMovingOld = false;
        private float _offsetToStopPow = 0;
        private bool _canFollow = true;
        private float _deltaMovement = 0;

        #endregion

        #region Private methods

        private void Awake()
        {
            _transform = transform;

            _offsetToStopPow = Mathf.Pow(_offsetToStop, 2);
        }

        private void Start()
        {
            if (_target != null)
                _transform.position = _target.position;
        }

        private void Update()
        {
            _deltaMovement = 0;
            
            if (!_canFollow)
                return;

            if (_lookAtTarget)
            {
                LookAt();
            }

            if (_target == null)
                return;

            if (_agent == null)
                return;

            _isMovingOld = _isMoving;
            
            // Test if the distance between the agent (which is now the proxy) and the entity is less than the offset distance to stop
            if ((_transform.position - _target.position).sqrMagnitude < _offsetToStopPow)
            {
                _isMoving = false;
                return;
            }

            _isMoving = true;

            var oldPosition = _transform.position;

            // Follow the target proxy
            var direction = _target.position - transform.position;
            _transform.position += direction * Time.deltaTime * _speed;

            // Calculate the orientation based on the velocity of the agent 
            Vector3 orientation = _transform.position - _lastPosition;

            // Check if the agent has some minimal velocity 
            if (orientation.sqrMagnitude > 0.1f)
            {
                // We don't want him to look up or down 
                orientation.y = 0;

                // Use Quaternion.LookRotation() to set the model's new rotation and smooth the transition with Quaternion.Lerp();
                _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(_transform.position - _lastPosition), Time.deltaTime * _rotationSpeed);
            }
            else
            {
                // If the agent is stationary we tell him to assume the proxy's rotation 
                _transform.rotation = Quaternion.Lerp(_transform.rotation, Quaternion.LookRotation(_target.forward), Time.deltaTime * _rotationSpeed);
            }

            _deltaMovement = (oldPosition - _transform.position).magnitude;
            
            // This is needed to calculate the orientation in the next frame 
            _lastPosition = _transform.position;
        }

        private void LateUpdate()
        {
            CheckMovingStateUpdate();
        }

        private void CheckMovingStateUpdate()
        {
            //if (_isMoving != _isMovingOld)
            {
                OnFollowingStateChanged(_deltaMovement);
            }
        }

        private void LookAt()
        {
            if (_target == null)
                return;

            transform.rotation = Quaternion.LookRotation(_entityTarget.position - transform.position);
        }

        #endregion

        #region Public methods

        public void StartLookAtTarget(Transform target)
        {
            _lookAtTarget = true;
            _entityTarget = target;
        }

        public void StopLookAtTarget()
        {
            _lookAtTarget = false;
            _entityTarget = null;
        }

        public void StopFollowing()
        {
            _canFollow = false;
        }

        public void ResumeFollowing()
        {
            _canFollow = true;
        }

        #endregion
    }
}