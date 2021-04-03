using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityAnimationEvents : MonoBehaviour
    {
        public Action OnProcessAttack;

        #region Public methods

        public void AttackHit()
        {
            OnProcessAttack?.Invoke();
        }
        
        #endregion
    }
}