using DudeRescueSquad.Core.Inventory;
using DudeRescueSquad.Core.Inventory.Items.Weapons;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class Character : MonoBehaviour, DudeResqueSquad.IDamageable
    {
        [SerializeField] private float _maxHealth = 0;
        [SerializeField] private float _initialHealth = 0;

        #region Private properties

        private IEquipment _equipment = null;
        private ICharacterAbility[] _abilities = null;
        private float _health = 0;

        #endregion

        #region Private methods

        private void Awake()
        {
            // Equipment
            _equipment = new CharacterEquipment();

            // Abilities
            _abilities = this.GetComponents<ICharacterAbility>();

            foreach (var ability in _abilities)
            {
                if (!ability.IsEnabled()) continue;

                ability.Initialization();
            }

            Debug.Log($"<color=yellow>Character abilities</color>: {_abilities.Length}");
        }

        private void Start()
        {
            MaxHealth = _maxHealth;
            Health = _initialHealth;
        }

        private void Update()
        {
            EveryFrame();
        }

        #endregion

        #region Public methods

        public bool HasWeaponEquipped()
        {
            if (!_equipment.HasItemEquipped()) return false;

            var item = _equipment.GetCurrentItem() as WeaponItem;

            return item != null;
        }

        public WeaponItem GetWeaponEquipped()
        {
            return _equipment.GetCurrentItem() as WeaponItem;
        }

        #endregion

        #region Protected methods

        /// <summary>
		/// We do this every frame. This is separate from Update for more flexibility.
		/// </summary>
		protected virtual void EveryFrame()
        {
            // we process our abilities
            EarlyProcessAbilities();
            ProcessAbilities();
            
            // TODO: 
            // LateProcessAbilities();

            // we send our various states to the animator.		 
            // UpdateAnimators();
        }

        /// <summary>
		/// Calls all registered abilities' Early Process methods
		/// </summary>
		protected virtual void EarlyProcessAbilities()
        {
            foreach (var ability in _abilities)
            {
                if (ability.IsEnabled() && ability.WasInitialized())
                {
                    ability.EarlyProcessAbility();
                }
            }
        }

        /// <summary>
		/// Calls all registered abilities' Process methods
		/// </summary>
		protected virtual void ProcessAbilities()
        {
            foreach (var ability in _abilities)
            {
                if (!ability.IsEnabled() || !ability.WasInitialized()) continue;

                ability.Process();
            }
        }

        #endregion

        #region IDamageable implementation (Move all of this to another component)

        public float MaxHealth { get; set; }

        public float Health
        {
            get => _health;
            set { _health = value; OnPropertyChanged(nameof(Health)); }
        }

        public bool IsDead => Health == 0;

        public bool IsTakingDamage { get; private set; }

        public event System.EventHandler<DudeResqueSquad.CustomEventArgs.DamageEventArgs> OnTakeDamage;
        public event System.EventHandler<DudeResqueSquad.CustomEventArgs.EntityDeadEventArgs> OnDied;
        public event System.EventHandler<DudeResqueSquad.CustomEventArgs.HealEventArgs> OnHealed;
        public event PropertyChangedEventHandler PropertyChanged;

        public void TakeDamage(float value)
        {
            if (IsDead) return;

            Health = Mathf.Clamp(Health - value, 0, MaxHealth);

            if (Health == 0)
            {
                OnDied?.Invoke(this, new DudeResqueSquad.CustomEventArgs.EntityDeadEventArgs(string.Empty));
            }
            else
                OnTakeDamage?.Invoke(this, new DudeResqueSquad.CustomEventArgs.DamageEventArgs(string.Empty, value));
        }

        public void Heal(float value)
        {
            //
        }

        public void OnPropertyChanged([CallerMemberName] string name = null)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        }

        #endregion

        #region Debug methods

        [ContextMenu("Apply Damage for Debug")]
        private void ApplyDamage()
        {
            TakeDamage(Random.Range(5f, 10f));
        }

        #endregion
    }
}