using System;
using UnityEngine.AI;
using UnityEngine;

namespace DudeResqueSquad
{
    public class NavAgentsManager : MonoBehaviour
    {
        [Serializable]
        public struct AgentData
        {
            public NavMeshAgent agent;
            public float speed;
            public float stopDistance;
        }

        #region Inspector

        [SerializeField] private AgentData[] _agents = null;
        [SerializeField] private Transform _goal = null;

        #endregion

        #region Private properties

        private int _agentsAmount = 0;

        #endregion

        #region Private methods

        private void Awake()
        {
            _agentsAmount = _agents.Length;

            for (int i = 0; i < _agentsAmount; i++)
            {
                var agentData = _agents[i];
                agentData.agent.speed = 0;
                agentData.agent.stoppingDistance = agentData.stopDistance;
            }
        }

        private void Update()
        {
            // Check distance to goal
            for (int i = 0; i < _agentsAmount; i++)
            {
                var agentData = _agents[i];

                if (agentData.agent.speed == 0)
                    continue;

                Vector3 dir = _goal.position - agentData.agent.transform.position;
                float distance = dir.magnitude;

                if (distance <= agentData.stopDistance)
                {
                    // Stop agent speed
                    agentData.agent.speed = 0;

                    // Rotate agent towards goal
                    agentData.agent.transform.rotation = Quaternion.LookRotation(dir.normalized);
                }
            }
        }

        #endregion

        #region Public methods

        public void FollowGoal()
        {
            for (int i = 0; i < _agentsAmount; i++)
            {
                var agentData = _agents[i];

                agentData.agent.SetDestination(_goal.position);
                agentData.agent.speed = agentData.speed;
            }
        }

        #endregion
    }
}