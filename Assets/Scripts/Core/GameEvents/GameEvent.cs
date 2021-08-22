
namespace DudeRescueSquad.Core.Events
{
    public struct GameEvent
    {
        static GameEvent e;

        public string EventName { get; private set; }

        public GameEvent(string newName)
        {
            EventName = newName;
        }

        public static void Trigger(string newName)
        {
            e.EventName = newName;
            GameEventsManager.TriggerEvent(e);
        }
    }
}