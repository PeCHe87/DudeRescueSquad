using UnityEngine;

namespace DudeRescueSquad.Core.Characters
{
    public class CharacterAbility : MonoBehaviour, ICharacterAbility
    {
        #region Inspector properties

        [SerializeField] private bool _isEnabled = false;

        #endregion

        #region Protected properties

        protected bool _wasInitialized = false;

        #endregion

        #region Public methods

        /// <summary>
        /// This method is only used to display a helpbox text at the beginning of the ability's inspector
        /// </summary>
        /// <returns></returns>
        public virtual string HelpBoxText() { return string.Empty; }

        #endregion

        #region ICharacterAbility implementation

        public bool IsEnabled()
        {
            return _isEnabled;
        }

        public bool WasInitialized()
        {
            return _wasInitialized;
        }

        public virtual void Initialization()
        {
            this._wasInitialized = true;
        }

        /// <summary>
        /// The first of the 3 passes you can have in your ability. Think of it as EarlyUpdate() if it existed
        /// </summary>
        public virtual void EarlyProcessAbility()
        {
            InternalHandleInput();
        }

        /// <summary>
		/// Internal method to check if an input manager is present or not
		/// </summary>
		protected virtual void InternalHandleInput()
        {
            //if (_inputManager == null) { return; }
            //_horizontalInput = _inputManager.PrimaryMovement.x;
            //_verticalInput = _inputManager.PrimaryMovement.y;
            HandleInput();
        }

        /// <summary>
		/// Called at the very start of the ability's cycle, and intended to be overridden, looks for input and calls methods if conditions are met
		/// </summary>
		protected virtual void HandleInput(){}

        /// <summary>
        /// Every frame we check if it's needed to update the ammo display
        /// </summary>
        public virtual void Process(){}

        #endregion
    }
}