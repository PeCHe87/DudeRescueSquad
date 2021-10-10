using DudeRescueSquad.Core.LevelManagement;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        public virtual string Id { get; }
        public virtual Enums.InteractablePriorities Priority => Enums.InteractablePriorities.NONE;

        public virtual float AreaRadiusDetection => throw new System.NotImplementedException();

        public virtual float DistanceToBeDetected { get; }

        public virtual void Detect()
        {
            var payload = new object[] { this };
            GameLevelEvent.Trigger(GameLevelEventType.InteractableStartDetection, payload);
        }

        public virtual void StopDetection()
        {
            var payload = new object[] { this };
            GameLevelEvent.Trigger(GameLevelEventType.InteractableStopDetection, payload);
        }
    }
}