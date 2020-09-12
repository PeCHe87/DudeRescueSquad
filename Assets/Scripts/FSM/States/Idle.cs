using UnityEngine;

namespace DudeResqueSquad
{
    public class Idle : IState
    {
        #region Public properties

        public float RemainingWaitingTime { get => _remainingTime; }

        #endregion

        #region Private properties

        private float _minTime = 0;
        private float _maxTime = 0;
        private float _remainingTime = 0;

        #endregion

        #region Constructor 

        public Idle(EnemyData data)
        {
            _minTime = data.MinIdleTime;
            _maxTime = data.MaxIdleTime;
        }

        #endregion

        #region IState implementation 

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
            _remainingTime = Random.Range(_minTime, _maxTime);

            // TODO: move animator to "IDLE" state

            Debug.Log($"<b>IDLE</b> - <color=green>OnEnter</color> - RemainingTime: {_remainingTime}");
        }

        public void OnExit()
        {
            Debug.Log($"<b>IDLE</b> - <color=red>OnExit</color> - RemainingTime: {_remainingTime}");
        }

        #endregion
    }
}