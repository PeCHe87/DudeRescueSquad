using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeRescueSquad.UI
{
    public class UIScreenManager : Singleton<UIScreenManager>
    {
        [Header("UIScreenManager")]
        public List<UIScreen> screensList = new List<UIScreen>();

        private Dictionary<ScreenType, UIScreen> dic;

        #region Events

        public static event Action<ScreenType> OnScreenOpened;

        private static Action onShowScreen = delegate { };
        public static event Action OnShowScreen
        {
            add
            {
                onShowScreen -= value;
                onShowScreen = value;
            }
            remove { onShowScreen -= value; }
        }

        private static Action onHideScreen = delegate { };
        public static event Action OnHideScreen
        {
            add
            {
                onHideScreen -= value;
                onHideScreen = value;
            }
            remove { onHideScreen -= value; }
        }

        #endregion

        #region Unity methods

        private void Awake()
        {
            DontDestroyOnLoad(this);

            if (FindObjectsOfType(GetType()).Length > 1)
            {
                DestroyImmediate(gameObject);
                return;
            }

            dic = new Dictionary<ScreenType, UIScreen>(screensList.Count);
            for (int i = 0; i < screensList.Count; i++)
            {
                dic[screensList[i].screenType] = screensList[i];
            }
        }

        private void Start()
        {
            /*
            ConnectionStatus.Instance.OnRestart += HideAllScreens;
            ConnectionStatus.Instance.OnRestart += StopAllCoroutines;
            TeamManager.OnMyTeamStatusChanged += ShowTeamStatusChanged;
            PlayerDataChange.Subscribe(PlayerDataEvent.LEVEL, ShowLevelScreen, false);
            */
        }

        private void OnDestroy()
        {
            /*
            TeamManager.OnMyTeamStatusChanged -= ShowTeamStatusChanged;
            PlayerDataChange.UnSubscribe(PlayerDataEvent.LEVEL, ShowLevelScreen);
            */
        }

        private void Update()
        {
            /*
            if (!Input.GetKeyDown(KeyCode.Escape))
                return;

            if (UIGenericRewardAnimation.IsShowing)
                return;

            if (UINetworkWait.IsShowing)
                return;

            if (FTUEManager.Active && Managers.FTUE.IsBlocking)
                return;

            if (Managers.Popup.TryCloseSomething())
                return;

            UIScreen additive = null;
            UIScreen normal = null;
            for (int i = 0; i < screensList.Count; i++)
            {
                var s = screensList[i];
                if (s.gameObject.activeSelf)
                {
                    if (s.wasAdded)
                        additive = s;
                    else
                        normal = s;
                }
            }
            if (additive != null)
                additive.OnCloseScreen();
            else if (normal != null)
            {
                if (normal.screenType != ScreenType.Base)
                    normal.OnCloseScreen();
                else
                    Managers.Popup.ShowPopup<UIPopupGeneric>(PopupType.Generic).Setup(UIPopupGeneric.PopupType.AppClose);
            }
            */
        }

        #endregion

        #region private methods

        private void ShowLevelScreen()
        {
            WaitForScreenAndExecute(ScreenType.Base, () => ShowScreen(ScreenType.LevelUp));
        }

        private IEnumerator WaitForScreenAndExecuteInternal(ScreenType triggerScreen, System.Action callback = null)
        {
            int count = 0;
            while (count < 3)
            {
                if (GetActiveScreen() != triggerScreen)
                    count = 0;
                else
                    count++;
                yield return new WaitForSeconds(.1f);
            }
            callback?.Invoke();
        }

        #endregion

        #region public methods

        public void WaitForScreenAndExecute(ScreenType triggerScreen, System.Action callback)
        {
            StartCoroutine(WaitForScreenAndExecuteInternal(triggerScreen, callback));
        }

        public void ShowEliteScreenDelayed()
        {
            //WaitForScreenAndExecute(ScreenType.Base, () => ShowScreen<UIScreenStore>(ScreenType.Store).InitializeCategory(UIScreenStore.Category.Elite));
        }

        public void AddScreen(ScreenType sType)
        {
            ShowScreen<UIScreen>(sType, false);
        }

        public T AddScreen<T>(ScreenType sType) where T : UIScreen
        {
            return ShowScreen<T>(sType, false);
        }

        public void ShowScreen(ScreenType sType)
        {
            ShowScreen<UIScreen>(sType);
        }

        public T ShowScreen<T>(ScreenType sType, bool hideAllScreens = true) where T : UIScreen
        {
            if (!dic.TryGetValue(sType, out var screen))
            {
                Debug.LogError($"[UIScreenManager] ShowScreen failed. Couldn't find screen type: {sType}");
                return null;
            }

            //var screen = dic[sType];
            if (hideAllScreens)
            {
                for (int i = 0; i < screensList.Count; i++)
                {
                    if (screensList[i] != screen)
                        screensList[i].gameObject.Toggle(false);
                }
            }
            screen.gameObject.Toggle(true);
            screen.wasAdded = !hideAllScreens;

            //SoundFxManager.Instance.PlaySfx(Sfx.POPUP_CLOSE);

            onShowScreen?.Invoke();
            OnScreenOpened?.Invoke(sType);

            return screen as T;
        }

        public void HideScreen(ScreenType sType)
        {
            var screen = dic[sType];
            screen.gameObject.Toggle(false);

            onHideScreen?.Invoke();
        }

        public T GetScreen<T>(ScreenType sType) where T : UIScreen
        {
            return (T)dic[sType];
        }

        public ScreenType GetActiveScreen()
        {
            for (int i = 0; i < screensList.Count; i++)
            {
                if (screensList[i].gameObject.activeSelf)
                {
                    return screensList[i].screenType;
                }
            }
            return ScreenType.None;
        }

        public void HideAllScreens()
        {
            /*
            for (int i = 0; i < screensList.Count; i++)
            {
                screensList[i].gameObject.Toggle(false);
            }
            Managers.Camera.ToggleRender(true);
            */
        }

        #endregion
    }
}