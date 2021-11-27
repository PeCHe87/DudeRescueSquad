using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// TODO:
    /// </summary>
    public class CharacterAbilityMovement : MonoBehaviour, ICharacterAbility
    {
        #region Inspector properties

        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private float _speed = 1;

        #endregion

        #region Private properties

        private Character _character = null;
        private ICharacterController _controller = null;
        private bool _wasInitialized = false;
        private Transform _transform = null;
        private Vector3 _currentDirection = Vector3.zero;
        private Vector3 _lastInputDirection = Vector3.zero;
        private Vector3 _previousDirection = Vector3.zero;
        private Rigidbody _rigidBody = null;

        // Melee forward movement properties
        private Vector3 _meleeAttackDestination = default;
        private bool _meleeMovementEnabled = false;
        private float _meleeForwardSpeed = 0;
        private bool _processingMeleeAttack = false;

        #endregion

        #region Public properties

        public Vector3 CurrentDirection => _currentDirection;
        public Vector3 LastInputDirection => _lastInputDirection;
        public Vector3 PreviousDirection => _previousDirection;

        #endregion

        #region ICharacterAbility implementation

        public bool IsEnabled()
        {
            return _isEnabled;
        }

        public void EarlyProcessAbility(){}

        public void Process()
        {
            if (_meleeMovementEnabled)
            {
                MeleeMovement();
            }
            else if (!_processingMeleeAttack)
            {
                // Check if character can't make movement based on current state
                if (!_character.CanStartAction(DudeResqueSquad.Enums.ActionType.MOVE)) return;

                GetMovementDirection();
                Move();
            }
        }

        public bool WasInitialized()
        {
            return _wasInitialized;
        }

        public void Initialization()
        {
            _controller = GetComponent<JoystickCharacterController>();

            _character = GetComponent<Character>();

            _wasInitialized = true;
        }

        #endregion

        #region Public methods

        public void Disable()
        {
            _isEnabled = false;

            _currentDirection = Vector3.zero;

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;

            _rigidBody.isKinematic = true;
        }

        public void Enable()
        {
            _rigidBody.isKinematic = false;
            _isEnabled = true;
        }

        public void MoveToPosition(Vector3 position, float speed)
        {
            var direction = (position - _transform.position).normalized;

            Debug.DrawRay(_transform.position, direction * speed, Color.magenta);

            _rigidBody.velocity = Vector3.zero;

            _rigidBody.AddForce(direction * speed, ForceMode.VelocityChange);
        }

        public void StopMovement()
        {
            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
        }

        public void StartMeleeAttackForwardMovement(Vector3 direction, float speed, float duration, float attackDuration)
        {
            _processingMeleeAttack = true;

            _meleeAttackDestination = direction;
            _meleeForwardSpeed = speed;
            _meleeMovementEnabled = true;

            Invoke(nameof(StopMeleeAttackForwardMovement), duration);
            Invoke(nameof(MeleeAttackFinished), attackDuration);
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
            // Check current input for movement
            if (_controller.Horizontal() == 0 && _controller.Vertical() == 0)
            {
                _currentDirection = Vector3.zero;
                return;
            }

            _previousDirection = _currentDirection;

            _currentDirection = _controller.Direction();

            _currentDirection = new Vector3(_currentDirection.x, 0, _currentDirection.y).normalized;

            _lastInputDirection = _currentDirection;

            Debug.DrawRay(_transform.position, _currentDirection * _speed, Color.yellow);
        }

        private void Move()
        {
            _rigidBody.velocity = Vector3.zero;

            if (_currentDirection == Vector3.zero)
            {
                _character.UpdateState(DudeResqueSquad.Enums.CharacterState.IDLE);
                return;
            }

            _character.UpdateState(DudeResqueSquad.Enums.CharacterState.MOVING);

            _rigidBody.AddForce(_currentDirection * _speed, ForceMode.VelocityChange);
        }

        #region Melee forward movement

        private void StopMeleeAttackForwardMovement()
        {
            _meleeMovementEnabled = false;
            _meleeAttackDestination = Vector3.zero;
            _meleeForwardSpeed = 0;

            Disable();

            Debug.Log("<color=red>Melee attack</color> forward movement stopped");
        }

        private void MeleeAttackFinished()
        {
            _processingMeleeAttack = false;

            Enable();
        }

        private void MeleeMovement()
        {
            Debug.DrawRay(_transform.position, _meleeAttackDestination * _speed, Color.yellow);

            Debug.Log("<color=green>Melee attack</color> forward movement");

            _rigidBody.velocity = Vector3.zero;

            _rigidBody.AddForce(_meleeAttackDestination * _meleeForwardSpeed, ForceMode.VelocityChange);
        }

        #endregion

        #endregion
    }
}