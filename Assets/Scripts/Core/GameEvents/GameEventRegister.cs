
namespace DudeRescueSquad.Core.Events
{
    /// <summary>
    /// Static class that allows any class to start or stop listening to events
    /// </summary>
    public static class GameEventRegister
    {
        public delegate void Delegate<T>(T eventType);

        /// <summary>
        /// Start listening events of this especific type
        /// </summary>
        /// <typeparam name="T">the event Type</typeparam>
        /// <param name="caller"></param>
        public static void EventStartListening<T>(this IGameEventListener<T> caller) where T : struct
        {
            GameEventsManager.AddListener<T>(caller);
        }

        /// <summary>
        /// Stop listening events of this especific type
        /// </summary>
        /// <typeparam name="T">the event Type</typeparam>
        /// <param name="caller"></param>
        public static void EventStopListening<T>(this IGameEventListener<T> caller) where T : struct
        {
            GameEventsManager.RemoveListener<T>(caller);
        }
    }
}