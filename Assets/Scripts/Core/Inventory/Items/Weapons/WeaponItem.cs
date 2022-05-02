using UnityEngine;

namespace DudeRescueSquad.Core.Inventory.Items.Weapons
{
    /// <summary>
    /// Class in charge of weapon behaviour like assault, melee, throwable objects.
    /// When a weapon is equipped then it should be aware of target detection based on its parameters
    /// </summary>
    public class WeaponItem : Item
    {
        public string InstanceId { get => _instanceId; }
        public string TemplateId { get => _templateId; }
        public Enums.ItemTypes Type { get => _type; }

        protected string _instanceId = string.Empty;
        protected string _templateId = string.Empty;
        protected Enums.ItemTypes _type = Enums.ItemTypes.NONE;

        private Transform _target = null;
        private bool _targetDetected = false;

        public bool HasDetectedTarget()
        {
            return _targetDetected;
        }

        public Vector3 GetTargetPosition()
        {
            // Check how to detect the nearest target and returns its position
            if (!_targetDetected) return Vector3.zero;

            return _target.position;
        }

        public void SetTarget(Transform target)
        {
            _target = target;
            _targetDetected = true;
        }

        public void CleanTarget()
        {
            _target = null;
            _targetDetected = false;
        }
    }
}