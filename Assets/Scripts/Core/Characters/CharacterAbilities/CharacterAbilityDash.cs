using System;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// Add this ability to a character and it'll be able to dash (cover the specified distance in the specified time)
    /// </summary>
    public class CharacterAbilityDash : CharacterAbility
    {
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

        #region Unity events

        private void Awake()
        {
            ButtonActionManager.OnStartAction += StartAction;
        }

        private void OnDestroy()
        {
            ButtonActionManager.OnStartAction -= StartAction;
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

        #region Private methods

        private void StartAction(CustomEventArgs.StartActionEventArgs evtArgs)
        {
            // Check action type
            if (evtArgs.Type != Enums.ActionType.DASH) return;

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

            ProcessDashMovement();
        }

        private Vector3 GetCharacterOrientation()
        {
            if (_character.State == Enums.CharacterState.MOVING)
            {
                return _characterMovement.CurrentDirection;
            }

            return _characterMovement.LastInputDirection;
        }

        private void ProcessDashMovement()
        {
            _newPosition = Vector3.Lerp(_dashOrigin, _dashDestination, _dashCurve.Evaluate(_dashTimer / _dashDuration));
            _dashTimer += Time.deltaTime;

            _characterMovement.MoveToPosition(_newPosition, _speed);
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
        }

        private void TestInput()
        {
            if (_dashStarted) return;

            if (Input.GetKeyDown(KeyCode.Space))
            {
                StartAction(new CustomEventArgs.StartActionEventArgs(Enums.ActionType.DASH));
            }
        }

        #endregion
    }
}