using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// TODO:
    /// </summary>
    public class CharacterAbilityMovement : MonoBehaviour, ICharacterAbility
    {
        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private float _speed = 1;

        private ICharacterController _controller = null;
        private bool _wasInitialized = false;
        private Transform _transform = null;
        private Vector3 _currentDirection = Vector3.zero;
        private Rigidbody _rigidBody = null;

        #region ICharacterAbility implementation

        public bool IsEnabled()
        {
            return _isEnabled;
        }

        public void EarlyProcessAbility(){}

        public void Process()
        {
            GetMovementDirection();
            Move();
        }

        public bool WasInitialized()
        {
            return _wasInitialized;
        }

        public void Initialization()
        {
            this._controller = GetComponent<ICharacterController>();

            this._wasInitialized = true;
        }

        #endregion

        #region Private methods

        private void Awake()
        {
            this._rigidBody = GetComponent<Rigidbody>();
            this._transform = transform;
        }

        private void GetMovementDirection()
        {
            if (_controller.Horizontal() == 0 && _controller.Vertical() == 0)
            {
                _currentDirection = Vector3.zero;
                return;
            }

            _currentDirection = _controller.Direction();

            _currentDirection = new Vector3(_currentDirection.x, 0, _currentDirection.y).normalized;

            Debug.DrawRay(_transform.position, _currentDirection * _speed, Color.yellow);
        }

        private void Move()
        {
            _rigidBody.velocity = Vector3.zero;

            if (_currentDirection == Vector3.zero) return;

            _rigidBody.AddForce(_currentDirection * _speed, ForceMode.VelocityChange);
        }

        #endregion
    }
}