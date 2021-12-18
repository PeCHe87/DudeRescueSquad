using DudeRescueSquad.Core;
using DudeRescueSquad.Core.Events;
using DudeRescueSquad.Core.Inventory.View;
using DudeRescueSquad.Core.LevelManagement;
using DudeResqueSquad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.UI
{
    public class UIInteractableInformation : MonoBehaviour, IGameEventListener<GameLevelEvent>
    {
        #region Inspector

        [SerializeField] private Image _fillProgressBar = default;
        [SerializeField] private TextMeshProUGUI _txtName, _txtHealth, _txtType = default;
        [SerializeField] private bool _canDebug = true;

        #endregion

        #region  Private properties

        private Canvas _canvas = default;
        private BaseInteractable _currentInteractable = default;
        private IDamageable _damageable = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            Hide();
        }

        private void OnEnable()
        {
            this.EventStartListening();
        }

        private void OnDisable()
        {
            this.EventStopListening();
        }

        private void OnDestroy()
        {
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= UpdateHealthAfterTakingDamage;
            }
        }

        #endregion

        #region Private methods

        private void Detect(BaseInteractable interactable)
        {
            Show(interactable);
        }

        private void StopDetection()
        {
            // Unsubscribe previous damageable target if there was one
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= UpdateHealthAfterTakingDamage;
            }

            Hide();
        }

        private void Show(BaseInteractable interactable)
        {
            _currentInteractable = interactable;

            if (_canDebug)
            {
                _txtType.enabled = true;
                _txtType.text = interactable.Priority.ToString();
            }
            else
            {
                _txtType.enabled = false;
            }

            _txtName.text = GetInteractableName(interactable);

            // Unsubscribe previous damageable target if there was one
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= UpdateHealthAfterTakingDamage;
            }

            _damageable = interactable.transform.GetComponent<IDamageable>();

            if (_damageable != null)
            {
                _damageable.OnTakeDamage += UpdateHealthAfterTakingDamage;

                ShowHealth();
            }
            else
            {
                HideHealth();
            }

            if (_canDebug)
            {
                Debug.Log($"[UIInteractableInformation] - <color=green>SHOW</color> : {interactable.transform.name}");
            }

            // Show HUD only if it is hidden
            if (!_canvas.enabled)
            {
                _canvas.enabled = true;
            }
        }

        private string GetInteractableName(BaseInteractable interactable)
        {
            /*var entity = interactable.GetComponent<Entity>();
            if (entity != null) return entity.Data.DisplayName;

            var pickable = interactable.GetComponent<ViewItemPicker>();
            if (pickable != null) return pickable.ItemId;

            var damageable = interactable.GetComponent<DamageableProp>();
            if (damageable != null) return damageable.UID;

            var pickerInteractable = interactable as PickerItemInteractable;

            if (pickerInteractable != null) return pickerInteractable.Data.DisplayName;

            return "NO_NAME";*/

            return interactable.DisplayName;
        }

        private void Hide()
        {
            _currentInteractable = null;

            _canvas.enabled = false;

            if (_canDebug)
            {
                Debug.Log("[UIInteractableinformation] - <color=red>HIDE</color>");
            }
        }

        private void UpdateHealthAfterTakingDamage(object sender, CustomEventArgs.DamageEventArgs e)
        {
            ShowHealth();
        }

        private void ShowHealth()
        {
            if (_damageable == null)
            {
                return;
            }

            _fillProgressBar.fillAmount = _damageable.Health / _damageable.MaxHealth;

            _txtHealth.text = $"{Mathf.FloorToInt(_damageable.Health)}";
        }

        private void HideHealth()
        {
            _fillProgressBar.fillAmount = 0;

            _txtHealth.text = string.Empty;
        }

        #endregion

        #region GameEventListener<GameLevelEvent> implementation

        public virtual void OnGameEvent(GameLevelEvent eventData)
        {
            switch (eventData.EventType)
            {
                case GameLevelEventType.InteractableChanged:
                    Refresh(eventData.Payload);
                    break;
            }
        }

        private void Refresh(object[] payload)
        {
            if (payload == null) return;

            if (payload.Length == 0) return;

            // Skip it if priority is different to the priority expected
            var priority = (Enums.InteractablePriorities)payload[1];

            var interactable = payload[0] as BaseInteractable;

            // If there is no previous detection
            if (_currentInteractable == null)
            {
                // Check if it has to be hidden or shown
                if (interactable == null)
                {
                    Hide();
                }
                else
                {
                    Show(interactable);
                }
            }
            else
            {
                // If something was un detected but the current one is detected yet
                if (interactable == null)
                {
                    if (_currentInteractable.IsDetected) return;

                    Hide();
                }
                else
                {
                    // If the new detection is low priority than current one skip it
                    if (interactable.Priority < _currentInteractable.Priority) return;

                    Show(interactable);
                }
            }
        }

        #endregion
    }
}