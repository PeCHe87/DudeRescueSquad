using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core;
using DudeRescueSquad.Core.Inventory.View;
using DudeRescueSquad.Core.LevelManagement;

namespace DudeResqueSquad.UI
{
    public class PlayerInteractableIndicator : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        #region Inspector properties

        [SerializeField] private Enums.InteractablePriorities _priority = Enums.InteractablePriorities.NONE;
        [SerializeField] private Image _img = default;

        #endregion

        #region Private properties

        private Transform _originalParent;
        private Canvas _canvas = null;
        private Transform _cacheTransform = default;
        private Transform _currentTarget = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();

            _cacheTransform = transform;

            _originalParent = _cacheTransform.parent;

            Hide();
        }

        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        private void Update()
        {
            if (_cacheTransform != null && _currentTarget != null)
            {
                _cacheTransform.position = new Vector3(_currentTarget.position.x, 0.4f, _currentTarget.position.z);
            }
        }

        private void OnDestroy()
        {
            

            //GameEvents.OnDetectInteractable -= InteractableDetected;
           // GameEvents.OnStopDetectingIteractable -= InteractableLost;
        }

        #endregion

        #region Private methods

        private void Hide()
        {
            _canvas.enabled = false;
            _cacheTransform.SetParent(_originalParent);
        }

        private void Show(Transform target)
        {
            _cacheTransform.position = new Vector3(target.position.x, 0.4f, target.position.z);
            _cacheTransform.localRotation = Quaternion.Euler(90, 0, 0);
            _cacheTransform.localScale = Vector3.one * 0.001f;

            if (!_canvas.enabled)
            {
                _canvas.enabled = true;
            }

            if (target != _currentTarget)
            {
                _currentTarget = target;
            }
        }

        private void Refresh(object[] payload)
        {
            if (payload == null) return;

            if (payload.Length == 0) return;

            // Skip it if priority is different to the priority expected
            var priority = (Enums.InteractablePriorities) payload[1];

            if (priority != _priority) return;

            var interactable = payload[0] as BaseInteractable;

            // Check if it has to be hidden or shown
            if (interactable == null)
            {
                Hide();
            }
            else
            {
                Show(interactable.transform);
            }
        }

        #endregion

        #region IGameEventListener implementation

        /// <summary>
        /// Watches for different events like Interactable Pick Item events
        /// </summary>
        /// <param name="eventData">Interactable event.</param>
        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.InteractableChanged:
                    Refresh(eventData.Payload);
                    break;
            }
        }

        #endregion
    }
}