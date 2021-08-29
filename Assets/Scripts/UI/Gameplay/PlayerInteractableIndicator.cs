using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.Inventory.View;
using DudeRescueSquad.Core;

namespace DudeResqueSquad.UI
{
    public class PlayerInteractableIndicator : MonoBehaviour, IGameEventListener<InteractableEvent>
    {
        // TODO: this struct should be in another file (scriptable object woul be better) 
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
        private Enums.InteractablePriorities _currentPriority = Enums.InteractablePriorities.NONE;
        private Transform _currentInteractable = default;
        private Dictionary<Enums.InteractablePriorities, List<Transform>> _interactablesByPriority = default;
        private Dictionary<Enums.InteractablePriorities, int> _priorityValues = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            InitializePriorityValues();

            _canvas = GetComponent<Canvas>();

            _cacheTransform = transform;

            _originalParent = _cacheTransform.parent;

            Hide();

            _interactablesByPriority = new Dictionary<Enums.InteractablePriorities, List<Transform>>();

            GameEvents.OnDetectInteractable += InteractableDetected;
            GameEvents.OnStopDetectingIteractable += InteractableLost;
        }

        private void OnDestroy()
        {
            GameEvents.OnDetectInteractable += InteractableDetected;
            GameEvents.OnStopDetectingIteractable += InteractableLost;
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

        private void InteractableDetected(object sender, CustomEventArgs.InteractableArgs e)
        {
            var interactable = e.interactable;

            var priority = interactable.Priority;

            // Add to interactable list
            AddToList(priority, e.interactableTransform);

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

        private void AddToList(Enums.InteractablePriorities priority, Transform transform)
        {
            if (_interactablesByPriority.ContainsKey(priority))
            {
                if (_interactablesByPriority[priority].Contains(transform)) return;

                _interactablesByPriority[priority].Add(transform);
            }
            else
            {
                var list = new List<Transform>();
                list.Add(transform);
                _interactablesByPriority.Add(priority, list);
            }
        }

        private void InteractableLost(object sender, CustomEventArgs.InteractableArgs e)
        {
            // Remove it from the list
            RemoveFromList(e.interactable.Priority, e.interactableTransform);

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
            Transform currentTarget = null;

            int amountOfTargets = elements.Count;

            for (int i = 0; i < amountOfTargets; i++)
            {
                var possibleTarget = elements[i];

                if (possibleTarget == null) continue;

                float dist = (possibleTarget.position - playerPosition).magnitude;

                if (dist < currentDist)
                {
                    currentTarget = possibleTarget;
                    currentDist = dist;
                }
            }

            //Remove nulls from elements list
            CleanList(elements);

            return currentTarget;
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
        }

        private void SetColor()
        {
            var colorByPriority = _priorityColors.FirstOrDefault(x => x.priority == _currentPriority);

            _img.color = colorByPriority.color;
        }

        private void PickedItem(string itemId)
        {
            // If there isn't any interactable detected skip this
            if (_currentInteractable == null) return;

            var interactable = _currentInteractable.GetComponent<IInteractable>();

            // Remove it from the list
            RemoveFromList(interactable.Priority, _currentInteractable);

            // Check if there isn't any interactable in the list then hide the indicator
            if (!RemainingInteractables())
            {
                Hide();
                return;
            }

            // Show next interactable
            ShowNextInteractable();
        }

        private void EnemyDead(Transform element)
        {
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