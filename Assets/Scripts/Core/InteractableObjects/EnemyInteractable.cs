using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public class EnemyInteractable : BaseInteractable
    {
        [SerializeField] private string _id = default;
        [SerializeField] private int _distanceToBeDetected = 5;

        private bool _wasDetected = false;
        private IDamageable _damageable = default;

        public override string Id => _id;
        public override float DistanceToBeDetected => _distanceToBeDetected;

        #region Unity events

        private void Awake()
        {
            _damageable = GetComponent<IDamageable>();
            _damageable.OnDied += Dead;
        }

        private void OnDestroy()
        {
            _damageable.OnDied -= Dead;
        }

        #endregion

        #region Private methods

        private void Dead(object sender, CustomEventArgs.EntityDeadEventArgs eventArgs)
        {
            StopDetection();

            InteractableEvent.Trigger(InteractableEventType.EnemyDead, transform);
        }

        #endregion

        #region BaseInteractable implementation

        public override Enums.InteractablePriorities Priority => Enums.InteractablePriorities.ENEMY;

        public override float AreaRadiusDetection => _distanceToBeDetected;

        public override void Detect()
        {
            if (_damageable.IsDead) return;

            //if (_wasDetected) return;

            //base.Detect();

            //_wasDetected = true;

            //CustomEventArgs.InteractableArgs eventArgs = new CustomEventArgs.InteractableArgs(transform, this);

            //GameEvents.OnDetectInteractable?.Invoke(this, eventArgs);
        }

        public override void StopDetection()
        {
            //if (!_wasDetected) return;

            //base.StopDetection();

            //CustomEventArgs.InteractableArgs eventArgs = new CustomEventArgs.InteractableArgs(transform, this);

            //GameEvents.OnStopDetectingIteractable?.Invoke(this, eventArgs);

            //_wasDetected = false;
        }

        #endregion
    }
}