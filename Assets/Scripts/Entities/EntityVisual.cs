using UnityEngine;

// Reference: https://www.gamedev.net/articles/programming/general-and-gameplay-programming/pathfinding-and-local-avoidance-for-rpgrts-games-using-unity-r3703/

namespace DudeResqueSquad
{
    public class EntityVisual : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Transform _target = null;
        [SerializeField] private float _speed = 2;
        [SerializeField] private float _rotationSpeed = 8;
        [SerializeField] private float _offsetToStop = 0;

        #endregion

        #region Public properties

        public Transform Target { get => _target; set => _target = value; }

        #endregion

        #region Private properties

        private Transform _transform = null;
        private Vector3 _lastPosition;

        #endregion

        #region Private methods

        private void Awake()
        {
            _transform = transform;
        }

        private void Start()
        {
            if (_target != null)
                _transform.position = _target.position;
        }

        private void Update()
        {
            // Test if the distance between the agent (which is now the proxy) and the entity is less than the offset distance to stop
            if ((_transform.position - _target.position).sqrMagnitude < Mathf.Pow(_offsetToStop, 2))
                return;

            // Follow the target proxy
            _transform.position = Vector3.Lerp(_transform.position, _target.position, Time.deltaTime * _speed);
            //_transform.position = _target.position;

            // Rotate towards target proxy
            //_transform.rotation = _target.rotation;

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

            // This is needed to calculate the orientation in the next frame 
            _lastPosition = _transform.position;
        }

        #endregion

        #region Public methods

        #endregion
    }
}