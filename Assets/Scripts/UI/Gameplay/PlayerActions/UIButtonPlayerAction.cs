using DudeResqueSquad;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

namespace DudeRescueSquad.UI.Gameplay
{
    /// <summary>
    /// This classs represents the base logic for any button related with a player action like:
    /// attack, 
    /// interact, 
    /// use, 
    /// etc
    /// </summary>
    public abstract class UIButtonPlayerAction : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private GameObject _content = default;
        [SerializeField] private Button _button = default;
        [SerializeField] private Image _imgColdownProgress = null;
        [SerializeField] protected bool _useColdownByAction = false;

        #endregion

        #region Private properties

        private EventTrigger trigger;
        private EventTrigger.Entry entryPointerDown;
        private EventTrigger.Entry entryPointerUp;
        private float _coldownStartTime = 0;
        private float _coldownDuration = 0;

        #endregion

        #region Public properties

        public bool IsVisible => _content.activeSelf;

        #endregion

        #region Unity Events

        protected virtual void Awake()
        {
            // Add Event Triggers
            trigger = _button.GetComponent<EventTrigger>();

            entryPointerDown = new EventTrigger.Entry();
            entryPointerDown.eventID = EventTriggerType.PointerDown;
            entryPointerDown.callback.AddListener((data) => { StartAction(); });
            trigger.triggers.Add(entryPointerDown);

            entryPointerUp = new EventTrigger.Entry();
            entryPointerUp.eventID = EventTriggerType.PointerUp;
            entryPointerUp.callback.AddListener((data) => { StopAction(); });
            trigger.triggers.Add(entryPointerUp);
        }

        protected virtual void OnDestroy()
        {
            // Remove Event Triggers
            entryPointerDown.callback.RemoveListener((data) => { StartAction(); });
            trigger.triggers.Remove(entryPointerDown);

            entryPointerUp.callback.RemoveListener((data) => { StopAction(); });
            trigger.triggers.Remove(entryPointerUp);
        }

        #endregion

        #region Public methods

        public virtual void Setup(DudeRescueSquad.Core.Characters.Character character, bool startVisible = false)
        {
            if (startVisible)
            {
                Show();
            }
            else
            {
                Hide();
            }
        }

        public virtual void Teardown()
        {
            Disable();
        }

        public virtual void Show()
        {
            _content.Toggle(true);
        }

        public virtual void Hide()
        {
            _content.Toggle(false);
        }

        public virtual void Enable()
        {
            if (_button == null) return;

            _button.enabled = true; 
        }

        public virtual void Disable()
        {
            if (_button == null) return;

            _button.enabled = false;
        }

        protected abstract void StartAction();

        protected abstract void StopAction();

        protected void StartColdown()
        {
            _imgColdownProgress.fillAmount = 1;
        }

        protected void UpdateColdownProgress(float progress)
        {
            _imgColdownProgress.fillAmount = progress;
        }

        protected void FinishColdown()
        {
            _imgColdownProgress.fillAmount = 0;
        }

        #endregion
    }
}