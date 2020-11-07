using UnityEngine;

namespace DudeResqueSquad
{
    public class EntityStateDead : IState
    {
        // Constructor
        public EntityStateDead(Entity entity)
        {
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
            Debug.Log($"<b>DEAD</b> - <color=green>OnEnter</color>");
        }

        public void OnExit()
        {
            Debug.Log("<b>DEAD</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}