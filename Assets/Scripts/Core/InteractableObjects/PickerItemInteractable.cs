using DudeRescueSquad.Core.Inventory.View;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class PickerItemInteractable : BaseInteractable
    {
        [SerializeField] private int _distanceToBeDetected = 5;

        private bool _wasDetected = false;
        private ViewItemPicker _picker = default;

        private void Awake()
        {
            _picker = GetComponent<ViewItemPicker>();
            _picker.OnPicked += Picked;
        }

        private void OnDestroy()
        {
            _picker.OnPicked -= Picked;
        }

        private void Picked()
        {
            StopDetection();

            InteractableEvent.Trigger(InteractableEventType.PickItem, _picker.ItemId);
        }

        #region BaseInteractable implementation

        public override Enums.InteractablePriorities Priority => Enums.InteractablePriorities.PICKABLE_ITEM;

        public override float AreaRadiusDetection => _distanceToBeDetected;

        public override void Detect()
        {
            if (_picker.WasPicked) return;

            if (_wasDetected) return;

            base.Detect();

            _wasDetected = true;

            CustomEventArgs.InteractableArgs eventArgs = new CustomEventArgs.InteractableArgs(transform, this);

            GameEvents.OnDetectInteractable?.Invoke(this, eventArgs);
        }

        public override void StopDetection()
        {
            base.StopDetection();

            CustomEventArgs.InteractableArgs eventArgs = new CustomEventArgs.InteractableArgs(transform, this);

            GameEvents.OnStopDetectingIteractable?.Invoke(this, eventArgs);

            _wasDetected = false;
        }

        #endregion
    }
}