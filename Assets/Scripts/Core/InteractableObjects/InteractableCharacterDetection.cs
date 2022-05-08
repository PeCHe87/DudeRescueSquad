
using DudeResqueSquad;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    /// <summary>
    /// Logic to detect which interactable objects are the nearest to the player based on configuration coming from an scriptable object
    /// It tries to find an interactable of each type (enemy, item picker, npc, door, etc) near to the character and communicates when something new is detected or undetected
    /// The configuration for each search comes from a scriptable object to have a dynamic way to modify values for each searching individually
    /// The concept of nearest is defined for the object that is on the view angle in front of the character (angle defined on config) and nearest in distance
    /// The priority is about to be in the character field of view, if not, then nearest distance will define if there is something detected.
    /// It is in charge of communicate to the Item Picker button when to show or hide based on item picker detection and distance
    /// </summary>
    public class InteractableCharacterDetection : MonoBehaviour
    {
        #region Events

        public static event System.Action<Transform, Enums.InteractablePriorities> OnDetected;
        public static event System.Action<Enums.InteractablePriorities> OnUndetected;
        public static event System.Action<string> OnShowButton;
        public static event System.Action OnHideButton;

        #endregion

        #region Inspector properties

        [Tooltip("Transform that represents the model of the entity that rotates and be used to detect interactables at forward direction")]
        [SerializeField] private Transform _entityOrientation = default;

        [Header("Configuration for Item Pickers")]   // TODO: all these configuration fields should be coming from a scriptable object in the future
        [SerializeField] private float _delayToDetect = 0.5f;
        [SerializeField] private float _angleDetectionItemPickers = 360;
        [SerializeField] private float _radiusDetectionItemPickers = 20;
        [SerializeField] private float _angleFieldOfViewItemPickers = 90;
        [Header("Configuration for Enemies")]        // TODO: all these configuration fields should be coming from a scriptable object in the future
        [SerializeField] private float _angleDetectionEnemies = 360;
        [SerializeField] private float _radiusDetectionEnemies = 20;
        [SerializeField] private float _angleFieldOfViewEnemies = 120;

        [Header("Item pickers")]
        [Tooltip("List of all 'Item Pickers' on the level that should be check on the detection area")]
        [SerializeField] private PickerItemInteractable[] _itemPickers = default;
        [Tooltip("Layer mask to understand which are the 'item pickers' on the level")]
        [SerializeField] private LayerMask _itemPickersLayerMask = default;
        [SerializeField] private BaseInteractable _detectedItemPicker = default;

        [Header("Enemies")]
        [Tooltip("List of all 'Enemies' on the level that should be check on the detection area")]
        [SerializeField] private Entity[] _enemies = default;
        [Tooltip("Layer mask to understand which are the 'enemies' on the level")]
        [SerializeField] private LayerMask _enemiesLayerMask = default;
        [SerializeField] private Entity _detectedEnemy = default;

        #endregion

        #region Private properties

        private float _timeToDetect = 0;
        private List<PickerItemInteractable> _detectedItemPickers = default;
        private List<Entity> _detectedEnemies = default;

        #endregion

        #region Unity Events

        private void Start()
        {
            _detectedItemPickers = new List<PickerItemInteractable>();

            _detectedEnemies = new List<Entity>();
        }

        private void Update()
        {
            if (_timeToDetect == 0)
            {
                Detect();
            }
            else
            {
                _timeToDetect = Mathf.Clamp(_timeToDetect - Time.deltaTime, 0, _delayToDetect);
            }

            if (_canCheckPickerDistance)
            {
                CheckPickerDistance();
            }
        }

        #endregion

        #region Private methods

        private void Detect()
        {
            // Detect item picker
            DetectNearestItemPicker();

            // Detect enemy
            DetectNearestEnemy();

            // TODO: detect exit door

            _timeToDetect = _delayToDetect;
        }

        #endregion

        #region Item Picker methods

        private bool _canCheckPickerDistance = false;
        private bool _showingPickerButton = false;

        public void DetectNearestItemPicker()
        {
            _detectedItemPickers.Clear();

            var list = new List<( float angle, float distance, PickerItemInteractable picker)>();
            var characterPosition = _entityOrientation.position;

            for (int i = 0; i < _itemPickers.Length; i++)
            {
                var itemPicker = _itemPickers[i];

                // Check if picker was already picked
                if (itemPicker.WasPicked) continue;

                if (CheckItemPickerOnDetectionArea(_entityOrientation, itemPicker.transform, _angleDetectionItemPickers, _radiusDetectionItemPickers, out var entityAngle))
                {
                    var distanceToPlayer = Vector3.Distance(characterPosition, itemPicker.transform.position);
                    list.Add((
                        angle: entityAngle, 
                        distance: distanceToPlayer, 
                        picker: itemPicker)
                    );
                }
            }

            if (list.Count > 0)
            {
                PickerItemInteractable nearest = list[0].picker;

                var orderedByDistance = list.OrderBy(x => x.distance).ToArray< (float angle, float distance, PickerItemInteractable picker)> ();

                nearest = orderedByDistance[0].picker;

                if (_detectedItemPicker != null && nearest.Id.Equals(_detectedItemPicker.Id)) return;

                _detectedItemPicker = nearest;

                _canCheckPickerDistance = true;

                CommunicateToPickerButton();

                OnDetected?.Invoke(nearest.transform, Enums.InteractablePriorities.PICKABLE_ITEM);
            }
            else
            {
                if (_detectedItemPicker == null) return;

                _detectedItemPicker = null;

                _canCheckPickerDistance = false;
                _showingPickerButton = false;

                CommunicateToPickerButton();

                OnUndetected?.Invoke(Enums.InteractablePriorities.PICKABLE_ITEM);
            }
        }

        /// <summary>
        /// Check if item picker is on detection area and returns its angle from character forward direction
        /// </summary>
        ///< param name = "character" > character < / param >
        ///< param name = "itemPicker" > item picker < / param >
        ///< param name = "angle" > sector angle < / param >
        ///< param name = "radius" > sector radius < / param >
        /// <returns></returns>
        private bool CheckItemPickerOnDetectionArea(Transform character, Transform itemPicker, float angle, float radius, out float directionAngle)
        {
            Vector3 direction = itemPicker.position - character.position;

            Vector3 itemPickerPosition = itemPicker.position + Vector3.up * 0.5f;

            // Mathf.Rad2Deg  : radian value to degree conversion constant
            // Mathf.Acos (f) : returns the arccosine value of parameter F
            directionAngle = Mathf.Acos(Vector3.Dot(direction.normalized, character.forward)) * Mathf.Rad2Deg;

            if (directionAngle < angle * 0.5f && direction.magnitude < radius) return true;

            return false;
        }

        private void CheckPickerDistance()
        {
            if (_detectedItemPicker == null)
            {
                return;
            }

            var distance = Vector3.Distance(_detectedItemPicker.transform.position, _entityOrientation.position);

            var canShow = (distance <= _detectedItemPicker.DistanceToBeDetected);

            if (canShow == _showingPickerButton) return;

            _showingPickerButton = canShow;

            CommunicateToPickerButton();
        }

        private void CommunicateToPickerButton()
        {
            if (_showingPickerButton)
            {
                OnShowButton?.Invoke(_detectedItemPicker.Id);
            }
            else
            {
                OnHideButton?.Invoke();
            }
        }

        #endregion

        #region Enemies detection methods

        public void DetectNearestEnemy()
        {
            _detectedEnemies.Clear();

            var list = new List<(float angle, float distance, Entity enemy)>();
            var characterPosition = _entityOrientation.position;

            for (int i = 0; i < _enemies.Length; i++)
            {
                var enemy = _enemies[i];

                // Check if enemy is alive
                if (enemy.IsDead) continue;

                if (CheckEnemyOnDetectionArea(_entityOrientation, enemy.transform, _angleDetectionEnemies, _radiusDetectionEnemies, out var entityAngle))
                {
                    var distanceToPlayer = Vector3.Distance(characterPosition, enemy.transform.position);
                    list.Add((angle: entityAngle, distance: distanceToPlayer, enemy: enemy));
                }
            }

            if (list.Count > 0)
            {
                var minDistance = float.MaxValue;
                var minAngle = float.MaxValue;
                Entity nearest = list[0].enemy;

                var orderedByDistance = list.OrderBy(x => x.distance).ToArray<(float angle, float distance, Entity enemy)>();

                for (int i = 0; i < orderedByDistance.Length; i++)
                {
                    var value = orderedByDistance[i];

                    if (value.angle <= _angleFieldOfViewEnemies && value.angle < minAngle)
                    {
                        nearest = value.enemy;
                        minAngle = value.angle;
                        minDistance = value.distance;
                    }

                    //Debug.LogError($"Picker <color=yellow>'{value.enemy.UID}'</color>, distance: {value.distance}, angle to player: {value.angle}");
                }

                if (_detectedEnemy != null && nearest.UID.Equals(_detectedEnemy.UID)) return;

                _detectedEnemy = nearest;

                //Debug.LogError($"<color=green>Detect</color> item picker: {nearest.UID}");

                OnDetected?.Invoke(nearest.transform, Enums.InteractablePriorities.ENEMY);
            }
            else
            {
                if (_detectedEnemy == null) return;

                //Debug.LogError($"<color=red>Undetect</color> item picker {_detectedEnemy.UID}");

                _detectedEnemy = null;

                OnUndetected?.Invoke(Enums.InteractablePriorities.ENEMY);
            }
        }

        /// <summary>
        /// Check if enemy is on detection area and returns its angle from character forward direction
        /// </summary>
        ///< param name = "character" > character < / param >
        ///< param name = "enemy" > enemy < / param >
        ///< param name = "angle" > sector angle < / param >
        ///< param name = "radius" > sector radius < / param >
        /// <returns></returns>
        private bool CheckEnemyOnDetectionArea(Transform character, Transform enemy, float angle, float radius, out float directionAngle)
        {
            Vector3 direction = enemy.position - character.position;

            Vector3 enemyPosition = enemy.position + Vector3.up * 0.5f;
            Vector3 characterPosition = character.position + Vector3.up * 0.5f;

            // Check if there is a direct raycast from character towards enemy, else is not detected
            if (Physics.Raycast(enemyPosition, direction, direction.magnitude, _enemiesLayerMask))
            {
                directionAngle = 0;
                return false;
            }

            // Mathf.Rad2Deg  : radian value to degree conversion constant
            // Mathf.Acos (f) : returns the arccosine value of parameter F
            directionAngle = Mathf.Acos(Vector3.Dot(direction.normalized, character.forward)) * Mathf.Rad2Deg;

            if (directionAngle < angle * 0.5f && direction.magnitude < radius) return true;

            return false;
        }

        #endregion
    }
}