using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class CharacterState : MonoBehaviour
    {
        public Action<CharacterStates> OnChanged;

        [Serializable]
        public enum CharacterStates { NONE, IDLE, RUNNING, ATTACKING }

        [SerializeField] private CharacterStates _currentState = CharacterStates.NONE;

        public void SetState(CharacterStates state)
        {
            if (state.Equals(_currentState))
                return;

            _currentState = state;

            OnChanged?.Invoke(_currentState);
        }
    }
}