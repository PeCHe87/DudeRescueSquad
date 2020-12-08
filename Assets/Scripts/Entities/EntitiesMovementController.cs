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

                if (diff <= entity.DistanceToStop)
                {
                    Stop(entity);

                    if (_canDebug)
                        Debug.Log($"Stop agent {entity.name}");
                }
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
                entity.Follower.Obstacle.carving = false;
                entity.Follower.Obstacle.enabled = false;

                _obstacles[i] = entity.Follower.Obstacle;

                // Add Agent
                entity.Follower.Agent.isStopped = true;
                entity.Follower.Agent.enabled = false;

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

                if (entity.State == Enums.EnemyStates.IDLE || entity.State == Enums.EnemyStates.TAKING_DAMAGE || entity.State == Enums.EnemyStates.DEAD)
                    continue;

                // Check if entity has a detected target
                if (entity.Follower.Target == null)
                    continue;

                var agentTransform = _transforms[i];

                var diff = (entity.Follower.Target.position - agentTransform.position).magnitude;

                if (diff <= entity.DistanceToStop)
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

                if (diff <= entity.DistanceToStop)
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
                    entity.Follower.Agent.enabled = true;
                    entity.Follower.Obstacle.enabled = false;
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
                    entity.Follower.Obstacle.carving = false;
                    entity.Follower.Obstacle.enabled = false;

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
                entity.Follower.Agent.enabled = false;
            }

            // Don't enable obstacle when entity is dead
            if (entity.State != Enums.EnemyStates.DEAD)
            {
                entity.Follower.Obstacle.transform.position = entity.Follower.Agent.transform.position;
                entity.Follower.Obstacle.enabled = true;
                entity.Follower.Obstacle.carving = true;

                entity.Follower.Obstacle.size = entity.Follower.ObstacleSize;
            }
            else
            {
                entity.Follower.Obstacle.carving = false;
                entity.Follower.Obstacle.enabled = false;
            }
        }

        private void Resume(Entity entity)
        {
            // Enables agent
            entity.Follower.Agent.enabled = true;

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