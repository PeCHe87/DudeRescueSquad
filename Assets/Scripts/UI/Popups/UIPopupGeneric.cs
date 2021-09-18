using System;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.UI
{
    public class UIPopupGeneric : UIPopup
    {
        [Header("UIPopupGeneric")]
        public Text headerText;
        public Text descriptionText;
        public Button primaryButton;
        public Button secondaryButton;

        public enum PopupType
        {
            VaultFull,
            Generic,
            ExitBattle,
            BuildersInfo,
            Notifications,
            AppClose
        }

        private PopupType currentType;
        private Action cachedAction = null;
        private Action onCloseAction = null;

        #region public methods

        public UIPopupGeneric Setup(string title, string desc)
        {
            currentType = PopupType.Generic;
            headerText.text = title;
            descriptionText.text = desc;

            primaryButton.gameObject.SetActive(true);
            primaryButton.GetComponentInChildren<Text>().text = "OK";

            secondaryButton.gameObject.SetActive(false);
            onCloseAction = null;
            return this;
        }

        public UIPopupGeneric Setup(string title, string desc, Action action)
        {
            currentType = PopupType.Generic;
            headerText.text = title;
            descriptionText.text = desc;

            primaryButton.gameObject.SetActive(true);
            primaryButton.GetComponentInChildren<Text>().text = "NO";

            secondaryButton.gameObject.SetActive(true);
            secondaryButton.GetComponentInChildren<Text>().text = "YES";

            cachedAction = action;
            onCloseAction = null;

            gameObject.SetActive(true);
            return this;
        }

        public UIPopupGeneric Setup(PopupType type, string text = null)
        {
            /*
            //	Managers.UI.ToggleBaseCanvas(false);
            currentType = type;
            cachedAction = null;
            onCloseAction = null;
            switch (type)
            {
                case PopupType.VaultFull:
                    headerText.text = "CAN'T COLLECT";
                    descriptionText.text = "Your vault is full\nUpgrade your vault to collect";

                    primaryButton.gameObject.SetActive(true);
                    primaryButton.GetComponentInChildren<Text>().text = "Go To Vault".ToUpper();
                    if (Managers.Base.CurrentlySelectedObject is FarmingUtilityObject)
                    {
                        secondaryButton.gameObject.SetActive(true);
                        secondaryButton.GetComponentInChildren<Text>().text = "Manage".ToUpper();
                    }
                    else
                    {
                        secondaryButton.gameObject.SetActive(false);
                    }
                    break;
                case PopupType.Generic:
                    headerText.text = "ATTENTION";
                    descriptionText.text = text;

                    primaryButton.gameObject.SetActive(true);
                    primaryButton.GetComponentInChildren<Text>().text = "OK";

                    secondaryButton.gameObject.SetActive(false);
                    break;
                case PopupType.ExitBattle:
                    headerText.text = "WARNING";
                    descriptionText.text = "Are you sure you want to quit this battle";

                    primaryButton.gameObject.SetActive(true);
                    primaryButton.GetComponentInChildren<Text>().text = "NO";

                    secondaryButton.gameObject.SetActive(true);
                    secondaryButton.GetComponentInChildren<Text>().text = "YES";
                    break;
                case PopupType.BuildersInfo:
                    int maxBuilders = PlayerProfileV2.client.GetVipBenefits().builder_limit; //[AA] TODO: fill builders data
                    int currentBuilders = PlayerProfileV2.client.buildings.Count(b => b.IsUpgrading); //[AA] TODO: fill builders data

                    headerText.text = "BUILDERS";
                    descriptionText.text = $"Your builders can work on {maxBuilders} building at once.\nThey are currently working on {currentBuilders} building.";

                    primaryButton.gameObject.SetActive(true);
                    primaryButton.GetComponentInChildren<Text>().text = "OK";

                    secondaryButton.gameObject.SetActive(false);
                    break;
                case PopupType.Notifications:
                    headerText.text = "CONFIRM NOTIFICATIONS";
                    descriptionText.text = "Would you like to receive notifications when your upgrades are ready?";

                    primaryButton.gameObject.SetActive(true);
                    primaryButton.GetComponentInChildren<Text>().text = "NO";

                    secondaryButton.gameObject.SetActive(true);
                    secondaryButton.GetComponentInChildren<Text>().text = "YES";
                    break;
                case PopupType.AppClose:
                    headerText.text = "ATTENTION";
                    descriptionText.text = "Are you sure you want to leave the game?";
                    primaryButton.gameObject.SetActive(true);
                    primaryButton.GetComponentInChildren<Text>().text = "YES";

                    secondaryButton.gameObject.SetActive(true);
                    secondaryButton.GetComponentInChildren<Text>().text = "NO";
                    break;
            }
            gameObject.SetActive(true);
            */
            return this;
        }

        public void SetCloseAction(Action closeAction)
        {
            onCloseAction = closeAction;
        }

        #endregion

        #region UI callbacks

        public void OnClickPrimary()
        {
            /*
            switch (currentType)
            {
                case PopupType.VaultFull:
                    Managers.Base.vault.OnSelected();
                    Managers.Camera.GetCamera<PlayerBaseCamera>(GameCameraType.Base).SetFocusObject(Managers.Base.vault.transform, 1);
                    Managers.Popup.HideAllPopups();
                    break;
                case PopupType.ExitBattle:
                    gameObject.SetActive(false);
                    break;
                case PopupType.AppClose:
                    gameObject.SetActive(false);
#if UNITY_EDITOR
                    UnityEditor.EditorApplication.isPlaying = false;
#else
				Application.Quit();
#endif
                    break;
                default:
                    OnClickClose();
                    break;
            }
            */

            OnClickClose();
        }

        public void OnClickSecondary()
        {
            /*
            switch (currentType)
            {
                case PopupType.VaultFull:
                    Managers.Screen.ShowScreen<UIScreenBaseObjectSelected>(ScreenType.BaseObjectSelected).Initialize(Managers.Base.CurrentlySelectedObject as UpgradableBaseObject);
                    gameObject.SetActive(false);
                    break;
                case PopupType.ExitBattle:
                    Managers.Game.OnGameEnded(true);
                    gameObject.SetActive(false);
                    break;
                case PopupType.Notifications:
                    NotificationMgr.Instance.ToggleNotifications(true);
                    gameObject.SetActive(false);
                    break;
                default:
                    OnClickClose();
                    cachedAction?.Invoke();
                    cachedAction = null;
                    break;
            }
            */

            OnClickClose();
            cachedAction?.Invoke();
            cachedAction = null;
        }

        public override void OnClickClose()
        {
            base.OnClickClose();
            onCloseAction?.Invoke();
            onCloseAction = null;
        }

        #endregion
    }
}