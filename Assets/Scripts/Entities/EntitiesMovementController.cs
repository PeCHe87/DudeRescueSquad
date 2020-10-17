using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class EntitiesMovementController : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Transform _target = null;  // TODO: this should come from each Entity
        [SerializeField] private Entity[] _entities = null;
        [SerializeField] private EntityFollower _followerTemplate = null;
        [SerializeField] private float _timeToStartFollowing = 1;
        [SerializeField] private bool _canDebug = false;

        #endregion

        #region Private properties

        private bool _wasInitialized = false;
        private Transform[] _transforms = null;
        private NavMeshAgent[] _agents = null;
        private NavMeshObstacle[] _obstacles = null;

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

                // Check if entity has a detected target
                if (entity.Follower.Target == null)
                    continue;

                var agentTransform = _transforms[i];
                var obstacle = _obstacles[i];
                var agent = _agents[i];

                // Skip agents that aren't moving
                if (!agent.enabled)
                    continue;

                var diff = (entity.Follower.Target.position - agentTransform.position).magnitude;

                if (diff <= entity.Data.DistanceToStop)
                {
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                        agent.enabled = false;
                    }

                    obstacle.transform.position = agent.transform.position;
                    obstacle.enabled = true;
                    obstacle.carving = true;

                    if (_canDebug)
                        Debug.Log($"Stop agent {agent.transform.name}");
                }
            }
        }

        private void Init()
        {
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

            InvokeRepeating("Follow", 5, _timeToStartFollowing);

            _wasInitialized = true;
        }

        private async void Follow()
        {
            int amount = _agents.Length;

            for (int i = 0; i < amount; i++)
            {
                var entity = _entities[i];

                // Check if entity has a detected target
                if (entity.Follower.Target == null)
                    continue;

                var agentTransform = _transforms[i];

                var diff = (entity.Follower.Target.position - agentTransform.position).magnitude;

                if (diff <= entity.Data.DistanceToStop)
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

                if (diff <= entity.Data.DistanceToStop)
                    continue;

                var obstacle = _obstacles[i];
                var agent = _agents[i];

                if (!Application.isPlaying)
                    break;

                StartCoroutine(UpdateDestination(entity, agent, obstacle));
            }
        }

        private IEnumerator UpdateDestination(Entity entity, NavMeshAgent agent, NavMeshObstacle obstacle)
        {
            obstacle.carving = false;
            obstacle.enabled = false;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            // Check if current target exists
            if (entity.Follower.Target != null)
            {
                agent.enabled = true;

                if (agent.enabled)
                {
                    agent.SetDestination(entity.Follower.Target.position);
                    agent.isStopped = false;
                }
            }
        }

        #endregion
    }
}