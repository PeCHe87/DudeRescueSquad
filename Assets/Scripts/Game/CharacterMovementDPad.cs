using System;
using UnityEngine;
using static DudeResqueSquad.Character;

namespace DudeResqueSquad
{
    public class CharacterMovementDPad : MonoBehaviour, ICharacterMovement
    {
        public static Action<Vector2> OnStartDrag;
        public static Action<DragInfo> OnDrag;
        public static Action<Vector2> OnTouch;
        public static Action OnEndDrag;

        public enum SwipeType { NONE, UP, DOWN, LEFT, RIGHT }

        [Serializable]
        public struct DragInfo
        {
            public SwipeType swipeType;
            public Vector2 startPosition;
            public Vector2 endPosition;
            public Vector3 direction;

            public void ResetInfo()
            {
                swipeType = SwipeType.NONE;
                startPosition = endPosition = Vector2.zero;
                direction = Vector3.zero;
            }
        }

        #region Inspector properties

        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _zOffet = 10;

        #endregion

        #region Private properties

        private FixedJoystick _joystick = null;
        private bool _isMoving = false;
        private bool _touchStarted = false;
        private int _framesSinceTouchStarted = 0;
        private Vector2 _touchUp = Vector3.zero;
        private Vector2 _touchDown = Vector2.zero;
        private DragInfo _currentDragInfo;
        private float _minDistanceForSwipe = 20f;
        private bool _isDragging = false;

        #endregion

        public DragInfo CurrentDragInfo { get => _currentDragInfo; }

        #region Private methods

        private void Awake()
        {
            _joystick = FindObjectOfType<FixedJoystick>();

            if (_joystick == null)
                Debug.LogError("Don't find any Joystick on Scene");

            _currentDragInfo = new DragInfo();
        }

        private void Update()
        {
            ControlTouches();

            /*
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
            */
        }

        /*private bool DetectTap()
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
        }*/

        private void ControlTouches()
        {
            int touches = Input.touches.Length;

            // Check if there is at least one touch
            if (touches > 0)
            {
                // First touch
                var touch = Input.touches[0];

                // Check current state of touch
                if (touch.phase == TouchPhase.Began)        // - Began
                {
                    _touchUp = touch.position;
                    _touchDown = touch.position;

                    _framesSinceTouchStarted = 0;
                }
                else if (touch.phase == TouchPhase.Moved)   // - Moved
                {
                    _framesSinceTouchStarted++;

                    _touchDown = touch.position;

                    if (!_isDragging)
                    {
                        _isDragging = true;

                        OnStartDrag?.Invoke(_touchUp);
                    }

                    DetectSwipe();

                    Move(_currentDragInfo.direction.x, _currentDragInfo.direction.y);
                }
                else if (touch.phase == TouchPhase.Ended)   // - Ended
                {
                    // Reset current swipe
                    _currentDragInfo.ResetInfo();

                    if (_isDragging)
                    {
                        _isDragging = false;
                        OnEndDrag?.Invoke();

                        StopMoving();
                    }
                    else
                    {
                        if (_framesSinceTouchStarted <= 30)
                        {
                            OnTouch?.Invoke(_touchDown);

                            DoAction();
                        }
                    }

                    // Reset frames since touch has started
                    _framesSinceTouchStarted = 0;
                }
            }
        }

        private void DetectSwipe()
        {
            var swipe = SwipeType.NONE;
            float vertical = VerticalMovementDistance();
            float horizontal = HorizontalMovementDistance();

            bool checkedDistance = CheckMinDistanceRequired(vertical, horizontal);

            // If swipe min distance is checked
            if (checkedDistance)
            {
                bool isVertical = IsVerticalSwipe(vertical, horizontal);
                
                // If vertical swipe, check direction
                if (isVertical)
                    swipe = (_touchDown.y - _touchUp.y > 0) ? SwipeType.UP : SwipeType.DOWN;
                else    // Horizontal swipe, check direction
                    swipe = (_touchDown.x - _touchUp.x > 0) ? SwipeType.RIGHT : SwipeType.LEFT;
            }

            _currentDragInfo.swipeType = swipe;
            _currentDragInfo.startPosition = _touchUp;
            _currentDragInfo.endPosition = _touchDown;
            _currentDragInfo.direction = ScreenToWorldDirection(_currentDragInfo.startPosition, _currentDragInfo.endPosition);

            OnDrag?.Invoke(_currentDragInfo);

            // Reset touches
            //_touchUp = _touchDown;
        }

        public Vector3 ScreenToWorldDirection(Vector2 start, Vector2 end)
        {
            // Start position
            var startWorldPosition = _camera.ScreenToWorldPoint(new Vector3(start.x, start.y, _zOffet));

            // End position
            var endWorldPosition = _camera.ScreenToWorldPoint(new Vector3(end.x, end.y, _zOffet));

            var direction = endWorldPosition - startWorldPosition;
            direction.Normalize();
            direction.z = 0;

            return direction;
        }

        private float VerticalMovementDistance()
        {
            return Mathf.Abs(_touchDown.y - _touchUp.y);
        }

        private float HorizontalMovementDistance()
        {
            return Mathf.Abs(_touchDown.x - _touchUp.x);
        }

        private bool IsVerticalSwipe(float verticalDistance, float horizontalDistance)
        {
            return verticalDistance > horizontalDistance;
        }

        private bool CheckMinDistanceRequired(float verticalDistance, float horizontalDistance)
        {
            return verticalDistance > _minDistanceForSwipe ||
                   horizontalDistance > _minDistanceForSwipe;
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

        public Vector3 Direction()
        {
            return _currentDragInfo.direction;
        }

        #endregion
    }
}