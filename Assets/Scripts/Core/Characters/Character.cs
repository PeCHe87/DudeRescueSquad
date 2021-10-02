using System.ComponentModel;
using System.Runtime.CompilerServices;
using DudeResqueSquad;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class Character : MonoBehaviour, IDamageable
    {
        #region Inspector properties

        [SerializeField] private Enums.CharacterState _currentState = Enums.CharacterState.NONE;

        // TODO: this information below should be obtained from another side, like a initialization file or saving file, not a direct reference on inspector
        [Header("Health information")]
        [SerializeField] private float _maxHealth = 0;
        [SerializeField] private float _initialHealth = 0;

        [Header("Debug")]
        [SerializeField] private bool _canDebug = false;

        #endregion

        #region Private properties

        private ICharacterAbility[] _abilities = null;
        private float _health = 0;  // TODO: move to another component all character's health logic

        #endregion

        #region Public properties

        public DudeResqueSquad.Enums.CharacterState State => _currentState;

        #endregion

        #region Unity events

        private void Awake()
        {
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
            // TODO: all of this logic should be moved to an initialization method

            _currentState = Enums.CharacterState.IDLE;

            MaxHealth = _maxHealth;
            Health = _initialHealth;
        }

        private void Update()
        {
            EveryFrame();
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

        #region Public methods

        /// <summary>
        /// Updates the character's current state and process any change related with that change
        /// </summary>
        /// <param name="state"></param>
        internal void UpdateState(Enums.CharacterState state)
        {
            // Skip it if the new state is the same as it already is on
            if (_currentState == state) return;

            if (!IsHigherPriority(state)) return;

            // Update state to the new one
            _currentState = state;

            if (_canDebug)
            {
                Debug.Log($"Character -> UpdateState: {_currentState}");
            }

            // TODO: process everything related with current state change

            // TODO: update animation state
        }

        /// <summary>
        /// Character is informed about to stop a specific action and reset its state to idle if possible
        /// </summary>
        /// <param name="state"></param>
        internal void StopAction(Enums.CharacterState state)
        {
            if (_currentState == Enums.CharacterState.DEAD) return;

            _currentState = Enums.CharacterState.IDLE;

            if (_canDebug)
            {
                Debug.Log($"Character -> UpdateState: {_currentState}");
            }

            // TODO: process everything related with current state change

            // TODO: update animation state
        }

        /// <summary>
        /// Based on current character state, if character is dead or doing another stuff then it won't be able to start the action
        /// </summary>
        /// <returns></returns>
        internal bool CanStartAction(Enums.ActionType actionType)
        {
            if (_currentState == Enums.CharacterState.DEAD) return false;

            switch (actionType)
            {
                case Enums.ActionType.ATTACK:
                    return CheckIfAllowToStartAttack();

                case Enums.ActionType.DASH:
                    return CheckIfAllowToStartDash();

                case Enums.ActionType.MOVE:
                    return CheckIfAllowToStartMovement();
            }

            return true;
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

        #region Private methods

        /// <summary>
        /// Checks if based on the current state it is possible start an attack
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAllowToStartAttack()
        {
            if (_currentState == Enums.CharacterState.DASHING) return false;

            return true;
        }

        /// <summary>
        /// Checks if based on current state it is possible start a Dash movement
        /// </summary>
        /// <returns></returns>
        private bool CheckIfAllowToStartDash()
        {
            if (_currentState == Enums.CharacterState.ATTACKING) return false;

            return true;
        }

        private bool CheckIfAllowToStartMovement()
        {
            if (_currentState == Enums.CharacterState.DASHING) return false;

            return true;
        }

        /// <summary>
        /// Checks if the new state could override the current state
        /// </summary>
        /// <param name="newState"></param>
        /// <returns></returns>
        private bool IsHigherPriority(Enums.CharacterState newState)
        {
            if (_currentState == Enums.CharacterState.ATTACKING)
            {
                if (newState == Enums.CharacterState.MOVING) return false;
                if (newState == Enums.CharacterState.IDLE) return false;
            }

            return true;
        }

        #endregion

        #region Debug methods

        [ContextMenu("Apply Damage for Debug")]
        private void ApplyDamage()
        {
            TakeDamage(UnityEngine.Random.Range(5f, 10f));
        }

        #endregion
    }
}