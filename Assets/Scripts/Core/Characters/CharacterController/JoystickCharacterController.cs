using DudeResqueSquad;
using System;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class JoystickCharacterController : MonoBehaviour, ICharacterController
    {
        [SerializeField] private Joystick _joystick = null;

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

        #endregion
    }
}