using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.AI;

namespace DudeResqueSquad
{
    public class TestLocalAvoidance : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private Transform _target = null;
        [SerializeField] private NavMeshAgent[] _agents = null;
        [SerializeField] private float _distanceToStop = 2;
        [SerializeField] private NavMeshObstacle[] _obstacles = null;
        [SerializeField] private float _timeToStartFollowing = 1;

        #endregion

        #region private methods

        private Transform[] _transforms = null;

        #endregion

        #region Private methods

        private void Start()
        {
            int amount = _agents.Length;

            _transforms = new Transform[amount];

            _obstacles = new NavMeshObstacle[amount];

            for (int i = 0; i < amount; i++)
            {
                var agent = _agents[i];

                agent.isStopped = true;

                var obstacle = agent.GetComponent<NavMeshObstacle>();   //_obstacles[i];

                obstacle.carving = false;
                obstacle.enabled = false;

                _obstacles[i] = obstacle;

                var agentTransform = agent.transform;

                _transforms[i] = agentTransform;
            }

            InvokeRepeating("Follow", 5, _timeToStartFollowing);
        }

        private async void Follow()
        {
            int amount = _agents.Length;

            for (int i = 0; i < amount; i++)
            {
                var agentTransform = _transforms[i];

                var diff = (_target.position - agentTransform.position).magnitude;

                if (diff <= _distanceToStop)
                    continue;

                var obstacle = _obstacles[i];
                obstacle.carving = false;
                obstacle.enabled = false;
            }

            await Task.Delay((int)(1000 * _timeToStartFollowing));

            for (int i = 0; i < amount; i++)
            {
                var agentTransform = _transforms[i];

                var diff = (_target.position - agentTransform.position).magnitude;

                if (diff <= _distanceToStop)
                    continue;

                var obstacle = _obstacles[i];
                var agent = _agents[i];

                if (!Application.isPlaying)
                    break;

                StartCoroutine(UpdateDestination(agent, obstacle));
            }
        }

        private void Update()
        {
            // For each agent, who is moving, check if it reached the target
            int amount = _agents.Length;

            for (int i = 0; i < amount; i++)
            {
                var agentTransform = _transforms[i];
                var obstacle = _obstacles[i];
                var agent = _agents[i];

                // Skip agents that aren't moving
                if (!agent.enabled)
                    continue;

                var diff = (_target.position - agentTransform.position).magnitude;

                if (diff <= _distanceToStop)
                {
                    if (agent.enabled)
                    {
                        agent.isStopped = true;
                        agent.enabled = false;
                    }

                    obstacle.transform.position = agent.transform.position;
                    obstacle.enabled = true;
                    obstacle.carving = true;

                    Debug.Log($"Stop agent {agent.transform.name}");
                }
            }
        }

        private IEnumerator UpdateDestination(NavMeshAgent agent, NavMeshObstacle obstacle)
        {
            obstacle.carving = false;
            obstacle.enabled = false;

            yield return new WaitForEndOfFrame();
            yield return new WaitForEndOfFrame();

            agent.enabled = true;

            if (agent.enabled)
            {
                agent.SetDestination(_target.position);
                agent.isStopped = false;
            }
        }

        #endregion

        #region Public methods

        [ContextMenu("Follow Target")]
        public async void StartMovement()
        {
            int amount = _agents.Length;

            for (int i = 0; i < amount; i++)
            {
                var obstacle = _obstacles[i];
                obstacle.carving = false;
                obstacle.enabled = false;
            }

            await Task.Delay((int)(1000 * _timeToStartFollowing));

            for (int i = 0; i < amount; i++)
            {
                var obstacle = _obstacles[i];
                var agent = _agents[i];

                StartCoroutine(UpdateDestination(agent, obstacle));
            }
        }

        #endregion
    }
}