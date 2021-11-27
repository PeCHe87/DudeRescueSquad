using DudeRescueSquad.Core;
using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.LevelManagement;
using UnityEngine;

namespace DudeRescueSquad.UI.Gameplay
{
    public class UIButtonPlayerActionPickItem : UIButtonPlayerAction, IGameEventListener<GameLevelEvent>
    {
        #region Inspector properties
        #endregion

        #region Private properties

        private PickerItemInteractable _interactable = default;
        private string _pickerId = string.Empty;

        #endregion

        #region Unity events

        private void OnEnable()
        {
            this.EventStartListening<GameLevelEvent>();
        }

        private void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();
        }

        #endregion

        #region UIButtonPlayerAction implementation

        protected override void StartAction()
        {
            _pickerId = (_interactable == null) ? "NO PICKER" : _interactable.Id;

            Debug.Log($"<color=green>Pick item from button</color> {_pickerId}");

            InteractableEvent.Trigger(InteractableEventType.PickItemFromButton, _pickerId);
        }

        protected override void StopAction()
        {
            // Nothing to do here
        }

        #endregion

        #region Private methods

        private void Refresh(string itemId)
        {
            _pickerId = itemId;
        }

        private void RefreshContent(PickerItemInteractable interactable, bool pickable)
        {
            _interactable = interactable;

            if (pickable)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.InteractableStartDetection:
                    PickerItemInteractable interactable = (PickerItemInteractable) eventData.Payload[0];
                    Refresh(interactable.Id);
                    break;

                case GameLevelEventType.InteractableChanged:
                    RefreshContent((PickerItemInteractable) eventData.Payload[0], (bool) eventData.Payload[2]);
                    break;
            }
        }

        #endregion
    }
}