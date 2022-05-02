using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    public class WeaponAttackAreaDebugger : MonoBehaviour, IGameEventListener<InventoryEvent>
    {
        #region Inspector properties

        [SerializeField] private Transform _parent = default;
        [SerializeField] private Color _colorAttackArea = Color.red;
        [SerializeField] private string _shaderType = "Transparent/Diffuse";
        [SerializeField] private Transform[] _entities; // under attack
        [SerializeField] private LayerMask _layerObstacleMask = default;
        [SerializeField] private bool _canDebug = false;

        #endregion

        #region Private properties

        private float _range = 3;
        private float _detectionOffset = 1;
        private float _radiusDetection = 4;
        private float _angle = 60;
        private GameObject _attackArea = default;
        private MeshFilter _meshFilter = default;
        private MeshRenderer _meshRenderer = default;
        private Shader _shader = default;
        private bool _hasWeaponEquipped = false;

        #endregion

        #region Unity Events

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

        private void Update()
        {
            if (!_hasWeaponEquipped) return;

            for (int i = 0; i < _entities.Length; i++)
            {
                var entity = _entities[i];

                if (CheckUnderAttackArea(_parent, entity, _angle, _radiusDetection, out var entityAngle))
                {
                    if (_canDebug)
                    {
                        Debug.LogError($"Attacked: {entity.name} - angle:<color=cyan> {entityAngle}</color>");
                    }
                }
            }
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
        private bool CheckUnderAttackArea(Transform attacker, Transform attacked, float angle, float radius, out float attackedAngle)
        {
            Vector3 direction = attacked.position - attacker.position;

            Vector3 attackerPosition = attacker.position + Vector3.up * 0.5f;
            Vector3 attackedPosition = attacked.position + Vector3.up * 0.5f;

            // Check if there is a direct raycast from attacker towards entity, else is not under attack
            if (Physics.Raycast(attackerPosition, direction, direction.magnitude, _layerObstacleMask))
            {
                if (_canDebug)
                {
                    Debug.DrawLine(attackerPosition, attackedPosition, Color.red);
                }
                attackedAngle = 0;
                return false;
            }
            else
            {
                if (_canDebug)
                {
                    Debug.DrawLine(attackerPosition, attackedPosition, Color.green);
                }
            }


            // Mathf.Rad2Deg  : radian value to degree conversion constant
            // Mathf.Acos (f) : returns the arccosine value of parameter F
            attackedAngle = Mathf.Acos(Vector3.Dot(direction.normalized, attacker.forward)) * Mathf.Rad2Deg;

            if (attackedAngle < angle * 0.5f && direction.magnitude < radius) return true;

            return false;
        }

        private IEnumerator DrawAttackArea(Transform t, Vector3 center, float angle, float radius)
        {
            yield return new WaitForEndOfFrame();

            int pointAmmount = 100;
            float eachAngle = angle / pointAmmount;

            Vector3 forward = t.forward;
            List<Vector3> vertices = new List<Vector3>();

            vertices.Add(center);
            for (int i = 0; i < pointAmmount; i++)
            {
                Vector3 pos = Quaternion.Euler(0f, -angle / 2 + eachAngle * (i - 1), 0f) * forward * radius + center;
                vertices.Add(pos);
            }
            CreateMesh(vertices);
        }

        private void CreateMesh(List<Vector3> vertices)
        {
            int[] triangles;
            Mesh mesh = new Mesh();

            int triangleAmount = vertices.Count - 2;
            triangles = new int[3 * triangleAmount];

            //According to the number of triangles, the vertex order of the triangle is calculated
            for (int i = 0; i < triangleAmount; i++)
            {
                triangles[3 * i] = 0;
                triangles[3 * i + 1] = i + 1;
                triangles[3 * i + 2] = i + 2;
            }

            if (_attackArea == null)
            {
                _attackArea = new GameObject("mesh");
                _attackArea.transform.position = new Vector3(0f, 0.3f, -0.25f);

                _meshFilter = _attackArea.AddComponent<MeshFilter>();
                _meshRenderer = _attackArea.AddComponent<MeshRenderer>();
            }

            _shader = Shader.Find(_shaderType);

            mesh.vertices = vertices.ToArray();
            mesh.triangles = triangles;

            _meshFilter.mesh = mesh;
            _meshRenderer.material.shader = _shader;
            _meshRenderer.material.color = _colorAttackArea;

            _attackArea.transform.SetParent(_parent);
        }

        private void EquipWeapon(string itemId)
        {
            var itemData = InventoryCatalogItems.GetItemDataById(itemId);

            if (itemData.data.Type == WeaponType.NONE) return;

            if (_attackArea != null)
            {
                Destroy(_attackArea);
            }

            _angle = itemData.data.AngleView;
            _range = itemData.data.Range;
            _radiusDetection = _range + _detectionOffset;

            StartCoroutine(DrawAttackArea(_parent, transform.localPosition, _angle, _range));
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