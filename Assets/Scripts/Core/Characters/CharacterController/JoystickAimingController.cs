using DudeResqueSquad;
using System;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class JoystickAimingController : MonoBehaviour, ICharacterController
    {
        #region Inspector properties

        [SerializeField] private Joystick _joystick = null;
        [Tooltip("Total time to wait until the last manual aiming should be considered")]
        [SerializeField] private float _delayTime = 0;

        #endregion

        #region Private properties

        private float _remainingTime = 0;
        private bool _wasInitialized = false;
        private bool _pause = false;
        private bool _startDragging = false;

        #endregion

        #region Public properties

        public Joystick Joystick => _joystick;
        public bool InProgress => _remainingTime > 0;

        #endregion

        #region ICharacterMovement Implementation

        public event EventHandler<CustomEventArgs.MovementEventArgs> OnStartMoving;
        public event EventHandler OnStopMoving;

        public float Horizontal()
        {
            return _joystick.Horizontal;
        }

        public float Vertical()
        {
            return _joystick.Vertical;
        }

        public Vector3 Direction()
        {
            return _joystick.Direction;
        }

        public float DeadZone()
        {
            return _joystick.DeadZone;
        }

        public bool WasDragging()
        {
            return _startDragging;
        }

        #endregion

        #region Public methods

        public void Init()
        {
            _joystick.OnDragging += JoystickDragging;
            _joystick.OnRelease += JoystickWasReleased;

            _wasInitialized = true;
        }

        public void Teardown()
        {
            _joystick.OnDragging -= JoystickDragging;
            _joystick.OnRelease -= JoystickWasReleased;
        }

        public void Pause()
        {
            _pause = true;
        }

        public void Unpause()
        {
            _pause = false;
        }

        #endregion

        #region Unity events

        private void Update()
        {
            if (!_wasInitialized) return;

            if (_pause) return;

            if (_remainingTime <= 0) return;

            _remainingTime = Mathf.Clamp(_remainingTime - Time.deltaTime, 0, _delayTime);
        }

        #endregion

        #region Private methods

        private void JoystickDragging()
        {
            _startDragging = true;
        }

        private void JoystickWasReleased(Vector2 direction)
        {
            if (!_startDragging) return;

            _startDragging = false;

            _remainingTime = _delayTime;
        }

        #endregion
    }
}