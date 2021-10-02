using DudeRescueSquad.Core.LevelManagement;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.UI.Gameplay
{
    public class UIButtonPlayerActionDash : UIButtonPlayerAction
    {
        #region Inspector properties

        [SerializeField] private Image _imgFillEnableAction = null;

        #endregion

        #region UIButtonPlayerAction implementation

        protected override void StartAction()
        {
            Debug.Log("<color=green>OnPointerDown</color> called.");

            // TODO: check if it is possible to start the action

            GameLevelEvent.Trigger(GameLevelEventType.StartPlayerDash);
        }

        protected override void StopAction()
        {
            Debug.Log("<color=red>OnPointerUp</color> called.");
        }

        #endregion
    }
}