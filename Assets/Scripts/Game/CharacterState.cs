using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class CharacterState : MonoBehaviour
    {
        public Action<Enums.CharacterStates> OnChanged;

        [SerializeField] private Enums.CharacterStates _currentState = Enums.CharacterStates.NONE;

        public Enums.CharacterStates CurrentState { get => _currentState; }

        public void SetState(Enums.CharacterStates state)
        {
            //if (state.Equals(_currentState))
            //    return;

            _currentState = state;

            OnChanged?.Invoke(_currentState);
        }
    }
}