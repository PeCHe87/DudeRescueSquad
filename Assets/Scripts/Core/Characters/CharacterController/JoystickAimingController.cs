using DudeResqueSquad;
using System;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class JoystickAimingController : MonoBehaviour, ICharacterController
    {
        #region Inspector properties

        [SerializeField] private Joystick _joystick = null;

        #endregion

        #region Public properties

        public Joystick Joystick => _joystick;

        #endregion

        #region ICharacterMovement Implementation

        public event EventHandler<CustomEventArgs.MovementEventArgs> OnStartMoving;
        public event EventHandler OnStopMoving;

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

        public float DeadZone()
        {
            return _joystick.DeadZone;
        }

        #endregion
    }
}