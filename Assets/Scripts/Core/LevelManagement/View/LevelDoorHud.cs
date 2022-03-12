using System;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.Core.LevelManagement.View
{
    public class LevelDoorHud : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Button _btnSelection = default;

        #endregion

        #region Private properties

        private Canvas _canvas = default;
        private Action _callbackSelection = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _btnSelection.onClick.AddListener(Select);
        }

        private void OnDestroy()
        {
            _btnSelection.onClick.RemoveAllListeners();
        }

        #endregion

        #region Public properties

        public bool IsOpen => _canvas.enabled;

        #endregion

        #region Public methods

        public void Setup(Action callback)
        {
            _callbackSelection = callback;

            Hide();
        }

        public void Show()
        {
            _canvas.enabled = true;
        }

        public void Hide()
        {
            _canvas.enabled = false;
        }

        #endregion

        #region Private methods

        private void Select()
        {
            // Hide UI
            Hide();

            // Communicate to the door
            _callbackSelection?.Invoke();
        }

        #endregion
    }
}