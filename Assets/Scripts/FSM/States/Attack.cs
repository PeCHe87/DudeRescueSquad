using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class Attack : IState
    {
        #region Public properties

        public bool IsOnRange { get; private set; }

        #endregion

        #region Private properties

        private EnemyData _data = null;
        private float _remainingTime = 0;
        private Transform _transform = null;
        private FieldOfView _fov = null;
        private NavMeshAgent _navMeshAgent = null;
        private NavMeshObstacle _navMeshObstacle = null;

        #endregion

        // Constructor
        public Attack(EnemyData data, Transform transform, FieldOfView fov, Animator animator, NavMeshAgent agent, NavMeshObstacle obstacle)
        {
            _data = data;
            _transform = transform;
            _fov = fov;

            _navMeshAgent = agent;
            _navMeshObstacle = obstacle;
        }

        #region Private methods

        private void ProcessAttack()
        {
            Debug.Log($"<color=yellow><b>Attack</b></color>");

            // TODO: process attack based on weapon equipped

            _remainingTime = _data.DelayBetweenAttacks;
        }

        #endregion

        #region IState implementation

        public Enums.EnemyStates State()
        {
            return Enums.EnemyStates.ATTACKING;
        }

        public void Tick()
        {
            // If there is no target then skip logic
            if (_fov.NearestTarget == null)
                return;

            // Check distance to detect if it continues in the attacking range
            float distanceSquared = (_fov.NearestTarget.position - _transform.position).sqrMagnitude;

            if (distanceSquared <= _data.RadiusAttack)
            {
                IsOnRange = false;
                return;
            }

            // Time between attacks is decreased
            _remainingTime = Mathf.Clamp(_remainingTime - Time.deltaTime, 0, _remainingTime);

            if (_remainingTime <= 0)
                ProcessAttack();
        }

        public void OnEnter()
        {
            ProcessAttack();

            // Deactivate agent
            _navMeshAgent.enabled = false;

            // Activate obstacle
            _navMeshObstacle.enabled = true;

            Debug.Log($"<b>ATTACKING</b> - <color=green>OnEnter</color> - RemainingTimeNextAttack: {_remainingTime}");
        }

        public void OnExit()
        {
            Debug.Log($"<b>ATTACKING</b> - <color=red>OnExit</color>");
        }

        #endregion
    }
}