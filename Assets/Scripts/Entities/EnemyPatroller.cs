using System;
using System.ComponentModel;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    [RequireComponent(typeof(NavMeshAgent))]
    [RequireComponent(typeof(FieldOfView))]
    public class EnemyPatroller : MonoBehaviour, IDamageable
    {
        #region Inspector properties

        [SerializeField] private EnemyData _templateData = null;
        [SerializeField] private EnemyData _data = null;
        [SerializeField] private Enums.EnemyStates _state;
        [SerializeField] private Transform[] _points = null;
        [SerializeField] private Animator _animator = null;

        #endregion

        #region Private properties

        private StateMachine _stateMachine = null;
        private bool _isTakingDamage = false;
        private FieldOfView _fieldOfView = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            // Generate a copy of template configuration data
            _data = ScriptableObject.Instantiate(_templateData);

            // Init Health data
            Health = _data.CurrentHealth;
            MaxHealth = _data.MaxHealth;

            NavMeshAgent navMeshAgent = GetComponent<NavMeshAgent>();

            // Field of View
            _fieldOfView = GetComponent<FieldOfView>();
            _fieldOfView.Radius = _data.RadiusDetection;
            _fieldOfView.ViewAngle = _data.AngleDetection;
            _fieldOfView.TargetMask = _data.TargetMaskDetection;
            _fieldOfView.ObstacleMask = _data.ObstacleMaskDetection;

            // Create State Machine
            _stateMachine = new StateMachine();
            _stateMachine.PropertyChanged += StateMachineHasChanged;

            // Create states
            var stateIdle = new Idle(_data);
            var stateMoveBetweenPoints = new MoveBetweenPoints(_data, navMeshAgent, _animator, _points);
            var stateChasing = new ChaseTarget(_data, _fieldOfView, navMeshAgent, _animator);
            var stateAttacking = new Attack(_data, transform, _fieldOfView, _animator);
            var stateTakingDamage = new TakeDamage(_data, _animator);
            var stateDead = new Dead(_data, _animator);

            #region Create transitions from "IDLE" state

            _stateMachine.AddTransition(stateIdle, stateMoveBetweenPoints, () => stateIdle.RemainingWaitingTime == 0);

            #endregion

            #region Create transitions from "MOVE BETWEEN POINTS" state

            _stateMachine.AddTransition(stateMoveBetweenPoints, stateIdle, () => stateMoveBetweenPoints.IsWaiting);

            #endregion

            #region Create transitions from "CHASE TARGET" state

            // If it isn't chasing and not in attack range or there isn't target any more
            _stateMachine.AddTransition(stateChasing, stateIdle, () => (!stateChasing.IsChasing && !stateAttacking.IsOnRange) || _fieldOfView.NearestTarget == null);

            #endregion

            #region Create transitions from "ATTACK" state

            // If is attacking but target is out of attack range and there isn't any target anymore
            _stateMachine.AddTransition(stateAttacking, stateIdle, () => (!stateAttacking.IsOnRange) && _fieldOfView.NearestTarget == null);

            // If is attacking but target is in detection area yet
            _stateMachine.AddTransition(stateAttacking, stateChasing, () => (!stateAttacking.IsOnRange) && _fieldOfView.NearestTarget != null);

            #endregion

            #region Create transitions from "TAKING DAMAGE" state

            // Taking damage over -> Patrolling
            _stateMachine.AddTransition(stateTakingDamage, stateMoveBetweenPoints, () => !IsDead && !stateTakingDamage.IsRecovering && _fieldOfView.NearestTarget == null);

            // Taking damage over -> Chasing
            _stateMachine.AddTransition(stateTakingDamage, stateChasing, () => !IsDead && !stateTakingDamage.IsRecovering && _fieldOfView.NearestTarget != null && !IsOnAttackingRange());

            // Taking damage over -> Attacking
            _stateMachine.AddTransition(stateTakingDamage, stateAttacking, () => !IsDead && !stateTakingDamage.IsRecovering && _fieldOfView.NearestTarget != null && IsOnAttackingRange());

            #endregion

            #region Create transitions from ANY state

            // If it is dead
            _stateMachine.AddAnyTransition(stateDead, () => IsDead);

            // If there is a detected target and not in attacking range then start Chasing it
            _stateMachine.AddAnyTransition(stateChasing, () => !IsDead && !stateTakingDamage.IsRecovering && _fieldOfView.NearestTarget != null && !IsOnAttackingRange() );

            // If it is in range to Attack
            _stateMachine.AddAnyTransition(stateAttacking, () => !IsDead && !stateTakingDamage.IsRecovering && _fieldOfView.NearestTarget != null && IsOnAttackingRange());

            // If takes damage then move to this state
            _stateMachine.AddAnyTransition(stateTakingDamage, () => !IsDead && _isTakingDamage);

            #endregion

            // Set last state as "DEAD"
            _stateMachine.SetLastState(stateDead);

            // Start State machine at "IDLE" state
            _stateMachine.SetState(stateIdle);
        }

        private void OnDestroy()
        {
            _stateMachine.PropertyChanged -= StateMachineHasChanged;

            ScriptableObject.Destroy(_data);
        }

        private void Update()
        {
            _stateMachine.Tick();
        }

        /// <summary>
        /// Each time State Machine is updated then it is notified
        /// </summary>
        /// <param name="sender"></param>
        /// <param name="e"></param>
        private void StateMachineHasChanged(object sender, PropertyChangedEventArgs e)
        {
            _state = _stateMachine.GetCurrentState();
        }

        /// <summary>
        /// Detects if distance to the target is near enough to attack it. If there isn't any target then it isn't on attack range
        /// </summary>
        /// <returns></returns>
        private bool IsOnAttackingRange()
        {
            if (_fieldOfView.NearestTarget != null)
            {
                // Check distance to detect if it continues in the attacking range
                var diff = (_fieldOfView.NearestTarget.position - transform.position);
                float distanceMagnitude = diff.magnitude;

                if (distanceMagnitude <= _data.RadiusAttack)
                    return true;
            }

            return false;
        }

        #endregion

        #region IDamageable implementation

        public float MaxHealth { get; set; }

        public float Health { get; set; }

        public bool IsDead => Health == 0;

        public event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        public event EventHandler<CustomEventArgs.EntityDeadEventArgs> OnDied;
        public event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;

        public async void TakeDamage(float value)
        {
            if (IsDead)
                return;

            int healthBeforeDamage = (int)Health;

            Health = Mathf.Clamp(Health - value, 0, MaxHealth);

            Debug.Log($"<color=red>{name} takes damage, before damage: {healthBeforeDamage}, current Health: {Health}</color>");

            if (Health == 0)
            {
                var entityDeadEventArgs = new CustomEventArgs.EntityDeadEventArgs(_data.UID);

                // Communicates to each part of this entity about the situation
                OnDied?.Invoke(this, entityDeadEventArgs);

                // Communicates to the rest of the game about this situation
                GameEvents.OnEntityHasDied?.Invoke(this, entityDeadEventArgs);

                Debug.Log($"<color=red>{name} <b>IS DEAD</b></color>");

                _isTakingDamage = false;
            }
            else
            {
                OnTakeDamage?.Invoke(this, new CustomEventArgs.DamageEventArgs(_data.UID, value));

                _isTakingDamage = true;

                await Task.Delay(500);

                _isTakingDamage = false;
            }
        }

        public void Heal(float value)
        {
            Health = Mathf.Clamp(Health + value, 0, MaxHealth);

            OnHealed?.Invoke(this, new CustomEventArgs.HealEventArgs(_data.UID, value));
        }

        #endregion
    }
}