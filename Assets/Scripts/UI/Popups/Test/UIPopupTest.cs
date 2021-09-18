using System;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.UI
{
    public class UIPopupTest : UIPopup
    {
        #region Inspector properties

        [SerializeField] private Transform _content = default;
        [SerializeField] private Text _txtTitle = default;
        [SerializeField] private Text _txtDescription = default;
        [SerializeField] private Button _btnAction = default;
        [SerializeField] private Button _btnClose = default;

        #endregion

        #region Private properties

        private Action _cachedAction = null;
        private Action _onCloseAction = null;

        #endregion

        #region Unity events

        private void Awake()
        {
            _btnAction.onClick.AddListener(ButtonAction);
            _btnClose.onClick.AddListener(OnClickClose);
        }

        private void OnDestroy()
        {
            _btnAction.onClick.RemoveListener(ButtonAction);
            _btnClose.onClick.RemoveListener(OnClickClose);
        }

        #endregion

        #region Public methods

        public UIPopupTest Setup(string title, string desc, Action buttonAction, Action closeAction = null)
        {
            _txtTitle.text = title;
            _txtDescription.text = desc;

            _cachedAction = buttonAction;
            _onCloseAction = closeAction;

            gameObject.Toggle(true);
            PopupAnimator.Instance.AnimateScalePopup(Canvas, _content);

            return this;
        }

        #endregion

        #region Private methods

        private void ButtonAction()
        {
            OnClickClose();

            _cachedAction?.Invoke();
            _cachedAction = null;
        }

        #endregion

        #region UIPopup implementation

        public override void OnClickClose()
        {
            base.OnClickClose();

            _onCloseAction?.Invoke();
            _onCloseAction = null;
        }

        #endregion
    }
}