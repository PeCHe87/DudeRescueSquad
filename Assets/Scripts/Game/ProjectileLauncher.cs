using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

namespace DudeResqueSquad
{
    /// <summary>
    /// Class used to predict a projectile before launching it, given an initial position and final target position
    /// </summary>
    [ExecuteInEditMode]
    public class ProjectileLauncher : MonoBehaviour
    {
        #region Structs

        struct LaunchData
        {
            public readonly Vector3 initialVelocity;
            public readonly float timeToTarget;

            public LaunchData(Vector3 initialVelocity, float timeToTarget)
            {
                this.initialVelocity = initialVelocity;
                this.timeToTarget = timeToTarget;
            }

        }

        [System.Serializable]
        struct ProjectileData
        {
            public string uid;
            public Vector3 initialPosition;
            public Vector3 finalPosition;
            public Transform transform;
            public float currentTime;
            public float gravity;
            public Vector3 initialVelocity;
            public float timeToTarget;
            public bool wasCompleted;
        }

        #endregion

        #region Inspector properties

        [SerializeField] private Transform _initialPivot = null;
        [SerializeField] private GameObject _projectileTemplate = null;
        [SerializeField] private Transform _target = null;
        [SerializeField] private float _maxHeight = 25;
        [SerializeField] private float _gravity = -18;
        [SerializeField] private bool _debugPath;

        #endregion

        #region Private properties

        private List<ProjectileData> _projectiles = null;
        private int _projectileCounter = 0;

        #endregion

        #region Private methods

        private void Start()
        {
            //Physics.gravity = Vector3.up * _gravity;

            _projectiles = new List<ProjectileData>();
        }

        private void Update()
        {
            if (_debugPath)
                DrawPath();

            // If there isn't any projectile skip the rest of logic
            if (_projectiles == null)
                return;

            // Get the current position of each projectile based on its current time and launch configuration
            int amount = _projectiles.Count;

            for (int i = 0; i < amount; i++)
            {
                var projectile = _projectiles[i];

                if (projectile.wasCompleted)
                    continue;

                if (projectile.currentTime >= projectile.timeToTarget)
                {
                    projectile.wasCompleted = true;

                    _projectiles[i] = projectile;

                    RemoveProjectile(projectile, 1);

                    continue;
                }

                float simulationTime = projectile.currentTime;
                Vector3 displacement = projectile.initialVelocity * simulationTime + Vector3.up * projectile.gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = projectile.initialPosition + displacement;

                projectile.transform.position = drawPoint;

                projectile.currentTime += Time.deltaTime;

                _projectiles[i] = projectile;
            }
        }

        /// <summary>
        /// Based on initial position, final position, gravity and max height it calculates the initial velocity before launching a new projectile
        /// </summary>
        /// <returns></returns>
        private LaunchData CalculateLaunchData()
        {
            Vector3 initialPosition = _initialPivot.position;
            Vector3 targetPosition = _target.position;

            float displacementY = targetPosition.y - initialPosition.y;
            Vector3 displacementXZ = new Vector3(targetPosition.x - initialPosition.x, 0, targetPosition.z - initialPosition.z);

            float time = Mathf.Sqrt(-2 * _maxHeight / _gravity) + Mathf.Sqrt(2 * (displacementY - _maxHeight) / _gravity);

            Vector3 velocityY = Vector3.up * Mathf.Sqrt(-2 * _gravity * _maxHeight);
            Vector3 velocityXZ = displacementXZ / time;

            return new LaunchData(velocityXZ + velocityY * -Mathf.Sign(_gravity), time);
        }

        /// <summary>
        /// Draw a path for the current configuration properties
        /// </summary>
        private void DrawPath()
        {
            LaunchData launchData = CalculateLaunchData();
            Vector3 previousDrawPoint = _initialPivot.position;
            Vector3 initialPosition = _initialPivot.position;

            // Quality of path definition
            int resolution = 30;

            // Create each steps to make a path based on resolution
            for (int i = 1; i <= resolution; i++)
            {
                float simulationTime = i / (float)resolution * launchData.timeToTarget;
                Vector3 displacement = launchData.initialVelocity * simulationTime + Vector3.up * _gravity * simulationTime * simulationTime / 2f;
                Vector3 drawPoint = initialPosition + displacement;
                Debug.DrawLine(previousDrawPoint, drawPoint, Color.green);
                previousDrawPoint = drawPoint;
            }
        }

        private async void RemoveProjectile(ProjectileData projectile, float timeToDestroy)
        {
            await Task.Delay(Mathf.CeilToInt(1000 * timeToDestroy));

            int amount = _projectiles.Count;

            for (int i = 0; i < amount; i++)
            {
                var projectileData = _projectiles[i];

                if (projectileData.uid.Equals(projectile.uid))
                {
                    _projectiles.RemoveAt(i);
                    break;
                }
            }

            Destroy(projectile.transform.gameObject);
        }

        #endregion

        #region Public methods

        [ContextMenu("Test Fire projectile")]
        public void Fire()
        {
            var projectile = Instantiate(_projectileTemplate);
            projectile.transform.position = _initialPivot.position;

            var data = CalculateLaunchData();
            var projectileData = new ProjectileData();

            projectileData.uid = _projectileCounter.ToString();
            projectileData.transform = projectile.transform;
            projectileData.initialVelocity = data.initialVelocity;
            projectileData.timeToTarget = data.timeToTarget;
            projectileData.initialPosition = _initialPivot.position;
            projectileData.currentTime = 0;
            projectileData.gravity = _gravity;
            projectileData.wasCompleted = false;

            _projectiles.Add(projectileData);

            _projectileCounter++;

            Debug.Log($"<b>Launch</b>: <color=green>'{projectileData.uid}'</color> time to target: {projectileData.timeToTarget}, gravity: {projectileData.gravity}");
        }

        #endregion
    }
}