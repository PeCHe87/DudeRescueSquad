using System;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.Core.Events
{
    /// <summary>
        /// This class handles event management, and can be used to broadcast events throughout the game, to tell one class (or many) that something's happened.
        /// Events are structs, you can define any kind of events you want. This manager comes with MMGameEvents, which are 
        /// basically just made of a string, but you can work with more complex ones if you want.
        /// 
        /// To trigger a new event, from anywhere, do YOUR_EVENT.Trigger(YOUR_PARAMETERS)
        /// So MMGameEvent.Trigger("Save"); for example will trigger a Save MMGameEvent
        /// 
        /// you can also call MMEventManager.TriggerEvent(YOUR_EVENT);
        /// For example : MMEventManager.TriggerEvent(new MMGameEvent("GameStart")); will broadcast an MMGameEvent named GameStart to all listeners.
        ///
        /// To start listening to an event from any class, there are 3 things you must do : 
        ///
        /// 1 - tell that your class implements the MMEventListener interface for that kind of event.
        /// For example: public class GUIManager : Singleton<GUIManager>, MMEventListener<MMGameEvent>
        /// You can have more than one of these (one per event type).
        ///
        /// 2 - On Enable and Disable, respectively start and stop listening to the event :
        /// void OnEnable()
        /// {
        /// 	this.MMEventStartListening<MMGameEvent>();
        /// }
        /// void OnDisable()
        /// {
        /// 	this.MMEventStopListening<MMGameEvent>();
        /// }
        /// 
        /// 3 - Implement the MMEventListener interface for that event. For example :
        /// public void OnMMEvent(MMGameEvent gameEvent)
        /// {
        /// 	if (gameEvent.EventName == "GameOver")
        ///		{
        ///			// DO SOMETHING
        ///		}
        /// } 
        /// will catch all events of type MMGameEvent emitted from anywhere in the game, and do something if it's named GameOver
        /// </summary>
    [ExecuteAlways]
    public static class GameEventsManager
    {
        private static Dictionary<Type, List<IGameEventListenerBase>> _subscribersList;

        static GameEventsManager()
        {
            _subscribersList = new Dictionary<Type, List<IGameEventListenerBase>>();
        }

        /// <summary>
        /// Adds a new subscriber to a certain event.
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="T">The event type.</typeparam>
        public static void AddListener<T>(IGameEventListener<T> listener) where T : struct
        {
            Type eventType = typeof(T);

            if (!_subscribersList.ContainsKey(eventType))
                _subscribersList[eventType] = new List<IGameEventListenerBase>();

            if (!SubscriptionExists(eventType, listener))
                _subscribersList[eventType].Add(listener);
        }

        /// <summary>
        /// Removes a subscriber from a certain event.
        /// </summary>
        /// <param name="listener">listener.</param>
        /// <typeparam name="T">The event type.</typeparam>
        public static void RemoveListener<T>(IGameEventListener<T> listener) where T : struct
        {
            Type eventType = typeof(T);

            if (!_subscribersList.ContainsKey(eventType))
            {
#if EVENTROUTER_THROWEXCEPTIONS
				throw new ArgumentException( string.Format( "Removing listener \"{0}\", but the event type \"{1}\" isn't registered.", listener, eventType.ToString() ) );
#else
                return;
#endif
            }

            List<IGameEventListenerBase> subscriberList = _subscribersList[eventType];
            bool listenerFound;
            listenerFound = false;

            if (listenerFound)
            {

            }

            for (int i = 0; i < subscriberList.Count; i++)
            {
                if (subscriberList[i] == listener)
                {
                    subscriberList.Remove(subscriberList[i]);
                    listenerFound = true;

                    if (subscriberList.Count == 0)
                        _subscribersList.Remove(eventType);

                    return;
                }
            }

#if EVENTROUTER_THROWEXCEPTIONS
		    if( !listenerFound )
		    {
				throw new ArgumentException( string.Format( "Removing listener, but the supplied receiver isn't subscribed to event type \"{0}\".", eventType.ToString() ) );
		    }
#endif
        }

        /// <summary>
        /// Triggers an event. All instances that are subscribed to it will receive it (and will potentially act on it).
        /// </summary>
        /// <param name="newEvent">The event to trigger.</param>
        /// <typeparam name="T">The 1st type parameter.</typeparam>
        public static void TriggerEvent<T>(T newEvent) where T : struct
        {
            List<IGameEventListenerBase> list;

            if (!_subscribersList.TryGetValue(typeof(T), out list))
#if EVENTROUTER_REQUIRELISTENER
			        throw new ArgumentException( string.Format( "Attempting to send event of type \"{0}\", but no listener for this type has been found. Make sure this.Subscribe<{0}>(EventRouter) has been called, or that all listeners to this event haven't been unsubscribed.", typeof( MMEvent ).ToString() ) );
#else
                return;
#endif

            for (int i = 0; i < list.Count; i++)
            {
                (list[i] as IGameEventListener<T>).OnGameEvent(newEvent);
            }
        }

        /// <summary>
        /// Checks if there are subscribers for a certain type of events
        /// </summary>
        /// <returns><c>true</c>, if exists was subscriptioned, <c>false</c> otherwise.</returns>
        /// <param name="type">Type.</param>
        /// <param name="receiver">Receiver.</param>
        private static bool SubscriptionExists(Type type, IGameEventListenerBase receiver)
        {
            List<IGameEventListenerBase> receivers;

            if (!_subscribersList.TryGetValue(type, out receivers)) return false;

            bool exists = false;

            for (int i = 0; i < receivers.Count; i++)
            {
                if (receivers[i] == receiver)
                {
                    exists = true;
                    break;
                }
            }

            return exists;
        }
    }
}