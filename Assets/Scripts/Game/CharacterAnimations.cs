using System;
using UnityEngine;
using static UnityEngine.CullingGroup;

namespace DudeResqueSquad
{
    public class CharacterAnimations : MonoBehaviour
    {
        #region Private consts

        private const string _attackKey = "attack";
        private const string _runningKey = "running";

        #endregion

        #region Inspector properties

        [SerializeField] private Animator _anim = null;
        [SerializeField] private CharacterState _characterState = null;

        #endregion

        #region Private methods

        private void Awake()
        {
            if (_characterState != null)
            _characterState.OnChanged += CharacterStateChanged;

            _anim.SetBool(_runningKey, false);
        }

        private void OnDestroy()
        {
            if (_characterState != null)
                _characterState.OnChanged -= CharacterStateChanged;
        }

        private void CharacterStateChanged(CharacterState.CharacterStates state)
        {
            if (state == CharacterState.CharacterStates.IDLE)
                Idle();
            else if (state == CharacterState.CharacterStates.RUNNING)
                Run();
            else if (state == CharacterState.CharacterStates.ATTACKING)
                Attack();
        }

        private void Idle()
        {
            _anim.SetBool(_runningKey, false);
            _anim.ResetTrigger(_attackKey);
        }

        private void Run()
        {
            _anim.SetBool(_runningKey, true);
            _anim.ResetTrigger(_attackKey);
        }

        private void Attack()
        {
            _anim.SetTrigger(_attackKey);
        }

        #endregion
    }
}