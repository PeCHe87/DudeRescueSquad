using DudeRescueSquad.Core.Inventory.View;
using DudeResqueSquad;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class InteractablesController : MonoBehaviour
    {
        public static System.Action<BaseInteractable> OnDetect;
        public static System.Action OnStopDetection;

        [SerializeField] private Transform _playerCharacter = default;  // TODO: should be injected by game level initialization  when level initialization was created
        [SerializeField] private bool _canDebug = false;

        private List<BaseInteractable> _interactables = default;
        private bool _wasInitialized = false;

        #region Unity events

        private void Start()
        {
            Initialization();
        }

        private void Update()
        {
            if (!_wasInitialized) return;

            CheckProximity();
        }

        #endregion

        #region Private methods

        private void Initialization()
        {
            _interactables = FindObjectsOfType<BaseInteractable>().ToList();
            _wasInitialized = true;
        }

        private void CheckProximity()
        {
            var playerCharacterPosition = _playerCharacter.position;
            float playerY = playerCharacterPosition.y;

            if (_canDebug)
                Debug.Log("<b> ---------- </b>");

            float minDistance = float.MaxValue;

            // IMPROVEMENT: could be great have a list of elements separated by priority and ordered by distance. After each checking only mark as detected the nearest from the max priority.

            foreach (var element in _interactables)
            {
                if (element == null) continue;

                var targetPosition = element.transform.position;
                targetPosition.y = playerY;

                var distance = (targetPosition - playerCharacterPosition).magnitude;

                if (_canDebug)
                    Debug.Log($"Distance to element {element.name} is {distance} and area detection is {element.AreaRadiusDetection}");

                if (distance <= element.AreaRadiusDetection && distance < minDistance)
                {
                    minDistance = distance;
                    element.Detect();
                    //nearest = element;
                }
                else
                {
                    element.StopDetection();
                }
            }

            CleanList();
        }

        /// <summary>
        /// Removes all uneeded elements or that were consumed, died or whatever reason that they don't shouldn't exist on the list anymore
        /// </summary>
        private void CleanList()
        {
            var i = 0;

            while (i < _interactables.Count)
            {
                var element = _interactables[i];

                if (element == null)
                {
                    _interactables.RemoveAt(i);
                    continue;
                }

                if (element.Priority == Enums.InteractablePriorities.ENEMY)
                {
                    var enemy = element.GetComponent<IDamageable>();

                    if (enemy.IsDead)
                    {
                        _interactables.RemoveAt(i);
                        continue;
                    }
                }

                if (element.Priority == Enums.InteractablePriorities.PICKABLE_ITEM)
                {
                    var picker = element.GetComponent<ViewItemPicker>();

                    if (picker.WasPicked)
                    {
                        _interactables.RemoveAt(i);
                        continue;
                    }
                }

                i++;
            }
        }

        #endregion
    }
}