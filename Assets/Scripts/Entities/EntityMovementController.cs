using System;
using System.Collections;
using System.Threading;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntityMovementController : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private EntityFollower _followerTemplate = null;
        [SerializeField] private float _timeToStartFollowing = 1;
        [SerializeField] private bool _canDebug = false;

        #endregion

        #region Private properties

        private Entity _entity = null;
        private bool _wasInitialized = false;
        private Transform _transform = null;
        private NavMeshAgent _agent = null;
        private NavMeshObstacle _obstacle = null;
        private CancellationToken _taskCancellationToken = default;
        private CancellationTokenSource _token = default;
        private float _originalY = default;

        #endregion

        #region Private methods

        private void Awake()
        {
            _entity = GetComponent<Entity>();

            _originalY = transform.position.y;
        }

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (!_wasInitialized)
                return;

            // Skip entity if it doing a knockback movement
            if (_entity.KnockBackInProgress)
            {
                ResetPositionDuringKnockBack();

                return;
            }

            // If idle or dead stop it
            if (_entity.State == Enums.EnemyStates.IDLE || _entity.State == Enums.EnemyStates.DEAD)
            {
                Stop();

                return;
            }

            // Check if entity has a detected target
            if (_entity.Follower.Target == null) return;

            // Skip agent that isn't moving
            if (!_entity.Follower.Agent.enabled) return;

            var diff = (_entity.Follower.Target.position - _transform.position).magnitude;

            if (diff <= DistanceToStop(_entity))
            {
                Stop();

                if (_canDebug)
                    Debug.Log($"Stop agent {_entity.name}");
            }
        }

        private void ResetPositionDuringKnockBack()
        {
            var position = _transform.position;
            position.y = _originalY;
            _entity.Follower.Agent.Warp(position);
        }

        private void OnDestroy()
        {
            _token.Cancel();
        }

        /// <summary>
        /// It determines the distance to stop when entity is following a target based on its current state
        /// </summary>
        /// <param name="entity"></param>
        /// <returns></returns>
        private float DistanceToStop(Entity entity)
        {
            if (entity.State == Enums.EnemyStates.PATROLLING)
            {
                return entity.Data.PatrollingDistanceToStop;
            }
            else
            {
                return entity.Weapon.AttackAreaRadius;
            }
        }

        private void Init()
        {
            _token = new CancellationTokenSource();
            _taskCancellationToken = _token.Token;

            _entity.InitMovement(_followerTemplate);

            // Add transform
            _transform = _entity.transform;

            // Add Obstacle
            _entity.Follower.SetObstacleEnabledState(false);

            _obstacle = _entity.Follower.Obstacle;

            // Add Agent
            _entity.Follower.Agent.isStopped = true;
            _entity.Follower.SetAgentEnabledState(false);

            _agent = _entity.Follower.Agent;

            InvokeRepeating("Follow", 1, _timeToStartFollowing);

            _wasInitialized = true;

            Debug.Log($"<b>EntityMovementController</b> - Initialization: {_entity.UID}");
        }

        private async void Follow()
        {
            var entityState = _entity.State;

            var targetExists = _entity.Follower.Target != null;

            float diff = 0;

            if (entityState == Enums.EnemyStates.IDLE || entityState == Enums.EnemyStates.DEAD || entityState == Enums.EnemyStates.ATTACKING)
            {
                // Rotates toward target if it exists
                if (targetExists && (entityState == Enums.EnemyStates.IDLE || entityState == Enums.EnemyStates.ATTACKING))
                {
                    _entity.Follower.RotatesTowardTarget();
                }
            }
            else
            {
                // Check if entity has a detected target
                if (targetExists)
                {

                    diff = (_entity.Follower.Target.position - _transform.position).magnitude;

                    if (diff > DistanceToStop(_entity))
                    {
                        _obstacle.carving = false;
                        _obstacle.enabled = false;
                    }
                }
            }

            await Task.Delay((int)(1000 * _timeToStartFollowing));

            if (_taskCancellationToken.IsCancellationRequested) return;

            // Check if entity has a detected target
            if (_entity.Follower.Target == null) return;

            diff = (_entity.Follower.Target.position - _transform.position).magnitude;

            if (diff <= DistanceToStop(_entity)) return;

            StartCoroutine(UpdateDestination(_entity));
        }

        /// <summary>
        /// If entity state is PATROLLING or CHASING then its Agent component should be enabled and its Obstacle component should be disabled
        /// </summary>
        /// <param name="entity"></param>
        private void CheckFollowerBehaviourBasedOnEntityState(Entity entity)
        {
            if (entity.State == Enums.EnemyStates.PATROLLING || entity.State == Enums.EnemyStates.CHASING)
            {
                if (!entity.Follower.Agent.enabled)
                {
                    entity.Follower.SetAgentEnabledState(true);
                    entity.Follower.SetObstacleEnabledState(false);
                }
            }
        }

        private IEnumerator UpdateDestination(Entity entity)
        {
            if (entity.State != Enums.EnemyStates.IDLE && entity.State != Enums.EnemyStates.TAKING_DAMAGE && entity.State != Enums.EnemyStates.DEAD)
            {
                // Skip this logic if there isn't any target
                if (entity.Follower.Target != null)
                {
                    entity.Follower.Obstacle.size = Vector3.zero;
 
                    entity.Follower.SetObstacleEnabledState(false);

                    yield return new WaitForEndOfFrame();
                    yield return new WaitForEndOfFrame();

                    // Check if current target exists to resume its movement
                    if (entity.Follower.Target != null)
                    {
                        Resume(entity);
                    }
                }
            }
        }

        private void Stop()
        {
            if (_entity.Follower.Agent.enabled)
            {
                _entity.Follower.Agent.isStopped = true;
                _entity.Follower.SetAgentEnabledState(false);
            }

            // Don't enable obstacle when entity is dead
            if (_entity.State != Enums.EnemyStates.DEAD)
            {
                _entity.Follower.Obstacle.transform.position = _entity.Follower.Agent.transform.position;
                _entity.Follower.SetObstacleEnabledState(true);

                _entity.Follower.Obstacle.size = _entity.Follower.ObstacleSize;
            }
            else
            {
                _entity.Follower.SetObstacleEnabledState(false);
            }
        }

        private void Resume(Entity entity)
        {
            // Enables agent
            entity.Follower.SetAgentEnabledState(true);

            if (entity.Follower.Agent.enabled)
            {
                entity.Follower.Agent.SetDestination(entity.Follower.Target.position);
                entity.Follower.Agent.isStopped = false;

                ResumeSpeedBasedOnState(entity);
            }
        }

        /// <summary>
        /// Resumes the agent speed based on entity's state
        /// </summary>
        /// <param name="entity"></param>
        private void ResumeSpeedBasedOnState(Entity entity)
        {
            if (entity.State == Enums.EnemyStates.CHASING)
                entity.Follower.Agent.speed = entity.Data.SpeedChasingMovement;
            else if (entity.State == Enums.EnemyStates.PATROLLING)
                entity.Follower.Agent.speed = entity.Data.SpeedPatrollingMovement;
        }

        private void Disable(Entity entity)
        {
            entity.Follower.Agent.enabled = false;
            entity.Follower.SetAgentEnabledState(false);

            entity.Follower.Obstacle.enabled = false;
        }

        #endregion

        #region  public methods

        public void StopMovementDuringKnockback()
        {
            Stop();

            ResetPositionDuringKnockBack();

            _entity.Follower.SetTarget(null);
            _entity.Follower.StopTakingDamage();
        }

        public void ResumeMovementAfterKnockback()
        {
            ResetPositionDuringKnockBack();

            _entity.Follower.ResumeAfterTakingDamage();
        }

        #endregion
    }
}