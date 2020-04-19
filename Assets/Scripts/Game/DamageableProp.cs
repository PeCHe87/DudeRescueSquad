using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class DamageableProp : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _initialHealth = 0;
        [SerializeField] private GameObject _art = null;

        private Collider _collider = null;

        private void Start()
        {
            _collider = GetComponent<Collider>();

            Health = MaxHealth = _initialHealth;
        }

        #region IDamageable implementation

        public float MaxHealth { get; set; }

        public float Health { get; set; }

        public bool IsDead => Health == 0;

        public event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        public event EventHandler OnDied;
        public event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;

        public void TakeDamage(float value)
        {
            Health = Mathf.Clamp(Health - value, 0, MaxHealth);

            if (Health == 0)
            {
                _art.SetActive(false);

                _collider.enabled = false;

                OnDied?.Invoke(this, EventArgs.Empty);
            }
            else
                OnTakeDamage?.Invoke(this, new CustomEventArgs.DamageEventArgs(value));
        }

        public void Heal(float value)
        {
            Health = Mathf.Clamp(Health + value, 0, MaxHealth);

            OnHealed?.Invoke(this, new CustomEventArgs.HealEventArgs(value));
        }

        #endregion
    }
}