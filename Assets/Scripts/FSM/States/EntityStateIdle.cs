using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityStateIdle : IState
    {
        #region Public properties

        public float RemainingWaitingTime { get => _remainingTime; }

        #endregion

        #region Private properties

        private Entity _entity = null;
        private float _remainingTime = 0;

        #endregion

        #region Constructor 

        public EntityStateIdle(Entity entity)
        {
            _entity = entity;
        }

        #endregion

        #region IState implementation 

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.IDLE;
        }

        public void Tick()
        {
            if (_remainingTime <= 0)
                return;

            // Timer idle should be decreased
            _remainingTime = Mathf.Clamp(_remainingTime - Time.deltaTime, 0, _remainingTime);

            //Debug.Log($"<b>IDLE</b> - <color=yellow>Tick</color> - Remaining time: {_remainingTime}");
        }

        public void OnEnter()
        {
            _remainingTime = Random.Range(_entity.Data.MinIdleTime, _entity.Data.MaxIdleTime);

            Debug.Log($"<b>IDLE</b> - <color=green>OnEnter</color> - RemainingTime: {_remainingTime}");
        }

        public void OnExit()
        {
            Debug.Log($"<b>IDLE</b> - <color=red>OnExit</color> - RemainingTime: {_remainingTime}");
        }

        #endregion
    }
}