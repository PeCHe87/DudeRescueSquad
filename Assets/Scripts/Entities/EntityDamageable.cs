using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityDamageable : MonoBehaviour, IDamageable
    {
        #region Private properties

        private string _uid = string.Empty;
        private float _timeForRecoveringAfterDamage = 0;
        private Collider _collider = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void StopTakingDamage()
        {
            IsTakingDamage = false;
        }

        #endregion

        #region Public methods

        public void Init(int health, int maxHealth, string uid, float timeForRecoveringAfterDamage)
        {
            Health = health;
            MaxHealth = maxHealth;
            _uid = uid;
            _timeForRecoveringAfterDamage = timeForRecoveringAfterDamage;
        }

        #endregion

        #region IDamageable implementation

        public float MaxHealth { get; set; }

        public float Health { get; set; }

        public bool IsDead => Health == 0;

        public bool IsTakingDamage { get; private set; }

        public event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        public event EventHandler<CustomEventArgs.EntityDeadEventArgs> OnDied;
        public event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;
        public event PropertyChangedEventHandler PropertyChanged;

        public void TakeDamage(float value)
        {
            Health = Mathf.Clamp(Health - value, 0, MaxHealth);

            if (Health == 0)
            {
                _collider.enabled = false;

                OnDied?.Invoke(this, new CustomEventArgs.EntityDeadEventArgs(_uid));
            }
            else
            {
                IsTakingDamage = true;

                OnTakeDamage?.Invoke(this, new CustomEventArgs.DamageEventArgs(_uid, value));

                Invoke("StopTakingDamage", _timeForRecoveringAfterDamage);
            }
        }

        public void Heal(float value)
        {
            Health = Mathf.Clamp(Health + value, 0, MaxHealth);

            OnHealed?.Invoke(this, new CustomEventArgs.HealEventArgs(_uid, value));
        }
        
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}