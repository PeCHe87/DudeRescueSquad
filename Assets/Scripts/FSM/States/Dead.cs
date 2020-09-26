using UnityEngine;

namespace DudeResqueSquad
{
    public class Dead : IState
    {
        #region Private properties

        private EnemyData _data = null;

        #endregion

        public Dead(EnemyData data, Animator animator)
        {
            _data = data;
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

            Debug.Log($"<b>DEAD</b> - <color=green>OnEnter</color>");
        }

        public void OnExit()
        {
            Debug.Log("<b>DEAD</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}