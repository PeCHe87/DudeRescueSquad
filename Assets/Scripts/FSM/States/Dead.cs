using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class Dead : IState
    {
        #region Private properties

        private EnemyData _data = null;
        private NavMeshAgent _navMeshAgent = null;
        private NavMeshObstacle _navMeshObstacle = null;

        #endregion

        public Dead(EnemyData data, Animator animator, NavMeshAgent agent, NavMeshObstacle obstacle)
        {
            _data = data;

            _navMeshAgent = agent;

            _navMeshObstacle = obstacle;
        }

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.DEAD;
        }

        public void Tick()
        {
        }

        public void OnEnter()
        {
            // TODO: move animator to "DEAD" state

            // Cancel agent
            _navMeshAgent.enabled = false;

            // cancel obstacle
            _navMeshObstacle.enabled = false;

            Debug.Log($"<b>DEAD</b> - <color=green>OnEnter</color>");
        }

        public void OnExit()
        {
            Debug.Log("<b>DEAD</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}