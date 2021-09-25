using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class CharacterMovementJoystick : MonoBehaviour, ICharacterMovement
    {
        [SerializeField] private Joystick _joystick = null;

        #region Private properties

        private bool _isDragging = false;
        private bool _isMoving = false;
        private Character _character = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            GameEvents.OnProcessAction += ProcessAction;
        }

        private void OnDestroy()
        {
            GameEvents.OnProcessAction -= ProcessAction;
        }

        private void Start()
        {
            _character = GetComponent<Character>();
        }

        private void Update()
        {
            if (_joystick.Vertical != 0 || _joystick.Horizontal != 0)
            {
                if (!_isDragging)
                {
                    _isDragging = true;

                    OnStartMoving?.Invoke(this, new CustomEventArgs.MovementEventArgs(_joystick.Horizontal, _joystick.Vertical));
                }
            }
            else
            {
                if (_isDragging)
                {
                    _isDragging = false;

                    OnStopMoving?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        private bool CanPerformAttack()
        {
            if (_character == null)
                return true;

            if (_character.State.CurrentState == Enums.CharacterStates.RUNNING)
                return false;

            return true;
        }

        private void ProcessAction(object sender, EventArgs e)
        {
            if (_character.Data.CurrentWeaponEquipped == null)
                return;

            // Skip it if current weapon is auto fire
            if (_character.Data.CurrentWeaponEquipped.AutoFire)
                return;

            // Check if can performs this action based on current character state
            if (!CanPerformAttack())
                return;

            OnDoAction?.Invoke(this, new CustomEventArgs.TouchEventArgs(Vector3.zero));
        }

        #endregion

        public void Attack()
        {
            if (_character.Data.CurrentWeaponEquipped == null)
                return;

            // Skip it if current weapon is auto fire
            if (_character.Data.CurrentWeaponEquipped.AutoFire)
                return;

            // Check if can performs this action based on current character state
            if (!CanPerformAttack())
                return;

            OnDoAction?.Invoke(this, new CustomEventArgs.TouchEventArgs(Vector3.zero));
        }

        public void PressActionButton()
        {

            if (_character.Data.CurrentWeaponEquipped == null)
                return;

            // Skip it if current weapon isn't auto fire
            if (!_character.Data.CurrentWeaponEquipped.AutoFire)
                return;

            if (!CanPerformAttack())
                return;

            //Debug.Log($"<color=green>Start <b>ACTION</b></color> - {_character.Data.CurrentWeaponEquipped.DisplayName}");

            OnStartAction?.Invoke(this, EventArgs.Empty);
        }

        public void ReleaseActionButton()
        {
            if (_character.Data.CurrentWeaponEquipped == null)
                return;

            // Skip it if current weapon isn't auto fire
            if (!_character.Data.CurrentWeaponEquipped.AutoFire)
                return;

            Debug.Log($"<color=red>Stop <b>ACTION</b></color> - {_character.Data.CurrentWeaponEquipped.DisplayName}");

            //OnStopAction?.Invoke(this, EventArgs.Empty);
            GameEvents.OnStopAction?.Invoke(this, EventArgs.Empty);
        }

        #region ICharacterMovement Implementation

        public event EventHandler<CustomEventArgs.TouchEventArgs> OnDoAction;
        public event EventHandler<CustomEventArgs.MovementEventArgs> OnStartMoving;
        public event EventHandler OnStopMoving;
        public event EventHandler OnStartAction;

        public float Horizontal()
        {
            return _joystick.Horizontal;
        }

        public float Vertical()
        {
            return _joystick.Vertical;
        }

        public Vector3 Direction()
        {
            return _joystick.Direction;
        }

        #endregion
    }
}