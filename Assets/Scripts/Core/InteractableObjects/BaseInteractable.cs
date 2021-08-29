using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public abstract class BaseInteractable : MonoBehaviour, IInteractable
    {
        public virtual Enums.InteractablePriorities Priority => Enums.InteractablePriorities.NONE;

        public virtual float AreaRadiusDetection => throw new System.NotImplementedException();

        public virtual void Detect()
        {
        }

        public virtual void StopDetection()
        {
        }
    }
}