using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory.View;
using DudeResqueSquad;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class InteractablesController : MonoBehaviour
    {
        [SerializeField] private Transform _playerCharacter = default;  // TODO: should be injected by game level initialization
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

            if (_canDebug)
                Debug.Log("<b> ---------- </b>");

            float minDistance = float.MaxValue;

            foreach (var element in _interactables)
            {
                if (element == null) continue;

                var distance = (element.transform.position - playerCharacterPosition).magnitude;

                if (_canDebug)
                    Debug.Log($"Distance to element {element.name} is {distance} and area detection is {element.AreaRadiusDetection}");

                if (distance <= element.AreaRadiusDetection && distance < minDistance)
                {
                    minDistance = distance;
                    element.Detect();
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