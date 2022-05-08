using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    /// <summary>
    /// Class in charge to help an entity to detect which targets are in range based on the equipped weapon
    /// </summary>
    public class WeaponAreaDetection : MonoBehaviour, IGameEventListener<InventoryEvent>
    {
        #region Inspector properties

        [Tooltip("Transform that represents the model of the entity that rotates and be used to detect entity forward direction")]
        [SerializeField] private Transform _entityOrientation = default;
        [Tooltip("List of all enemies on the level that should be check on the attack area")]
        [SerializeField] private Transform[] _entities; // TODO: this whould come from level manager
        [Tooltip("Layer mask to understand which are the obstacles on the level")]
        [SerializeField] private LayerMask _layerObstacleMask = default;

        #endregion

        #region Private properties

        private float _range = 3;
        private float _detectionOffset = 1;
        private float _radiusDetection = 4;
        private float _angle = 60;
        private GameObject _attackArea = default;
        private bool _hasWeaponEquipped = false;
        private List<Transform> _detectedTargets = default;

        #endregion

        #region Unity Events

        private void Start()
        {
            _detectedTargets = new List<Transform>();
        }

        /// <summary>
        /// On enable, we start listening for GameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        private void OnEnable()
        {
            this.EventStartListening<InventoryEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for GameEvents. You may want to extend that to stop listening to other types of events.
        /// </summary>
        protected void OnDisable()
        {
            this.EventStopListening<InventoryEvent>();
        }

        #endregion

        #region Public methods

        public Transform[] GetEntitiesOnArea()
        {
            if (!_hasWeaponEquipped) return null;

            _detectedTargets.Clear();

            for (int i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];

                if (CheckUnderAttackArea(_entityOrientation, entity, _angle, _radiusDetection, out var entityAngle, true))
                {
                    //Debug.LogError($"Attacked: {entity.name} - angle:<color=cyan> {entityAngle}</color>");
                    _detectedTargets.Add(entity);
                }
            }

            return _detectedTargets.ToArray();
        }

        public bool TryGetNearestTargetPRevious(out Transform nearest)
        {
            nearest = _entities[0];
            var minAngle = float.MaxValue;
            var foundTarget = false;

            for (int i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];

                var damageable = entity.GetComponent<DudeResqueSquad.IDamageable>();

                if (damageable.IsDead) continue;

                if (CheckUnderAttackArea(_entityOrientation, entity, _angle, _radiusDetection, out var entityAngle, true))
                {
                    if (entityAngle > minAngle) continue;

                    foundTarget = true;

                    nearest = entity;
                    minAngle = entityAngle;
                }
            }

            return foundTarget;
        }

        public bool TryGetNearestTarget(out Transform nearest)
        {
            nearest = _entities[0];
            var minAngle = float.MaxValue;
            var foundTarget = false;

            var targets = new List<Transform>();

            for (int i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];

                var damageable = entity.GetComponent<DudeResqueSquad.IDamageable>();

                if (damageable.IsDead) continue;

                // TODO: remove from local list all dead entities

                if (CheckUnderAttackArea(_entityOrientation, entity, _angle, _radiusDetection, out var entityAngle, false))
                {
                    if (entityAngle > minAngle) continue;

                    foundTarget = true;

                    nearest = entity;
                    minAngle = entityAngle;
                }
            }

            return foundTarget;
        }

        #endregion

        #region Private methods

        /// <summary>
        ///Fan attack range
        /// </summary>
        ///< param name = "attacker" > attacker < / param >
        ///< param name = "attacked" > attacked party < / param >
        ///< param name = "angle" > sector angle < / param >
        ///< param name = "radius" > sector radius < / param >
        /// <returns></returns>
        private bool CheckUnderAttackArea(Transform attacker, Transform attacked, float angle, float radius, out float attackedAngle, bool checkObstacles)
        {
            Vector3 direction = attacked.position - attacker.position;

            Vector3 attackerPosition = attacker.position + Vector3.up * 0.5f;

            // Check if there is a direct raycast from attacker towards entity, else is not under attack
            if (checkObstacles)
            {
                if (Physics.Raycast(attackerPosition, direction, direction.magnitude, _layerObstacleMask))
                {
                    attackedAngle = 0;
                    return false;
                }
            }

            // Mathf.Rad2Deg  : radian value to degree conversion constant
            // Mathf.Acos (f) : returns the arccosine value of parameter F
            attackedAngle = Mathf.Acos(Vector3.Dot(direction.normalized, attacker.forward)) * Mathf.Rad2Deg;

            if (attackedAngle < angle * 0.5f && direction.magnitude < radius) return true;

            return false;
        }

        private void EquipWeapon(string itemId)
        {
            var itemData = InventoryCatalogItems.GetItemDataById(itemId);

            if (itemData.data.Type == WeaponType.NONE) return;

            _angle = itemData.data.AngleView;
            _range = itemData.data.Range;
            _radiusDetection = _range + _detectionOffset;

            _hasWeaponEquipped = true;
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        /// <summary>
        /// Check different events related with game level
        /// </summary>
        /// <param name="eventData">Inventory event.</param>
        public virtual void OnGameEvent(InventoryEvent eventData)
        {
            switch (eventData.EventType)
            {
                case InventoryEventType.ItemEquipped:
                    EquipWeapon(eventData.ItemInstance.TemplateId);
                    break;
            }
        }

        #endregion
    }
}