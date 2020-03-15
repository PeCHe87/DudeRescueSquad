using System;
using static DudeResqueSquad.Character;

namespace DudeResqueSquad
{
    public interface ICharacterMovement
    {
        event EventHandler OnDoAction;
        event EventHandler<MovementEventArgs> OnStartMoving;
        event EventHandler OnStopMoving;

        float Horizontal();
        float Vertical();
    }
}