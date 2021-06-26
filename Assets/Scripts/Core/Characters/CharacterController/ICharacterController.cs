using DudeResqueSquad;
using System;
using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public interface ICharacterController
    {
        event EventHandler<CustomEventArgs.MovementEventArgs> OnStartMoving;
        event EventHandler OnStopMoving;

        float Horizontal();
        float Vertical();

        Vector3 Direction();
    }
}