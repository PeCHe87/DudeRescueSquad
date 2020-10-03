using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class Idle : IState
    {
        #region Public properties

        public float RemainingWaitingTime { get => _remainingTime; }

        #endregion

        #region Private properties

        private EnemyData _data = null;
        private float _remainingTime = 0;
        private NavMeshAgent _navMeshAgent = null;
        private NavMeshObstacle _navMeshObstacle = null;

        #endregion

        #region Constructor 

        public Idle(EnemyData data, NavMeshAgent agent, NavMeshObstacle obstacle)
        {
            _data = data;
            _navMeshAgent = agent;
            _navMeshObstacle = obstacle;
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
            _remainingTime = Random.Range(_data.MinIdleTime, _data.MaxIdleTime);

            // TODO: move animator to "IDLE" state

            // Cancel agent
            _navMeshAgent.enabled = false;

            // Activate obstacle
            _navMeshObstacle.enabled = true;

            Debug.Log($"<b>IDLE</b> - <color=green>OnEnter</color> - RemainingTime: {_remainingTime}");
        }

        public void OnExit()
        {
            Debug.Log($"<b>IDLE</b> - <color=red>OnExit</color> - RemainingTime: {_remainingTime}");
        }

        #endregion
    }
}