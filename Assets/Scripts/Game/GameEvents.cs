using System;

namespace DudeResqueSquad
{
    public class GameEvents
    {
        /// <summary>
        /// Stops the current action that player is doing
        /// </summary>
        public static EventHandler OnStopAction;

        /// <summary>
        /// Invoked when a player collects an item
        /// </summary>
        public static EventHandler<CustomEventArgs.CollectItemEventArgs> OnCollectItem;

        /// <summary>
        /// Each time an entity is destroyed this event could be invoked from the entity whose is dead
        /// </summary>
        public static EventHandler<CustomEventArgs.EntityDeadEventArgs> OnEntityHasDied;
    }
}