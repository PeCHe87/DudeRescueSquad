using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Events;

namespace DudeRescueSquad.Core.LevelManagement
{
    public struct GameLevelEvent
    {
        static GameLevelEvent e;

        public GameLevelEventType EventType { get; private set; }
        public Character Character { get; private set; }

        public GameLevelEvent(GameLevelEventType eventType, Character character)
        {
            EventType = eventType;
            Character = character;
        }

        public static void Trigger(GameLevelEventType eventType, Character character = null)
        {
            e.EventType = eventType;
            e.Character = character;

            GameEventsManager.TriggerEvent(e);
        }
    }
}