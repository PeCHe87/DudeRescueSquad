using DudeResqueSquad;

namespace DudeRescueSquad.Core
{
    public interface IInteractable
    {
        Enums.InteractablePriorities Priority { get; }
        float AreaRadiusDetection { get; }

        void Detect();
        void StopDetection();
    }
}