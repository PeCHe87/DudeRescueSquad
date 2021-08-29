using DudeRescueSquad.Core.Events;
using UnityEngine;

namespace DudeRescueSquad.Core
{
    public struct InteractableEvent
    {
        static InteractableEvent e;

        public InteractableEventType EventType { get; private set; }
        public Transform Element { get; private set; }
        public string ItemId { get; private set; }

        public static void Trigger(InteractableEventType eventType, Transform element)
        {
            e.Element = element;

            GameEventsManager.TriggerEvent(e);
        }

        public static void Trigger(InteractableEventType eventType, string itemId)
        {
            e.ItemId = itemId;

            GameEventsManager.TriggerEvent(e);
        }
    }
}