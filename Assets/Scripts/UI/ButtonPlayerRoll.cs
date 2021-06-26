using System;
using UnityEngine;
using UnityEngine.UI;

namespace DudeResqueSquad
{
    public class ButtonPlayerRoll : MonoBehaviour
    {
        #region Private properties

        private Button _btn = null;

        #endregion
        
        #region Private methods

        private void Awake()
        {
            _btn = GetComponent<Button>();
        }

        #endregion
        
        #region Public methods

        public void DoAction()
        {
            GameEvents.OnStartPlayerRolling?.Invoke(this, EventArgs.Empty);
        }
        
        #endregion
    }
}