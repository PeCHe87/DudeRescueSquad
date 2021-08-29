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
        /// Process the player's action
        /// </summary>
        public static EventHandler OnProcessAction;

        /// <summary>
        /// Invoked when a player collects an item
        /// </summary>
        public static EventHandler<CustomEventArgs.CollectItemEventArgs> OnCollectItem;

        /// <summary>
        /// Each time an entity is destroyed this event could be invoked from the entity whose is dead
        /// </summary>
        public static EventHandler<CustomEventArgs.EntityDeadEventArgs> OnEntityHasDied;
        
        // <summary>
        /// Each time an entity fires this event is invoked
        /// </summary>
        public static EventHandler<CustomEventArgs.SpawnProjectileEventArgs> OnSpawnProjectile;

        /// <summary>
        /// Invoked each time the player's enemy target detection changes
        /// </summary>
        public static EventHandler<CustomEventArgs.EnemyTargetedArgs> OnEnemyTargetChanged;
        
        /// <summary>
        /// Invoked when player wants to process a rolling
        /// </summary>
        public static EventHandler OnStartPlayerRolling;

        /// <summary>
        /// Invoked when player starts rolling animation on floor from animation event
        /// </summary>
        public static EventHandler OnStartPlayerRollingAnimation;
        
        /// <summary>
        /// Invoked when player stops rolling and starts standing during animation from animation event
        /// </summary>
        public static EventHandler OnStopPlayerRollingAnimation;

        #region Interactable events

        public static EventHandler<CustomEventArgs.InteractableArgs> OnDetectInteractable;
        public static EventHandler<CustomEventArgs.InteractableArgs> OnStopDetectingIteractable;

        #endregion

        #region Game Level events

        public static EventHandler OnGameLevelLoaded;

        #endregion
    }
}