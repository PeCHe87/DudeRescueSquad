using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.LevelManagement;
using DudeResqueSquad;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class InteractableEnemiesDetectionSystem : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        #region Private properties

        private Enums.InteractablePriorities _priority = Enums.InteractablePriorities.ENEMY;
        private bool _wasInitialized = false;
        private FieldOfView _fov = default;
        private string _currentEnemyTargetId = default;
        private BaseInteractable _currentEnemyTarget = default;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        private void LateUpdate()
        {
            if (!_wasInitialized) return;

            GetNearestInteractable();
        }

        #endregion

        #region Private methods

        private void Initialize(Transform character)
        {
            _fov = character.GetComponentInChildren<FieldOfView>();

            if (_fov != null)
            {
                _fov.OnDetectNewTarget += DetectTarget;
                _fov.OnStopDetecting += UndetectTarget;
            }

            _wasInitialized = true;
        }

        private void Teardown()
        {
            if (_fov != null)
            {
                _fov.OnDetectNewTarget -= DetectTarget;
                _fov.OnStopDetecting -= UndetectTarget;
            }
        }

        /// <summary>
        /// Gets the nearest interactable objects in the level and communicates if there is a new one
        /// </summary>
        private void GetNearestInteractable()
        {
            // Undetect an object
            if (_currentEnemyTarget == null && !string.IsNullOrEmpty(_currentEnemyTargetId))
            {
                var payload = new object[] { null, _priority };

                _currentEnemyTargetId = string.Empty;

                GameLevelEvent.Trigger(GameLevelEventType.InteractableChanged, payload);

                Debug.Log($"{_priority} - <color=red>Object detected: </color> NO ELEMENT", this);

                return;
            }

            // Detect an object and nothing was currently detected
            if (_currentEnemyTarget != null && string.IsNullOrEmpty(_currentEnemyTargetId))
            {
                var payload = new object[] { _currentEnemyTarget, _priority };

                _currentEnemyTargetId = _currentEnemyTarget.Id;

                GameLevelEvent.Trigger(GameLevelEventType.InteractableChanged, payload);

                Debug.Log($"{_priority} - <color=red>Object detected: </color>{_currentEnemyTargetId}", this);

                return;
            }

            // Detect something when there was another object already detected
            if (_currentEnemyTarget != null && !_currentEnemyTargetId.Equals(_currentEnemyTarget.Id))
            {
                var payload = new object[] { _currentEnemyTarget, _priority };

                _currentEnemyTargetId = _currentEnemyTarget.Id;

                GameLevelEvent.Trigger(GameLevelEventType.InteractableChanged, payload);

                Debug.Log($"{_priority} - <color=red>Object detected update: </color>{_currentEnemyTargetId}", this);
            }
        }

        /// <summary>
        /// When character detects an enemy it should be added as another interactable object
        /// </summary>
        private void DetectTarget(Transform target)
        {
            if (_priority != Enums.InteractablePriorities.ENEMY) return;

            _currentEnemyTarget = target.GetComponent<BaseInteractable>();

            GetNearestInteractable();
        }

        /// <summary>
        /// When character loses the detected enemy it should be removed from the list if it was part of it
        /// </summary>
        private void UndetectTarget()
        {
            if (_priority != Enums.InteractablePriorities.ENEMY) return;

            _currentEnemyTarget = null;

            GetNearestInteractable();
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.LevelLoaded:
                    Initialize(eventData.Character.transform);
                    break;

                case GameLevelEventType.LevelUnloaded:
                    Teardown();
                    break;
            }
        }

        #endregion
    }
}