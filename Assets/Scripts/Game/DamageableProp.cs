using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;
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
        private float _health = 0;

        private void Awake()
        {
            _collider = GetComponent<Collider>();
        }

        private void Start()
        {
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

        public float Health
        {
            get => _health;
            set { _health = value; OnPropertyChanged(nameof(Health)); }
        }

        public bool IsDead => Health == 0;

        public bool IsTakingDamage { get; private set; }

        public event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        public event EventHandler<CustomEventArgs.EntityDeadEventArgs> OnDied;
        public event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;
        public event PropertyChangedEventHandler PropertyChanged;

        public void TakeDamage(float value)
        {
            ShowHitEffect();
            
            if (IsDead)
            {
                return;
            }
            
            Health = Mathf.Clamp(Health - value, 0, MaxHealth);
            
            if (Health == 0)
            {
                if (_art != null)
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
        
        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion
    }
}