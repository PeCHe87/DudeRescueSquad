using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.LevelManagement;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// Add this ability to a character and it'll be able to dash (cover the specified distance in the specified time)
    /// </summary>
    public class CharacterAbilityDash : CharacterAbility, IGameEventListener<GameLevelEvent>
    {
        #region Events

        public event System.Action OnStartAction;
        public event System.Action<float> OnProcessActionProgress;
        public event System.Action OnStopAction;

        #endregion

        #region Inspector properties

        [Header("Configuration")]
        [Tooltip("The distance to cover")]
        [SerializeField] private float _dashDistance = 10f;
        [Tooltip("The duration of the dash, in seconds")]
        [SerializeField] private float _dashDuration = 0.5f;
        [Tooltip("The curve to apply to the dash's acceleration")]
        [SerializeField] private AnimationCurve _dashCurve = new AnimationCurve(new Keyframe(0f, 0f), new Keyframe(1f, 1f));
        [Tooltip("Time to wait until recover input movement after applying a dash")]
        [SerializeField] private float _delayAfterDash = 0.5f;
        [SerializeField] private float _speed = 10;

        #endregion

        #region Private properties

        private Character _character = null;
        private CharacterAbilityMovement _characterMovement = null;
        private CharacterAbilityOrientation _characterOrientation = null;
        private ICharacterController _controller = null;
        private bool _dashPressed = false;
        private bool _dashStarted = false;
        private float _dashTimer = 0;
        private Vector3 _dashOrigin = default;
        private Vector3 _dashDestination = default;
        private Vector3 _newPosition = default;

        #endregion

        #region Public properties

        public bool CanProcessAction => _character.CanStartAction(Enums.ActionType.DASH);
        public bool IsDashing => _dashStarted;
        public float DashProgress => _dashTimer / _dashDuration;

        #endregion

        #region Unity events

        private void Awake()
        {
            //ButtonActionManager.OnStartAction += StartAction;
        }

        /// <summary>
        /// On enable, we start listening for GameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for GameEvents. You may want to extend that to stop listening to other types of events.
        /// </summary>
        protected void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        private void OnDestroy()
        {
            //ButtonActionManager.OnStartAction -= StartAction;
        }

        #endregion

        #region CharacterAbility implementation

        public override void Process()
        {
            // Process dash movement if it was started
            if (!_dashStarted) return;

            ProcessDash();
        }

        public override void Initialization()
        {
            _controller = GetComponent<ICharacterController>();

            _character = GetComponent<Character>();

            _characterMovement = GetComponent<CharacterAbilityMovement>();

            _characterOrientation = GetComponent<CharacterAbilityOrientation>();

            _wasInitialized = true;
        }

        /// <summary>
        /// Gets input and triggers methods based on what's been pressed
        /// </summary>
        protected override void HandleInput()
        {
            TestInput();

            // Check input pressed
            if (!_dashPressed) return;

            // Check if a previous dash has been started
            if (_dashStarted) return;

            // TODO: check time to apply dash again if timer allows it

            StartDash();
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        /// <summary>
        /// Check different events related with game level
        /// </summary>
        /// <param name="eventData">Inventory event.</param>
        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.StartPlayerDash:
                    StartAction();
                    break;
            }
        }

        #endregion

        #region Private methods

        private void StartAction(CustomEventArgs.StartActionEventArgs evtArgs)
        {
            // Check action type
            if (evtArgs.Type != Enums.ActionType.DASH) return;

            _dashPressed = true;
        }

        private void StartAction()
        {
            _dashPressed = true;
        }

        private void StartDash()
        {
            // Check if it is possible based on character state
            if (!_character.CanStartAction(Enums.ActionType.DASH))
            {
                _dashPressed = false;

                return;
            }

            // Update character state
            _character.UpdateState(Enums.CharacterState.DASHING);

            var cacheTransform = transform;
            _dashStarted = true;
            _dashTimer = 0f;
            _dashOrigin = transform.position;

            var characterOrientation = GetCharacterOrientation();
            _dashDestination = cacheTransform.position + characterOrientation * _dashDistance;

            OnStartAction?.Invoke();

            ProcessDashMovement();
        }

        private Vector3 GetCharacterOrientation()
        {
            // If character is moving then make dash movement towards the input movement direction
            if (_characterMovement.CurrentDirection != Vector3.zero)    //if (_character.State == Enums.CharacterState.MOVING)
            {
                return _characterMovement.CurrentDirection;
            }

            // If character is not moving then use the current orientation
            return _characterOrientation.CurrentRotation;

            //return _characterMovement.PreviousDirection;
        }

        private void ProcessDashMovement()
        {
            var progress = _dashTimer / _dashDuration;

            _newPosition = Vector3.Lerp(_dashOrigin, _dashDestination, _dashCurve.Evaluate(progress));
            _dashTimer += Time.deltaTime;

            progress = _dashTimer / _dashDuration;

            _characterMovement.MoveToPosition(_newPosition, _speed);

            OnProcessActionProgress?.Invoke(progress);
        }

        private void ProcessDash()
        {
            // Check if dash can continue or it should be finished
            if (_dashTimer < _dashDuration)
            {
                ProcessDashMovement();
            }
            else
            {
                FinishDash();
            }
        }

        private void FinishDash()
        {
            _dashPressed = false;
            _dashStarted = false;

            _characterMovement.Disable();

            Invoke(nameof(RecoverInputMovementAfterDash), _delayAfterDash);
        }

        private void RecoverInputMovementAfterDash()
        {
            _characterMovement.Enable();

            // Update character state
            _character.StopAction(Enums.CharacterState.DASHING);

            OnStopAction?.Invoke();
        }

        private void TestInput()
        {
            if (_dashStarted) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                //StartAction(new CustomEventArgs.StartActionEventArgs(Enums.ActionType.DASH));

                StartAction();
            }
        }

        #endregion
    }
}