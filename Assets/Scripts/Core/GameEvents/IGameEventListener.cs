
namespace DudeRescueSquad.Core.Events
{
    /// <summary>
    /// A public interface you'll need to implement for each type of event you want to listen to.
    /// </summary>
    /// <typeparam name="T">The event type parameter.</typeparam>
    public interface IGameEventListener<T> : IGameEventListenerBase
    {
        void OnGameEvent(T eventType);
    }
}