using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;

namespace DudeRescueSquad.UI
{
    public class UIPopupManager : Singleton<UIPopupManager>
    {
        #region Inspector properties

        [SerializeField] private List<UIPopup> popupsList = new List<UIPopup>();

        #endregion

        #region Private properties

        private Dictionary<PopupType, UIPopup> dic;

        #endregion

        #region Public properties

        public bool IsPopupOpen { get; private set; }

        #endregion

        #region Events

        private static Action onShowPopup = delegate { };
        public static event Action OnShowPopup
        {
            add
            {
                onShowPopup -= value;
                onShowPopup = value;
            }
            remove { onShowPopup -= value; }
        }

        private static Action onHidePopup = delegate { };
        public static event Action OnHidePopup
        {
            add
            {
                onHidePopup -= value;
                onHidePopup = value;
            }
            remove { onHidePopup -= value; }
        }

        #endregion

        #region Unity methods

        protected virtual void Awake()
        {
            DontDestroyOnLoad(this);

            if (FindObjectsOfType(GetType()).Length > 1)
            {
                DestroyImmediate(gameObject);
                return;
            }

            popupsList = FindObjectsOfType<UIPopup>().ToList();

            dic = new Dictionary<PopupType, UIPopup>(popupsList.Count);

            for (int i = 0; i < popupsList.Count; i++)
            {
                dic[popupsList[i].popupType] = popupsList[i];
            }

            HideAllPopups();

            UIPopup.OnClosePopup += PopupWasClosed;
        }

        private void OnDestroy()
        {
            UIPopup.OnClosePopup -= PopupWasClosed;
        }

        #endregion

        #region Public methods

        public bool TryCloseSomething()
        {
            // any popup is being shown
            if (!IsShowing())
                return false;
            // search for 'closable' popups that may be closed
            List<UIPopup> closablePopups = new List<UIPopup>();
            for (int i = 0; i < popupsList.Count; i++)
            {
                if (popupsList[i].gameObject.activeSelf && popupsList[i].CanClose() && popupsList[i].Canvas.enabled)
                {
                    closablePopups.Add(popupsList[i]);
                }
            }

            // found 'closable' popups, let's find top-most sibling to close
            if (closablePopups.Count > 0)
            {
                int maxSiblingIndex = -1;
                int candidate = -1;
                for (int i = 0; i < closablePopups.Count; i++)
                {
                    int index = closablePopups[i].transform.GetSiblingIndex() + closablePopups[i].Canvas.sortingOrder * 100;
                    if (maxSiblingIndex < index)
                    {
                        maxSiblingIndex = index;
                        candidate = i;
                    }
                }
                if (candidate >= 0)
                {
                    closablePopups[candidate].OnClickClose();
                }
            }
            return true;
        }

        public void HideAllPopups()
        {
            for (int i = 0; i < popupsList.Count; i++)
            {
                if (popupsList[i].gameObject.activeSelf)
                    popupsList[i].gameObject.SetActive(false);
            }

            IsPopupOpen = false;
        }

        public void HideAllPopupsCanvas()
        {
            for (int i = 0; i < popupsList.Count; i++)
            {
                if (popupsList[i].gameObject.activeSelf)
                    popupsList[i].Canvas.enabled = false;
            }
        }

        public void UnHideAllPopupsCanvas()
        {
            for (int i = 0; i < popupsList.Count; i++)
            {
                popupsList[i].Canvas.enabled = true;
            }
        }

        public T ShowPopup<T>(PopupType pType) where T : UIPopup
        {
            if (!dic.TryGetValue(pType, out var popup))
            {
                Debug.LogError($"Couldn't find popup type: {pType}");
                return null;
            }

            popup.gameObject.SetActive(true);
            popup.transform.SetAsLastSibling();

            Debug.Log($"ShowPopup '{pType}'");

            onShowPopup?.Invoke();

            return (T)popup;
        }

        public T GetPopup<T>(PopupType pType) where T : UIPopup
        {
            return (T)dic[pType];
        }

        public UIPopup ShowPopup(PopupType pType)
        {
            return ShowPopup<UIPopup>(pType);
        }

        public bool HidePopup(PopupType pType)
        {
            var popup = dic[pType];
            if (!popup && !popup.gameObject.activeSelf)
                return false;
            popup.OnClickClose();

            onHidePopup?.Invoke();

            return true;
        }

        public bool IsShowing()
        {
            for (int i = 0; i < popupsList.Count; i++)
            {
                if (popupsList[i].gameObject.activeSelf && popupsList[i].Canvas.enabled)
                {
                    return true;
                }
            }
            return false;
        }

        public bool IsShowing(PopupType popupType)
        {
            var popup = dic[popupType];
            return popup.gameObject.activeSelf;
        }

        #endregion

        #region Internal methods

        private void PopupWasClosed(UIPopup popup)
        {
            Debug.Log($"<b>UIPopupManager</b> - close popup: {popup.name}");
        }

        #endregion
    }
}