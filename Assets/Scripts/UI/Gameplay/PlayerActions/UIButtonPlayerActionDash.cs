using DudeRescueSquad.Core.Characters;
using DudeRescueSquad.Core.LevelManagement;
using UnityEngine;

namespace DudeRescueSquad.UI.Gameplay
{
    public class UIButtonPlayerActionDash : UIButtonPlayerAction
    {
        #region Inspector properties
        #endregion

        #region Private properties

        private CharacterAbilityDash _characterDash = default;

        #endregion

        #region UIButtonPlayerAction implementation

        public override void Setup(Character character, bool startVisible = false)
        {
            base.Setup(character, startVisible);

            _characterDash = character.GetComponent<CharacterAbilityDash>();
            _characterDash.OnStartAction += ActionStarted;
            _characterDash.OnProcessActionProgress += ActionProgressProcessed;
            _characterDash.OnStopAction += ActionStopped;
        }

        public override void Teardown()
        {
            base.Teardown();

            if (_characterDash)
            {
                _characterDash.OnStartAction -= ActionStarted;
                _characterDash.OnProcessActionProgress -= ActionProgressProcessed;
                _characterDash.OnStopAction -= ActionStopped;
            }
        }

        protected override void StartAction()
        {
            Debug.Log("<color=green>OnPointerDown</color> called.");

            GameLevelEvent.Trigger(GameLevelEventType.StartPlayerDash);
        }

        protected override void StopAction()
        {
            Debug.Log("<color=red>OnPointerUp</color> called.");
        }

        #endregion

        #region Private methods

        private void ActionStarted()
        {
            StartColdown();
        }

        private void ActionProgressProcessed(float progress)
        {
            UpdateColdownProgress(progress);
        }

        private void ActionStopped()
        {
            FinishColdown();
        }

        #endregion
    }
}