using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core;
using DudeRescueSquad.Core.Inventory.View;

namespace DudeResqueSquad.UI
{
    public class PlayerInteractableIndicator : MonoBehaviour, IGameEventListener<InteractableEvent>
    {
        // TODO: this struct should be in another file (scriptable object would be better) 
        // related with game colors configuration
        [Serializable]
        public struct InteractablePriorityColor
        {
            public Enums.InteractablePriorities priority;
            public Color color;
        }
        
        #region Inspector properties

        [SerializeField] private Transform _playerTarget = default; // TODO: this could be injected from some game level initialization
        [SerializeField] private Image _img = default;
        [SerializeField] private InteractablePriorityColor[] _priorityColors = default;

        #endregion

        #region Private properties

        private Transform _originalParent;
        private Canvas _canvas = null;
        private Transform _cacheTransform = default;
        private Transform _currentTarget = default;
        private Enums.InteractablePriorities _currentPriority = Enums.InteractablePriorities.NONE;
        private Transform _currentInteractable = default;
        private Dictionary<Enums.InteractablePriorities, List<Transform>> _interactablesByPriority = default;
        private Dictionary<Enums.InteractablePriorities, int> _priorityValues = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            this.EventStartListening<InteractableEvent>();

            InitializePriorityValues();

            _canvas = GetComponent<Canvas>();

            _cacheTransform = transform;

            _originalParent = _cacheTransform.parent;

            Hide();

            LoadAllInteractables();

            GameEvents.OnDetectInteractable += InteractableDetected;
            GameEvents.OnStopDetectingIteractable += InteractableLost;
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
            this.EventStopListening<InteractableEvent>();

            GameEvents.OnDetectInteractable -= InteractableDetected;
            GameEvents.OnStopDetectingIteractable -= InteractableLost;
        }

        #endregion

        #region Private methods

        /// <summary>
        /// Gets list of priorities ordered by enum value based on the Enum definition
        /// </summary>
        private void InitializePriorityValues()
        {
            var temp = new Dictionary<Enums.InteractablePriorities, int>();

            foreach (int value in Enum.GetValues(typeof(Enums.InteractablePriorities)))
            {
                var prio = (Enums.InteractablePriorities)value;

                // Skip NONE value
                if (prio == Enums.InteractablePriorities.NONE) continue;

                temp.Add(prio, value);
            }

            _priorityValues = new Dictionary<Enums.InteractablePriorities, int>(temp.Count);

            foreach (var item in temp.OrderByDescending(key => key.Value))
            {
                _priorityValues.Add(item.Key, item.Value);
            }
        }

        private void LoadAllInteractables()
        {
            _interactablesByPriority = new Dictionary<Enums.InteractablePriorities, List<Transform>>();

            var interactablesOnLevel = FindObjectsOfType<BaseInteractable>();

            foreach (var item in interactablesOnLevel)
            {
                var priority = item.Priority;

                if (!_interactablesByPriority.ContainsKey(priority))
                {
                    _interactablesByPriority.Add(priority, new List<Transform>());
                }

                _interactablesByPriority[priority].Add(item.transform);
            }
        }

        private void InteractableDetected(object sender, CustomEventArgs.InteractableArgs e)
        {
            var interactable = e.interactable;

            var priority = interactable.Priority;

            // Add to interactable list
            //AddToList(priority, e.interactableTransform);

            if (priority < _currentPriority) return;

            if (_interactablesByPriority[priority] == null)
            {
                Hide();
                return;
            }

            // Check proximity from all elements in the list
            var nearest = GetNearest(_interactablesByPriority[priority]);

            _currentPriority = priority;

            _currentInteractable = nearest;

            Show(nearest);
        }

        private void InteractableLost(object sender, CustomEventArgs.InteractableArgs e)
        {
            // Check if there isn't any interactable in the list then hide the indicator
            if (!RemainingInteractables())
            {
                Hide();

                return;
            }

            // Show next interactable
            ShowNextInteractable();
        }

        /// <summary>
        /// Checks if there is at least one interactable on any list of priorities or all were already removed
        /// </summary>
        /// <returns></returns>
        private bool RemainingInteractables()
        {
            foreach (var item in _interactablesByPriority)
            {
                var list = item.Value;

                if (list.Count > 0)
                    return true;
            }

            InteractablesController.OnStopDetection?.Invoke();

            return false;
        }

        /// <summary>
        /// From the remaining interactables, order them by priority and get the nearest.
        /// Mark it as the current one
        /// </summary>
        private void ShowNextInteractable()
        {
            // Check by priority first then show the nearest one from the selected category
            var list = GetRemainingByPriority(out var priority);

            if (list == null)
            {
                Hide();
                return;
            }

            // Check proximity from all elements in the list
            var nearest = GetNearest(list);

            if (nearest == null)
            {
                Hide();
                return;
            }

            _currentPriority = priority;

            _currentInteractable = nearest;

            Show(nearest);
        }

        private List<Transform> GetRemainingByPriority(out Enums.InteractablePriorities priority)
        {
            priority = Enums.InteractablePriorities.NONE;

            foreach (var item in _priorityValues)
            {
                var prio = item.Key;

                if (_interactablesByPriority.ContainsKey(prio))
                {
                    if (_interactablesByPriority[prio].Count > 0)
                    {
                        priority = prio;
                        return _interactablesByPriority[prio];
                    }
                }
            }

            return null;

            /*if (_interactablesByPriority.ContainsKey(Enums.InteractablePriorities.ENEMY))
            {
                if (_interactablesByPriority[Enums.InteractablePriorities.ENEMY].Count > 0)
                {
                    priority = Enums.InteractablePriorities.ENEMY;
                }
            }
            else if (_interactablesByPriority.ContainsKey(Enums.InteractablePriorities.INTERACTABLE_ELEMENT))
            {
                if (_interactablesByPriority[Enums.InteractablePriorities.INTERACTABLE_ELEMENT].Count > 0)
                {
                    priority = Enums.InteractablePriorities.INTERACTABLE_ELEMENT;
                }
            }
            else if (_interactablesByPriority.ContainsKey(Enums.InteractablePriorities.PICKABLE_ITEM))
            {
                if (_interactablesByPriority[Enums.InteractablePriorities.PICKABLE_ITEM].Count > 0)
                {
                    priority = Enums.InteractablePriorities.PICKABLE_ITEM;
                }
            }

            return (priority == Enums.InteractablePriorities.NONE) ? null : _interactablesByPriority[priority];*/
        }

