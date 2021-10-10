using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.LevelManagement;
using UnityEngine;

namespace DudeRescueSquad.UI.Gameplay
{
    public class UIPlayerActionsManager : MonoBehaviour, IGameEventListener<InventoryEvent>, IGameEventListener<GameLevelEvent>
    {
        #region Inspector properties

        [SerializeField] private UIButtonPlayerActionAttack _attackButton = default;
        [SerializeField] private UIButtonPlayerActionDash _dashButton = default;

        #endregion

        #region Private properties

        private Character _character = default;
        private CharacterAbilityHandleWeapon _characterHandleWeapon = default;

        #endregion

        #region GameEventListener<InventoryEvent> implementation

        /// <summary>
        /// Check if a weapon is equipped to communicate the respective button about it
        /// </summary>
        /// <param name="eventData">Inventory event.</param>
        public virtual void OnGameEvent(InventoryEvent eventData)
        {
            switch (eventData.EventType)
            {
                case InventoryEventType.ItemEquipped:
                    EquipWeapon(eventData.ItemId);
                    break;
            }
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.LevelLoaded:
                    Initialize(eventData.Character);
                    break;

                case GameLevelEventType.LevelUnloaded:
                    Teardown();
                    break;
            }
        }

        #endregion

        #region Unity Events

        /// <summary>
        /// On enable, we start listening for GameEvents. You may want to extend that to listen to other types of events.
        /// </summary>
        private void OnEnable()
        {
            this.EventStartListening<InventoryEvent>();
            this.EventStartListening<GameLevelEvent>();
        }

        /// <summary>
        /// On disable, we stop listening for GameEvents. You may want to extend that to stop listening to other types of events.
        /// </summary>
        protected void OnDisable()
        {
            this.EventStopListening<InventoryEvent>();
            this.EventStopListening<GameLevelEvent>();
        }

        #endregion

        #region Private methods

        private void Initialize(Character character)
        {
            _character = character;

            _characterHandleWeapon = _character.GetComponent<CharacterAbilityHandleWeapon>();

            _attackButton.Setup(_character);
            _dashButton.Setup(_character, true);
        }

        private void EquipWeapon(string itemId)
        {
            Debug.Log($"Item equipped: {itemId}");

            _attackButton.RefreshContent(_characterHandleWeapon.CurrentWeapon);

            if (!_attackButton.IsVisible)
            {
                _attackButton.Show();
            }
        }

        private void Teardown()
        {
            _attackButton.Teardown();
            _dashButton.Teardown();
        }

        #endregion
    }
}