using UnityEngine;

namespace DudeRescueSquad.UI
{
    public enum PopupType
    {
        Test = -1,
        None = 0,

        // Main menu
        Generic = 1,

        // Gameplay
    }

    public class UIPopup : MonoBehaviour
    {
        [Header("UIPopup")]
        public PopupType popupType = PopupType.None;
        public System.Action OnCloseCallback = null;
        public static event System.Action<UIPopup> OnClosePopup;

        private Canvas _canvas = null;
        public Canvas Canvas => _canvas ?? (_canvas = GetComponent<Canvas>());

        #region Unity methods

        protected virtual void OnEnable()
        {
            //Managers.Camera.GetCamera<PlayerBaseCamera>(GameCameraType.Base).SetCanMove(false);
        }

        protected virtual void OnDisable()
        {
            //Managers.Camera.GetCamera<PlayerBaseCamera>(GameCameraType.Base).SetCanMove(true);
        }

        #endregion

        #region private methods
        #endregion

        #region virtual methods

        public virtual bool CanClose()
        {
            return true;
        }

        #endregion

        #region UI callbacks

        public virtual void OnClickClose()
        {
            gameObject.Toggle(false);
            OnCloseCallback?.Invoke();
            OnCloseCallback = null;

            OnClosePopup?.Invoke(this);
        }

        #endregion
    }
}