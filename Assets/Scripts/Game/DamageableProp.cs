using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class DamageableProp : MonoBehaviour, IDamageable
    {
        [SerializeField] private string _uid = string.Empty;
        [SerializeField] private float _initialHealth = 0;
        [SerializeField] private GameObject _art = null;
        [SerializeField] private GameObject _hitEffect = null;
        [SerializeField] private Transform _hitPivot = null;

        private Collider _collider = null;

        private void Awake()
        {
            _collider = GetComponent<Collider>();

            Health = MaxHealth = _initialHealth;
        }

        private void ShowHitEffect()
        {
            if (_hitEffect == null)
                return;

            GameObject hit = Instantiate(_hitEffect, _hitPivot);
            hit.transform.localPosition = Vector3.zero;
            hit.transform.SetParent(transform.parent);
        }

        #region IDamageable implementation

        public float MaxHealth { get; set; }

        public float Health { get; set; }

        public bool IsDead => Health == 0;

        public event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        public event EventHandler<CustomEventArgs.EntityDeadEventArgs> OnDied;
        public event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;

        public void TakeDamage(float value)
        {
            Health = Mathf.Clamp(Health - value, 0, MaxHealth);

            ShowHitEffect();

            if (Health == 0)
            {
                _art.SetActive(false);

                _collider.enabled = false;

                OnDied?.Invoke(this, new CustomEventArgs.EntityDeadEventArgs(_uid));
            }
            else
                OnTakeDamage?.Invoke(this, new CustomEventArgs.DamageEventArgs(_uid, value));
        }

        public void Heal(float value)
        {
            Health = Mathf.Clamp(Health + value, 0, MaxHealth);

            OnHealed?.Invoke(this, new CustomEventArgs.HealEventArgs(_uid, value));
        }

        #endregion
    }
}