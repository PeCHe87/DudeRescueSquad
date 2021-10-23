﻿using DudeRescueSquad.Core.Inventory;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    /// <summary>
    /// Manage all stuff related with character rotation orientation.
    /// When character doesn't have any weapon equipped, then it rotates toward the input direction.
    /// When a weapon is equipped, if the weapon can override the character orientation, the final orientation
    /// depends on the weapon detection and if there is a detected target to point towards.
    /// </summary>
    public class CharacterAbilityOrientation : MonoBehaviour, ICharacterAbility
    {
        #region Inspector properties

        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private Transform _model = null;
        [SerializeField] private bool _instantRotation = false;
        [SerializeField] private float _turnSmoothTime = 0.1f;
        [SerializeField] private float _turnFollowingWeaponTargetSmoothTime = 0.05f;

        #endregion

        public Vector3 ModelDirection => _model.forward;
        public Transform Model => _model;

        #region Private properties

        private ICharacterController _movementController = null;
        private ICharacterController _aimingController = null;
        private Character _character = null;
        private CharacterAbilityHandleWeapon _characterAbilityHandleWeapon = null;
        private bool _wasInitialized = false;
        private float _targetAngle = 0;
        private float _turnSmoothVelocity = 0;
        private Vector3 _currentRotation = Vector3.zero;

        #endregion

        public Vector3 CurrentRotation => _currentRotation;

        #region ICharacterAbility implementation

        public void Initialization()
        {
            this._movementController = GetComponent<JoystickCharacterController>();
            this._aimingController = GetComponent<JoystickAimingController>();

            this._character = GetComponent<Character>();

            this._characterAbilityHandleWeapon = GetComponent<CharacterAbilityHandleWeapon>();

            this._wasInitialized = true;
        }

        public bool IsEnabled()
        {
            return _isEnabled;
        }

        public void EarlyProcessAbility() { }

        public void Process()
        {
            CheckOrientation();
            Rotate();
        }

        public bool WasInitialized()
        {
            return _wasInitialized;
        }

        #endregion

        #region Private methods

        /*private void Awake()
        {
            this._controller = GetComponent<ICharacterController>();
        }*/

        /// <summary>
        /// Check the orientation based on the current equipped item (if corresponds) or the input direction
        /// </summary>
        private void CheckOrientation()
        {
            // Check aiming controller input, if dead zone then skip it, else use it to orientate the character
            if (_aimingController.Direction().magnitude > 0)
            {
                CheckOrientationBasedOnAiming();
                return;
            }

            // Check if there is a weapon equipped that can override the orientation based on its target
            if (_characterAbilityHandleWeapon.IsEquipped)
            {
                // Check if current weapon type is assault and has target detected
                if (_characterAbilityHandleWeapon.CurrentWeapon.WeaponType == Enums.ItemTypes.WEAPON_ASSAULT && _characterAbilityHandleWeapon.HasDetectedTarget)
                {
                    CheckOrientationBasedOnWeapon();
                    return;
                }
            }

            CheckOrientationBasedOnMovement();
        }

        /// <summary>
        /// Gets the orientation based on the current item and its target detection
        /// </summary>
        /// <param name="weapon"></param>
        private void CheckOrientationBasedOnWeapon()
        {
            var targetPosition = _characterAbilityHandleWeapon.CurrentTarget.position;

            targetPosition.y = transform.position.y;

            var dir = targetPosition - transform.position;

            var angleGoal = Mathf.Atan2(dir.x, dir.z) * Mathf.Rad2Deg;

            _targetAngle = Mathf.SmoothDampAngle(_model.eulerAngles.y, angleGoal, ref _turnSmoothVelocity, _turnFollowingWeaponTargetSmoothTime);

            Debug.DrawRay(transform.position, dir * 4, Color.green);
        }

        /// <summary>
        /// Gets the orientation based on the input direction
        /// </summary>
        private void CheckOrientationBasedOnMovement()
        {
            var dir = _movementController.Direction();

            if (_instantRotation)
            {
                _targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            }
            else
            {
                var angleGoal = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                if (angleGoal == 0)
                {
                    _targetAngle = 0;
                    return;
                }

                _targetAngle = Mathf.SmoothDampAngle(_model.eulerAngles.y, angleGoal, ref _turnSmoothVelocity, _turnSmoothTime);
            }
        }

        /// <summary>
        /// Gets the orientation based on the input direction
        /// </summary>
        private void CheckOrientationBasedOnAiming()
        {
            var dir = _aimingController.Direction();

            if (_instantRotation)
            {
                _targetAngle = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;
            }
            else
            {
                var angleGoal = Mathf.Atan2(dir.x, dir.y) * Mathf.Rad2Deg;

                if (angleGoal == 0)
                {
                    _targetAngle = 0;
                    return;
                }

                _targetAngle = Mathf.SmoothDampAngle(_model.eulerAngles.y, angleGoal, ref _turnSmoothVelocity, _turnSmoothTime);
            }
        }

        /// <summary>
        /// Rotates the character towards the found angle if there is one
        /// </summary>
        private void Rotate()
        {
            if (_targetAngle == 0) return;

            _model.rotation = Quaternion.Euler(0, _targetAngle, 0);

            _currentRotation = _model.forward;
        }

        #endregion

        #region Public methods

        public void Disable()
        {
            _isEnabled = false;
        }

        public void Enable()
        {
            _isEnabled = true;
        }

        #endregion
    }
}