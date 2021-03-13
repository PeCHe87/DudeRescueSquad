using System;
using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntitiesMovementController : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Entity[] _entities = null;
        [SerializeField] private EntityFollower _followerTemplate = null;
        [SerializeField] private float _timeToStartFollowing = 1;
        [SerializeField] private bool _canDebug = false;
        [SerializeField] private Transform _followerObstaclesOutside = null;

        #endregion

        #region Private properties

        private bool _wasInitialized = false;
        private Transform[] _transforms = null;
        private NavMeshAgent[] _agents = null;
        private NavMeshObstacle[] _obstacles = null;
        private Vector3 _positionOutsideWorld = Vector3.zero;

        #endregion

        #region Private methods

        private void Start()
        {
            Init();
        }

        private void Update()
        {
            if (!_wasInitialized)
                return;

            // For each agent, who is moving, check if it reached the target
            int amount = _agents.Length;

            for (int i = 0; i < amount; i++)
            {
                var entity = _entities[i];

                if (entity.State == Enums.EnemyStates.IDLE || entity.State == Enums.EnemyStates.TAKING_DAMAGE || entity.State == Enums.EnemyStates.DEAD)
                {
                    Stop(entity);

                    continue;
                }

                // Check if entity has a detected target
                if (entity.Follower.Target == null)
                    continue;

                var agentTransform = _transforms[i];
                

                // Skip agents that aren't moving
                if (!entity.Follower.Agent.enabled)
                    continue;

                var diff = (entity.Follower.Target.position - agentTransform.position).magnitude;

                if (diff <= DistanceToStop(entity))
                {
                    Stop(entity);

                    if (_canDebug)
                        Debug.Log($"Stop agent {entity.name}");
                }
            }
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
            _positionOutsideWorld = _followerObstaclesOutside.position;

            int amount = _entities.Length;

            _transforms = new Transform[amount];
            _agents = new NavMeshAgent[amount];
            _obstacles = new NavMeshObstacle[amount];

            for (int i = 0; i < amount; i++)
            {
                var entity = _entities[i];

                entity.InitMovement(_followerTemplate);

                // Add transform
                _transforms[i] = entity.transform;

                // Add Obstacle
                entity.Follower.SetObstacleEnabledState(false);

                _obstacles[i] = entity.Follower.Obstacle;

                // Add Agent
                entity.Follower.Agent.isStopped = true;
                entity.Follower.SetAgentEnabledState(false);

                _agents[i] = entity.Follower.Agent;
            }

            InvokeRepeating("Follow", 1, _timeToStartFollowing);

            _wasInitialized = true;

            Debug.Log("<b>EntitiesMovementController</b> - Initialization");
        }

        private async void Follow()
        {
            int amount = _agents.Length;

            for (int i = 0; i < amount; i++)
            {
                var entity = _entities[i];
                var entityState = entity.State;

                var targetExists = entity.Follower.Target != null;
                
                if (entityState == Enums.EnemyStates.IDLE || entityState == Enums.EnemyStates.TAKING_DAMAGE ||
                    entityState == Enums.EnemyStates.DEAD || entityState == Enums.EnemyStates.ATTACKING)
                {
                    // Rotates toward target if it exists
                    if (targetExists && (entityState == Enums.EnemyStates.IDLE || entityState == Enums.EnemyStates.ATTACKING))
                    {
                        entity.Follower.RotatesTowardTarget();
                    }
                    
                    continue;
                }

                // Check if entity has a detected target
                if (!targetExists)
                    continue;

                var agentTransform = _transforms[i];

                var diff = (entity.Follower.Target.position - agentTransform.position).magnitude;

                if (diff <= DistanceToStop(entity))
                    continue;

                var obstacle = _obstacles[i];
                obstacle.carving = false;
                obstacle.enabled = false;
            }

            await Task.Delay((int)(1000 * _timeToStartFollowing));

            for (int i = 0; i < amount; i++)
            {
                var entity = _entities[i];

                // Check if entity has a detected target
                if (entity.Follower.Target == null)
                    continue;

                var agentTransform = _transforms[i];

                var diff = (entity.Follower.Target.position - agentTransform.position).magnitude;

                if (diff <= DistanceToStop(entity))
                    continue;

                if (!Application.isPlaying)
                    break;

                StartCoroutine(UpdateDestination(entity));
            }
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

        private void Stop(Entity entity)
        {
            if (entity.Follower.Agent.enabled)
            {
                entity.Follower.Agent.isStopped = true;
                entity.Follower.SetAgentEnabledState(false);
            }

            // Don't enable obstacle when entity is dead
            if (entity.State != Enums.EnemyStates.DEAD)
            {
                entity.Follower.Obstacle.transform.position = entity.Follower.Agent.transform.position;
                entity.Follower.SetObstacleEnabledState(true);

                entity.Follower.Obstacle.size = entity.Follower.ObstacleSize;
            }
            else
            {
                entity.Follower.SetObstacleEnabledState(false);
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

        #endregion
    }
}