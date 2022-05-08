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

            InteractableCharacterDetection.OnShowButton += ShowButton;
            InteractableCharacterDetection.OnHideButton += HideButton;
        }

        private void OnDisable()
        {
            this.EventStopListening<GameLevelEvent>();

            InteractableCharacterDetection.OnShowButton -= ShowButton;
            InteractableCharacterDetection.OnHideButton -= HideButton;
        }

        #endregion

        #region UIButtonPlayerAction implementation

        protected override void StartAction()
        {
            //_pickerId = (_interactable == null) ? "NO PICKER" : _interactable.Id;

            Debug.Log($"<color=green>Pick item from button</color> {_pickerId}");

            InteractableEvent.Trigger(InteractableEventType.PickItemFromButton, _pickerId);

            Hide();
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

        private void ShowButton(string id)
        {
            _pickerId = id;

            Show();
        }

        private void HideButton()
        {
            Hide();
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            /*switch (eventData.EventType)
            {
                case GameLevelEventType.InteractableStartDetection:
                    PickerItemInteractable interactable = (PickerItemInteractable) eventData.Payload[0];
                    Refresh(interactable.Id);
                    break;

                case GameLevelEventType.InteractableChanged:
                    var isPicker = eventData.Payload[0] is PickerItemInteractable;

                    if (isPicker)
                    {
                        var picker = (PickerItemInteractable)eventData.Payload[0];
                        RefreshContent(picker, (bool)eventData.Payload[2]);
                    }

                    break;
            }*/
        }

        #endregion
    }
}