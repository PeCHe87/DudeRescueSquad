using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityStateChasing : IState
    {
        #region Public properties

        public bool IsChasing { get => _isChasing; }

        #endregion

        #region Private properties

        private Entity _entity = null;
        private float _distanceToStop = 0;
        private bool _isChasing = false;

        #endregion

        public EntityStateChasing(Entity entity)
        {
            _entity = entity;
        }

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.CHASING;
        }

        public void Tick()
        {
            if (!_isChasing)
                return;

            // If there isn't any detected target stop chasing
            if (_entity.Follower.Target == null)
            {
                _isChasing = false;

                return;
            }

            // Check if distance to target is enough or it should continue chasing it
            var remainingDistance = (_entity.Follower.Target.position - _entity.Follower.Agent.transform.position).magnitude;

            if (remainingDistance <= _distanceToStop)
            {
                _isChasing = false;

                return;
            }
        }

        public void OnEnter()
        {
            _isChasing = true;

            _distanceToStop = _entity.Weapon.AttackAreaRadius;  //_entity.Data.ChasingDistanceToStop;

            Debug.Log($"<b>CHASING</b> - <color=green>OnEnter</color> - target: {_entity.Follower.Target.name}");
        }

        public void OnExit()
        {
            Debug.Log("<b>CHASING</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}