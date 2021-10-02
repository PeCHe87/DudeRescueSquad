using DudeRescueSquad.Core.LevelManagement;
using DudeRescueSquad.Core.Weapons;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.UI.Gameplay
{
    public class UIButtonPlayerActionAttack : UIButtonPlayerAction
    {
        #region Inspector properties

        [SerializeField] private Image _icon = default;
        [SerializeField] private Image _imgFillEnableAction = null;
        [SerializeField] private Image _imgFillDurability = null;
        [SerializeField] private Image _imgBackgroundDurability = null;
        [SerializeField] private Image _imgCurrentItem = null;
        [SerializeField] private TextMeshProUGUI _txtBulletsAmount = null;

        [Header("Reloading")]
        [SerializeField] private GameObject _progressBarReloading = null;
        [SerializeField] private Image _progressBarFillReloading = null;

        #endregion

        #region UIButtonPlayerAction implementation

        protected override void StartAction()
        {
            Debug.Log("<color=green>OnPointerDown</color> called.");

            // TODO: check if it is possible to start the action

            GameLevelEvent.Trigger(GameLevelEventType.StartPlayerAttack);
        }

        protected override void StopAction()
        {
            Debug.Log("<color=red>OnPointerUp</color> called.");

            GameLevelEvent.Trigger(GameLevelEventType.StopPlayerAttack);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// It updates the content of the button: item preview, current durability, current amount of bullets, max bullets, etc
        /// </summary>
        public void RefreshContent(BaseWeapon weapon)
        {
            _icon.sprite = weapon.WeaponData.Icon;

            // TODO: update another contet related with stuff like bullets, max bullets, durability, etc
        }

        #endregion
    }
}