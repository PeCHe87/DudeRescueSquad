using DudeResqueSquad;

namespace DudeRescueSquad.Core
{
    public interface IInteractable
    {
        string Id { get; }
        Enums.InteractablePriorities Priority { get; }
        float AreaRadiusDetection { get; }

        void Detect();
        void StopDetection();
    }
}