        /// <summary>
        /// Gets the nearest element to the player position based on a list of elements to check from
        /// </summary>
        /// <param name="elements"></param>
        /// <returns></returns>
        private Transform GetNearest(List<Transform> elements)
        {
            float currentDist = float.MaxValue;
            Vector3 playerPosition = _playerTarget.position;
            float playerY = playerPosition.y;

            Transform target = null;

            int amountOfTargets = elements.Count;

            for (int i = 0; i < amountOfTargets; i++)
            {
                var possibleTarget = elements[i];

                if (possibleTarget == null) continue;

                var targetPosition = possibleTarget.position;
                targetPosition.y = playerY;

                float dist = (targetPosition - playerPosition).magnitude;

                // TODO: cache the BaseInteractable component
                float minDistance = possibleTarget.GetComponent<BaseInteractable>().DistanceToBeDetected;

                if (dist <= minDistance && dist < currentDist)
                {
                    target = possibleTarget;
                    currentDist = dist;
                }
            }

            //Remove nulls from elements list
            CleanList(elements);

            // If there is no current target then stop detection
            if (target == null && _currentTarget != null)
            {
                _currentTarget = null;
                InteractablesController.OnStopDetection?.Invoke();
            }

            return target;
        }

        private void DebugShowElements(List<Transform> elements)
        {
            string message = string.Empty;
            foreach (var item in elements)
            {
                if (item == null) continue;
                message += $"- {item.name}";
            }

            Debug.Log($"Elements to check distance: {message}");
        }

        /// <summary>
        /// Removes all uneeded elements or that were consumed, died or whatever reason that they don't shouldn't exist on the list anymore
        /// </summary>
        private void CleanList(List<Transform> elements)
        {
            var i = 0;

            while (i < elements.Count)
            {
                var element = elements[i];

                if (element == null)
                {
                    elements.RemoveAt(i);
                    continue;
                }
               
                i++;
            }
        }

        private void RemoveFromList(Enums.InteractablePriorities priority, Transform transform)
        {
            if (!_interactablesByPriority.ContainsKey(priority)) return;

            _interactablesByPriority[priority].Remove(transform);
        }

        private void Hide()
        {
            _canvas.enabled = false;
            _cacheTransform.SetParent(_originalParent);
            _currentPriority = Enums.InteractablePriorities.NONE;
            _currentInteractable = null;
        }

        private void Show(Transform target)
        {
            if (target == null)
            {
                ShowNextInteractable();

                return;
            }

            _cacheTransform.position = new Vector3(target.position.x, 0.4f, target.position.z);
            _cacheTransform.localRotation = Quaternion.Euler(90, 0, 0);
            _cacheTransform.localScale = Vector3.one * 0.001f;

            SetColor();

            if (!_canvas.enabled)
            {
                _canvas.enabled = true;
            }

            if (target != _currentTarget)
            {
                InteractablesController.OnDetect?.Invoke(target.GetComponent<BaseInteractable>());

                _currentTarget = target;
            }
        }

        private void SetColor()
        {
            var colorByPriority = _priorityColors.FirstOrDefault(x => x.priority == _currentPriority);

            _img.color = colorByPriority.color;
        }

        private void PickedItem(string itemId)
        {
            var item = GetItem(itemId);

            var interactable = _currentInteractable.GetComponent<IInteractable>();

            // Remove it from the list
            if (item != null)
            {
                RemoveFromList(interactable.Priority, item);
            }

            // Check if there isn't any interactable in the list then hide the indicator
            if (!RemainingInteractables())
            {
                Hide();

                return;
            }

            // Show next interactable
            ShowNextInteractable();
        }

        private Transform GetItem(string itemId)
        {
            foreach (var item in _interactablesByPriority[Enums.InteractablePriorities.PICKABLE_ITEM])
            {
                var pickable = item.GetComponent<ViewItemPicker>();

                if (pickable == null) continue;

                if (!pickable.ItemId.Equals(itemId)) continue;

                return item;
            }

            return null;
        }

        private void EnemyDead(Transform element)
        {
            if (element == null) return;

            // If there isn't any interactable detected skip this
            if (_currentInteractable == null) return;

            var interactable = element.GetComponent<IInteractable>();

            // Remove it from the list
            RemoveFromList(interactable.Priority, element);

            // Check if there isn't any interactable in the list then hide the indicator
            if (!RemainingInteractables())
            {
                Hide();

                return;
            }

            // Show next interactable
            ShowNextInteractable();
        }

        #endregion

        #region IGameEventListener implementation

        /// <summary>
        /// Watches for different events like Interactable Pick Item events
        /// </summary>
        /// <param name="eventData">Interactable event.</param>
        public virtual void OnGameEvent(InteractableEvent eventData)
        {
            switch (eventData.EventType)
            {
                case InteractableEventType.EnemyDead:
                    EnemyDead(eventData.Element);
                    break;

                case InteractableEventType.PickItem:
                    PickedItem(eventData.ItemId);
                    break;
            }
        }

        #endregion
    }
}