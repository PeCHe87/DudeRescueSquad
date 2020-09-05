using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public interface ICharacterMovement
    {
        event EventHandler<CustomEventArgs.TouchEventArgs> OnDoAction;
        event EventHandler<CustomEventArgs.MovementEventArgs> OnStartMoving;
        event EventHandler OnStopMoving;
        event EventHandler OnStartAction;

        float Horizontal();
        float Vertical();

        Vector3 Direction();
    }
}