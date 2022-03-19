using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Events;

namespace DudeRescueSquad.Core.LevelManagement
{
    public struct GameLevelEvent
    {
        static GameLevelEvent e;

        public GameLevelEventType EventType { get; private set; }
        public Character Character { get; private set; }
        public object[] Payload { get; private set; }
        public bool DeadZoneRelease { get; private set; }

        public GameLevelEvent(GameLevelEventType eventType, Character character, object[] payload, bool deadZoneRelease)
        {
            EventType = eventType;
            Character = character;
            Payload = payload;
            DeadZoneRelease = deadZoneRelease;
        }

        public static void Trigger(GameLevelEventType eventType, Character character = null)
        {
            e.EventType = eventType;
            e.Character = character;

            GameEventsManager.TriggerEvent(e);
        }

        public static void Trigger(GameLevelEventType eventType, object[] payload)
        {
            e.EventType = eventType;
            e.Payload = payload;

            GameEventsManager.TriggerEvent(e);
        }

        public static void Trigger(GameLevelEventType eventType, bool deadZoneRelease)
        {
            e.EventType = eventType;
            e.DeadZoneRelease = deadZoneRelease;

            GameEventsManager.TriggerEvent(e);
        }
    }
}