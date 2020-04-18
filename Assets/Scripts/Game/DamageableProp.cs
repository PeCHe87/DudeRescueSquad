using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeResqueSquad
{
    public class DamageableProp : MonoBehaviour, IDamageable
    {
        [SerializeField] private float _initialHealth = 0;

        private void Start()
        {
            Health = _initialHealth;
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
                OnDied?.Invoke(this, EventArgs.Empty);
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