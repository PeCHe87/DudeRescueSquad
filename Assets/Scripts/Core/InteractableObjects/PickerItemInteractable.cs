using DudeRescueSquad.Core.Inventory.View;
using DudeRescueSquad.Core.Weapons;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class PickerItemInteractable : BaseInteractable
    {
        #region Inspector properties

        [SerializeReference] private BaseWeaponDefinition _weaponDefinition = default;
        [SerializeField] private string _id = default;
        [SerializeField] private int _distanceToBeDetected = 5;

        #endregion

        #region Private properties

        private bool _wasDetected = false;
        private ViewItemPicker _picker = default;
        private Vector3 _positionToCompare = default;
        private bool _wasPicked = false;

        #endregion

        #region Public properties

        public override string Id => _id;
        public override float DistanceToBeDetected => _distanceToBeDetected;
        public override string DisplayName => _weaponDefinition.DisplayName;
        public bool WasPicked => _wasPicked;

        #endregion

        #region Unity events

        private void Awake()
        {
            _picker = GetComponentInChildren<ViewItemPicker>();
            _picker.OnPicked += Picked;
        }

        private void OnDestroy()
        {
            _picker.OnPicked -= Picked;
        }

        private void OnTriggerEnter(Collider other)
        {
            if (!other.tag.Equals("Player")) return;

            Detect();
        }

        private void OnTriggerExit(Collider other)
        {
            if (!other.tag.Equals("Player")) return;

            StopDetection();
        }

        #endregion

        #region Private methods

        private void Picked()
        {
            _wasPicked = true;

            StopDetection();

            InteractableEvent.Trigger(InteractableEventType.PickItem, _picker.ItemId);
        }

        #endregion

        #region BaseInteractable implementation

        public override Enums.InteractablePriorities Priority => Enums.InteractablePriorities.PICKABLE_ITEM;

        public override float AreaRadiusDetection => _distanceToBeDetected;

        public override void Detect()
        {
            if (_picker.WasPicked) return;

            base.Detect();
        }

        #endregion
    }
}