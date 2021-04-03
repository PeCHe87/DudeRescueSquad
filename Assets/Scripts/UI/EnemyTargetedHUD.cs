using TMPro;
using UnityEngine;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class EnemyTargetedHUD : MonoBehaviour
    {
        #region Inspector

        [SerializeField] private Image _fillProgressBar = default;
        [SerializeField] private TextMeshProUGUI _txtName, _txtHealth = default;
        [SerializeField] private bool _canDebug = true;

        #endregion

        #region  Private properties
        
        private Canvas _canvas = default;
        private string _entityId = default;
        private float _oldHealth = default;
        private float _maxHealth;
        private IDamageable _damageable = default;

        #endregion

        #region Private methods

        private void Awake()
        {
            _canvas = GetComponent<Canvas>();
            _canvas.enabled = false;

            GameEvents.OnEnemyTargetChanged += EnemyTargetChanged;
        }

        private void OnDestroy()
        {
            GameEvents.OnEnemyTargetChanged -= EnemyTargetChanged;

            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= UpdateHealthAfterTakingDamage;
                _damageable.OnDied -= HideAfterDying;
            }
        }

        private void EnemyTargetChanged(object sender, CustomEventArgs.EnemyTargetedArgs e)
        {
            if (e.target == null)
            {
                Hide();
            }
            else
            {
                Show(e.target);
            }
        }

        private void Show(Transform target)
        {
            Entity entity = target.GetComponent<Entity>();

            if (entity == null)
            {
                Hide();
                return;
            }
            
            if (!string.IsNullOrEmpty(_entityId) && _entityId.Equals(entity.UID))
            {
                return;
            }
            
            _entityId = entity.UID;

            _txtName.text = entity.Data.DisplayName;
            
            // Unsubscribe previous damageable target if there was one
            if (_damageable != null)
            {
                _damageable.OnTakeDamage -= UpdateHealthAfterTakingDamage;
                _damageable.OnDied -= HideAfterDying;
            }
            
            _damageable = target.GetComponent<IDamageable>();

            // Subscribe the damageable component for the detected target
            if (_damageable == null)
            {
                Hide();
                return;
            }

            _damageable.OnTakeDamage += UpdateHealthAfterTakingDamage;
            _damageable.OnDied += HideAfterDying;

            _oldHealth = _damageable.Health;
            _maxHealth = _damageable.MaxHealth;

            ShowHealth();

            if (_canDebug)
            {
                Debug.Log($"<color=green>SHOW</color> EnemyTargetedHUD for entity: {_entityId}");
            }
            
            // Show HUD only if it is hidden
            if (!_canvas.enabled)
            {
                _canvas.enabled = true;
            }
        }

        private void HideAfterDying(object sender, CustomEventArgs.EntityDeadEventArgs e)
        {
            Hide();
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
            
            _txtHealth.text = $"{Mathf.FloorToInt(_damageable.Health)}/{Mathf.FloorToInt(_damageable.MaxHealth)}";
        }

        private void Hide()
        {
            _entityId = string.Empty;

            _canvas.enabled = false;

            if (_canDebug)
            {
                Debug.Log("<color=red>HIDE</color> EnemyTargetedHUD");
            }
        }

        #endregion
    }
}