using System;
using UnityEngine;
using static DudeResqueSquad.Character;

namespace DudeResqueSquad
{
    public class CharacterMovementDPad : MonoBehaviour, ICharacterMovement
    {
        #region Private properties

        private FixedJoystick _joystick = null;
        private bool _isMoving = false;
        private bool _touchStarted = false;
        private int _framesSinceTouchStarted = 0;

        #endregion

        #region Private methods

        private void Awake()
        {
            _joystick = FindObjectOfType<FixedJoystick>();

            if (_joystick == null)
                Debug.LogError("Don't find any Joystick on Scene");
        }

        private void Update()
        {
            if (_joystick == null)
                return;

            float x = _joystick.Horizontal;
            float y = _joystick.Vertical;

            if (Mathf.Abs(x) <= _joystick.DeadZone && Mathf.Abs(y) <= _joystick.DeadZone)
            {
                bool tapDetected = DetectTap();

                if (tapDetected)
                    DoAction();
                else if (_isMoving)
                   StopMoving();

                return;
            }
            else if (_touchStarted) // Reset checking of touch for action
            {
                _touchStarted = false;
                _framesSinceTouchStarted = 0;
                Debug.Log("Cancel Touch checking");
            }

            Move(x, y);
        }

        private bool DetectTap()
        {
            // Make checking
            int length = Input.touches.Length;

            if (_touchStarted)
                _framesSinceTouchStarted++;

            if (length > 0)
            {
                var touch = Input.touches[0];

                if (touch.phase == TouchPhase.Began)
                {
                    _touchStarted = true;
                    _framesSinceTouchStarted = 0;
                    Debug.Log("Touch <color=green><b>BEGAN</b></color>");
                }
                else if (touch.phase == TouchPhase.Moved)
                {
                    if (_touchStarted)
                        Debug.Log("Touch <color=red><b>MOVED</b></color>");

                    _touchStarted = false;
                }
                else if (touch.phase == TouchPhase.Ended && _touchStarted)
                {
                    _touchStarted = false;

                    if (_framesSinceTouchStarted <= 30)
                    {
                        Debug.Log("Touch <b>ENDED</b> frames: " + _framesSinceTouchStarted);
                        return true;
                    }
                }
            }

            return false;
        }

        private void StartMoving(MovementEventArgs e)
        {
            _isMoving = true;

            OnStartMoving?.Invoke(this, e);
        }

        private void StopMoving()
        {
            _isMoving = false;

            OnStopMoving?.Invoke(this, EventArgs.Empty);
        }

        private void DoAction()
        {
            OnDoAction?.Invoke(this, EventArgs.Empty);
        }

        private void Move(float x, float y)
        {
            if (!_isMoving)
                StartMoving(new MovementEventArgs(x, y));
        }

        #endregion

        #region ICharacterMovement Implementation

        public event EventHandler OnDoAction;
        public event EventHandler<MovementEventArgs> OnStartMoving;
        public event EventHandler OnStopMoving;

        public float Horizontal()
        {
            return _joystick.Horizontal;
        }

        public float Vertical()
        {
            return _joystick.Vertical;
        }

        #endregion
    }
}