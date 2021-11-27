using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityStateAttacking : IState
    {
        #region Public properties

        public bool IsAttacking { get => _isAttacking; }

        #endregion

        #region Private properties

        private Entity _entity = null;
        private float _attackRangeDistance = 0;
        private bool _isAttacking = false;

        #endregion

        public EntityStateAttacking(Entity entity)
        {
            _entity = entity;
        }

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.ATTACKING;
        }

        public void Tick()
        {
            if (!_isAttacking)
                return;

            // If there isn't any detected target stop chasing
            if (_entity.Follower.Target == null)
            {
                _isAttacking = false;

                return;
            }

            // Check if distance to target is enough or it should continue chasing it
            var remainingDistance = (_entity.Follower.Target.position - _entity.Follower.Agent.transform.position).magnitude;

            if (remainingDistance <= _attackRangeDistance)
            {
                _isAttacking = false;

                return;
            }
        }

        public void OnEnter()
        {
            _isAttacking = true;

            _attackRangeDistance = _entity.Weapon.AttackAreaRadius;

            if (_entity.Follower.Target == null) return;

            Debug.Log($"<b>ATTACKING</b> - <color=green>OnEnter</color> - target: {_entity.Follower.Target.name}, attack range distance: {_attackRangeDistance}");
        }

        public void OnExit()
        {
            Debug.Log("<b>ATTACKING</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}