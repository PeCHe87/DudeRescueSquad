using UnityEngine;

namespace DudeResqueSquad
{
    public class TakeDamage : IState
    {
        #region Public properties

        public bool IsRecovering { get; private set; }

        #endregion

        #region Private properties

        private EnemyData _data = null;
        private float _remainingTime = 0;

        #endregion

        public TakeDamage(EnemyData data, Animator animator)
        {
            _data = data;
        }

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.TAKING_DAMAGE;
        }

        public void Tick()
        {
            if (!IsRecovering)
                return;

            _remainingTime = Mathf.Clamp(_remainingTime - Time.deltaTime, 0, _remainingTime);

            if (_remainingTime <= 0)
                IsRecovering = false;
        }

        public void OnEnter()
        {
            IsRecovering = true;

            _remainingTime = _data.TimeForRecoveringAfterDamage;

            // TODO: move animator to "TAKE DAMAGE" state

            Debug.Log($"<b>TAKE DAMAGE</b> - <color=green>OnEnter</color> - remaining time: {_remainingTime}");
        }

        public void OnExit()
        {
            Debug.Log("<b>TAKE DAMAGE</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}