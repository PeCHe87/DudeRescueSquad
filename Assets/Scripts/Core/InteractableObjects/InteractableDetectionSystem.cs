using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.LevelManagement;
using DudeResqueSquad;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class InteractableDetectionSystem : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        #region Inspector properties

        [SerializeField] private Enums.InteractablePriorities _priority = Enums.InteractablePriorities.NONE;

        #endregion

        #region Private properties

        private Dictionary<string, BaseInteractable> _nearInteractables = default;
        private bool _wasInitialized = false;
        private BaseInteractable _nearest = default;
        private Transform _character = default;
        private float _distanceToCharacter = float.MaxValue;

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

            if (_nearInteractables.Count == 0) return;

            GetNearestInteractable();
        }

        #endregion

        #region Private methods

        private void Initialize(Transform character)
        {
            _character = character;

            _nearInteractables = new Dictionary<string, BaseInteractable>();

            _wasInitialized = true;
        }

        private void Teardown()
        {
            // TODO: check if it is needed to do something here or could be deleted
        }

        private void StartDetection(object[] payload)
        {
            if (!_wasInitialized) return;

            if (payload == null) return;

            if (payload.Length == 0) return;

            var interactable = payload[0] as BaseInteractable;

            if (interactable == null) return;

            // Check if it exists on the list of near objects
            if (_nearInteractables.ContainsKey(interactable.Id)) return;

            // Only accept same priority as the system detects
            if (_priority != interactable.Priority) return;

            _nearInteractables.Add(interactable.Id, interactable);
        }

        private void StopDetection(object[] payload)
        {
            if (!_wasInitialized) return;

            if (payload == null) return;

            if (payload.Length == 0) return;

            var interactable = payload[0] as BaseInteractable;

            if (interactable == null) return;

            // Only accept same priority as the system detects
            if (_priority != interactable.Priority) return;

            // Check if it exists on the list of near objects
            if (!_nearInteractables.ContainsKey(interactable.Id)) return;

            _nearInteractables.Remove(interactable.Id);

            // If the undetected object is the nearest then clean it
            if (_nearest.Id.Equals(interactable.Id))
            {
                _nearest = null;
            }

            GetNearestInteractable();
        }

        /// <summary>
        /// Gets the nearest interactable objects in the level and communicates if there is a new one
        /// </summary>
        private void GetNearestInteractable()
        {
            string oldNearest = (_nearest == null) ? "No element" : _nearest.Id;

            bool hasChanged = false;

            // If the list of near elements is empty clean the nearest
            if (_nearInteractables.Count == 0)
            {
                _nearest = null;
                hasChanged = true;
            }
            else
            {
                // Check if the current list of interactables has a new element that should be the new nearest based on priority and distance
                CalculateNearest(oldNearest);

                if (!string.IsNullOrEmpty(oldNearest) && _nearest == null)
                {
                    hasChanged = true;  // loses detection
                }
                else if (!_nearest.Id.Equals(oldNearest))
                {
                    hasChanged = true;
                }
            }

            if (!hasChanged) return;

            var payload = new object[] { _nearest, _priority };

            GameLevelEvent.Trigger(GameLevelEventType.InteractableChanged, payload);

            Debug.Log($"{_priority} - <color=yellow>Object detected: </color>{((_nearest == null) ? "No element" : _nearest.Id)}, previous nearest: {oldNearest}", this);
        }

        private void CalculateNearest(string currentId)
        {
            _distanceToCharacter = float.MaxValue;

            List<string> toClean = new List<string>();

            var playerCharacterPosition = _character.position;
            float playerY = playerCharacterPosition.y;

            foreach (var item in _nearInteractables)
            {
                var element = item.Value;

                if (element == null)
                {
                    toClean.Add(item.Key);
                    continue;
                }

                // Check distance to player
                var targetPosition = element.transform.position;
                targetPosition.y = playerY;

                var distance = (targetPosition - playerCharacterPosition).magnitude;

                if (distance > _distanceToCharacter) continue;

                // Update min distance
                _distanceToCharacter = distance;

                // Update nearest
                _nearest = element;
            }

            if (toClean.Count == 0) return;

            CleanObsoletes(toClean.ToArray());
        }

        /// <summary>
        /// Removes all elements with a null value from the dictionary of near elements
        /// </summary>
        /// <param name="toClean"></param>
        private void CleanObsoletes(string[] toClean)
        {
            foreach (var id in toClean)
            {
                if (!_nearInteractables.ContainsKey(id)) continue;

                _nearInteractables.Remove(id);
            }
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

                case GameLevelEventType.InteractableStartDetection:
                    StartDetection(eventData.Payload);
                    break;

                case GameLevelEventType.InteractableStopDetection:
                    StopDetection(eventData.Payload);
                    break;
            }
        }

        #endregion
    }
}