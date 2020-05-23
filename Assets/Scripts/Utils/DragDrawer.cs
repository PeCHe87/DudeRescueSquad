using System.Collections;
using UnityEngine;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class DragDrawer : MonoBehaviour
    {
        [SerializeField] private Camera _camera = null;
        [SerializeField] private float _zOffet = 10;
        [SerializeField] private Image _imgTouch = null;
        [SerializeField] private Image _imgDragOrigin = null;
        [SerializeField] private Image _imgDragDirection = null;
        [SerializeField] private float _directionOffset = 2;
        [SerializeField] private float _timeToHideTouch = 1;
        [SerializeField] private bool _canDebug = false;

        private bool _hiddingTouch = false;
        private float _remainingTime = 0;
        private RectTransform _imgDragDirectionRectTransform = null;

        private void Awake()
        {
            _imgTouch.enabled = false;
            _imgDragDirection.enabled = false;
            _imgDragOrigin.enabled = false;

            _imgDragDirectionRectTransform = _imgDragDirection.GetComponent<RectTransform>();

            CharacterMovementDPad.OnDrag += DragMovement;
            CharacterMovementDPad.OnStartDrag += StartDrag;
            CharacterMovementDPad.OnEndDrag += EndDrag;
            CharacterMovementDPad.OnTouch += Touch;
        }

        private void OnDestroy()
        {
            CharacterMovementDPad.OnDrag -= DragMovement;
            CharacterMovementDPad.OnStartDrag -= StartDrag;
            CharacterMovementDPad.OnEndDrag -= EndDrag;
            CharacterMovementDPad.OnTouch -= Touch;
        }

        private void Update()
        {
            if (_hiddingTouch)
            {
                _remainingTime -= Time.deltaTime;

                if (_remainingTime <= 0)
                {
                    _remainingTime = 0;
                    _hiddingTouch = false;
                    _imgTouch.enabled = false;
                }
            }
        }

        private void Touch(Vector2 position)
        {
            _imgTouch.transform.position = position;
            _imgTouch.enabled = true;

            if (_canDebug)
                Debug.Log("<color=yellow>Touch</color>");

            _hiddingTouch = true;

            _remainingTime = _timeToHideTouch;
        }

        private void StartDrag(Vector2 position)
        {
            var startPosition = new Vector3(position.x, position.y, 0);

            // Hide touch image
            _imgTouch.enabled = false;
            _hiddingTouch = false;

            _imgDragOrigin.transform.position = startPosition;

            _imgDragOrigin.enabled = true;

            _imgDragDirection.transform.position = startPosition;
            _imgDragDirection.enabled = true;

            if (_canDebug)
                Debug.Log("<color=green>Start Drag</color>");
        }

        private void EndDrag()
        {
            _imgDragOrigin.enabled = false;
            _imgDragDirection.enabled = false;

            _imgDragOrigin.transform.position = Vector3.zero;
            _imgDragDirection.transform.position = Vector3.zero;

            if (_canDebug)
                Debug.Log("<color=red>End Drag</color>");
        }

        private void DragMovement(CharacterMovementDPad.DragInfo info)
        {
            // Start position
            var startPosition = info.startPosition;
            var startWorldPosition = _camera.ScreenToWorldPoint(new Vector3(startPosition.x, startPosition.y, _zOffet));

            Debug.DrawRay(startWorldPosition, info.direction * 2, Color.blue);

            float angle = Vector2.Angle(info.startPosition.normalized, info.direction.normalized);

            if (_canDebug)
                Debug.Log($"Angle to desired direction: {angle}");


            Vector3 directionPosition = startPosition;
            directionPosition += info.direction.normalized * _directionOffset;
            directionPosition.z = 0;

            _imgDragDirectionRectTransform.position = directionPosition;
        }
    }
}