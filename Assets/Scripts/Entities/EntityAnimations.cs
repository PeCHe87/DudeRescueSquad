using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityAnimations : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Animator _animator = null;

        #endregion

        #region Private properties

        // Bool possible states
        private bool _isWalking = false;
        private bool _isRunning = false;
        private bool _isAttacking = false;
        private bool _attack = false;
        private bool _dead = false;
        private bool _takeDamage = false;
        private bool _isAiming = false;

        // Animator IDs
        private int _isWalkingId = 0;
        private int _isRunningId = 0;
        private int _isAttackingId = 0;
        private int _attackId = 0;
        private int _deadId = 0;
        private int _takeDamageId = 0;
        private int _isAimingId = 0;

        #endregion

        #region Private Methods

        private void Awake()
        {
            _isWalkingId = Animator.StringToHash("isWalking");
            _isRunningId = Animator.StringToHash("isRunning");
            _isAttackingId = Animator.StringToHash("isAttacking");
            _attackId = Animator.StringToHash("attack");
            _deadId = Animator.StringToHash("dead");
            _takeDamageId = Animator.StringToHash("takeDamage");
            _isAimingId = Animator.StringToHash("isAiming");
        }

        private void UpdateParams()
        {
            #region Bool parameters

            _animator.SetBool(_isWalkingId, _isWalking);
            _animator.SetBool(_isRunningId, _isRunning);
            _animator.SetBool(_isAttackingId, _isAttacking);
            _animator.SetBool(_isAimingId, _isAiming);

            #endregion

            #region Triggers

            if (_attack)
            {
                _animator.SetTrigger(_attackId);

                _attack = false;
            }

            if (_dead)
            {
                _animator.SetTrigger(_deadId);

                _dead = false;
            }

            if (_takeDamage)
            {
                _animator.SetTrigger(_takeDamageId);

                _takeDamage = false;
            }

            #endregion
        }

        private void ResetState()
        {
            _isWalking = false;
            _isRunning = false;
            _dead = false;
            _takeDamage = false;
        }

        #endregion

        #region Public methods

        public void Idle()
        {
            ResetState();

            UpdateParams();
        }

        public void Walk()
        {
            ResetState();

            _isWalking = true;

            UpdateParams();
        }

        public void Run()
        {
            ResetState();

            _isRunning = true;

            UpdateParams();
        }

        public void Attack(bool loop = false)
        {
            _isAttacking = loop;

            _attack = true;

            UpdateParams();
        }

        public void StopAttack()
        {
            _isAttacking = false;

            _attack = false;

            UpdateParams();
        }

        public void Die()
        {
            ResetState();

            _dead = true;

            // Cancel aiming state
            _isAiming = false;

            // Cancel attacking
            _isAttacking = false;
            _attack = false;

            // Update all params
            UpdateParams();
        }

        public void TakeDamage()
        {
            ResetState();

            _takeDamage = true;

            UpdateParams();
        }

        /// <summary>
        /// Adds/Removes the state of aiming to the current animator
        /// </summary>
        /// <param name="isAiming"></param>
        public void Aiming(bool isAiming)
        {
            _isAiming = isAiming;

            UpdateParams();
        }

        /// <summary>
        /// Starts or Stops the reloading state of animator
        /// </summary>
        /// <param name="show"></param>
        public void Reloading(bool show)
        {
            // Stop attacking
            _isAttacking = false;
            _attack = false;

            // TODO:
            Debug.Log("<b>TODO:</b> <color=red>RELOADING ANIMATION</color>");
        }

        public void ProcessUpdate(Enums.EnemyStates state)
        {
            // Based on current state param update the animations
            /*if (state == Enums.EnemyStates.IDLE)
                Idle();
            else if (state == Enums.EnemyStates.PATROLLING)
                Walk();
            else if (state == Enums.EnemyStates.CHASING)
                Run();*/
            if (state == Enums.EnemyStates.TAKING_DAMAGE)
                TakeDamage();
            else if (state == Enums.EnemyStates.DEAD)
                Die();
            else if (state == Enums.EnemyStates.ATTACKING)
                Idle();
        }

        #endregion
    }
}