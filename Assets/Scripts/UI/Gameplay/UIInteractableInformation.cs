using DudeRescueSquad.Core;
using DudeRescueSquad.Core.Inventory.View;
using DudeResqueSquad;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DudeRescueSquad.UI
{
    public class UIInteractableInformation : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _fillProgressBar = default;
        [SerializeField] private TextMeshProUGUI _txtName, _txtHealth, _txtType = default;
        [SerializeField] private bool _canDebug = true;

        #endregion

        #region  Private properties

        private Canvas _canvas = default;
        private IDamageable _damageable = default;

        #endregion

        #region Unity events

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            Hide();

            InteractablesController.OnDetect += Detect;
            InteractablesController.OnStopDetection += StopDetection;
        }

        private void OnDestroy()
        {
            InteractablesController.OnDetect -= Detect;
            InteractablesController.OnStopDetection -= StopDetection;

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
            _txtType.text = interactable.Priority.ToString();

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
            var entity = interactable.GetComponent<Entity>();
            if (entity != null) return entity.Data.DisplayName;

            var pickable = interactable.GetComponent<ViewItemPicker>();
            if (pickable != null) return pickable.ItemId;

            var damageable = interactable.GetComponent<DamageableProp>();
            if (damageable != null) return damageable.UID;

            return "NO_NAME";
        }

        private void Hide()
        {
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
    }
}