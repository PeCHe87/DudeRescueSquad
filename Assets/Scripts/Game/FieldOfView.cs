using System.Collections.Generic;
using UnityEngine;

namespace DudeResqueSquad
{
    public class FieldOfView : MonoBehaviour
    {
        #region Events

        public System.Action<Transform> OnDetectNewTarget;
        public System.Action OnStopDetecting;

        #endregion

        #region Inspector properties

        [SerializeField] private float _viewRadius = 0;
        [Range(0, 360)]
        [SerializeField] private float _viewAngle = 0;
        [SerializeField] private LayerMask _targetMask = 0;
        [SerializeField] private LayerMask _obstacleMask = 0;
        [SerializeField] private float _delayBetweenDetections = 0;
        [SerializeField] private Transform _nearestTarget = null;
        [SerializeField] private bool _globalRotation = false;

        #endregion

        #region Private properties

        private List<Transform> _visibleTargets = null;
        private float _timeToDetect = 0;

        #endregion

        #region Public properties

        public float Radius { get => _viewRadius; set => _viewRadius = value; }
        public float ViewAngle { get => _viewAngle; set => _viewAngle = value; }

        public List<Transform> VisibleTargets { get => _visibleTargets; }

        public Transform NearestTarget { get => _nearestTarget; }

        #endregion

        #region Private methods

        private void Start()
        {
            _visibleTargets = new List<Transform>(50);
        }

        private void Update()
        {
            if (_timeToDetect > 0)
            {
                _timeToDetect -= Time.deltaTime;
                return;
            }

            FindVisibleTargets();

            GetNearestTarget();

            _timeToDetect = _delayBetweenDetections;
        }

        private void GetNearestTarget()
        {
            float currentDist = float.MaxValue;
            Vector3 position = transform.position;
            Transform currentTarget = null;

            int amountOfTargets = _visibleTargets.Count;
            for (int i = 0; i < amountOfTargets; i++)
            {
                var possibleTarget = _visibleTargets[i];

                // Check if target is alive before considering to detection
                IDamageable damageable = possibleTarget.GetComponent<IDamageable>();

                // Skip non damageable objects or dead ones
                if (damageable == null || damageable.IsDead)
                    continue;

                float dist = (possibleTarget.position - position).magnitude;

                if (dist < currentDist)
                {
                    currentTarget = possibleTarget;
                    currentDist = dist;
                }
            }

            // Notifies that there is a new target and it is different to the current detected
            if (currentTarget != null)
            {
                if (OnDetectNewTarget != null)
                    OnDetectNewTarget(currentTarget);
            }
            else if (_nearestTarget != null && currentTarget == null)
            {
                if (OnStopDetecting != null)
                    OnStopDetecting();
            }

            // Updates the nearest target
            _nearestTarget = currentTarget;
        }

        #endregion

        #region Public methods

        public Vector3 DirectionFromAngle(float degreeAngle)
        {
            if (_globalRotation)
                degreeAngle += transform.eulerAngles.y;

            float radianAngle = degreeAngle * Mathf.Deg2Rad;

            float x = Mathf.Sin(radianAngle);
            float z = Mathf.Cos(radianAngle);

            return new Vector3(x, 0, z);
        }

        public void FindVisibleTargets()
        {
            Vector3 position = transform.position;

            _visibleTargets.Clear();

            Collider[] targetsInViewRadius = Physics.OverlapSphere(position, _viewRadius, _targetMask);

            int targetsAmount = targetsInViewRadius.Length;

            for (int i = 0; i < targetsAmount; i++)
            {
                Transform target = targetsInViewRadius[i].transform;

                IDamageable damageable = target.GetComponent<IDamageable>();

                // Skip non damageable objects or dead ones
                if (damageable == null || damageable.IsDead)
                    continue;
                else
                {
                    Vector3 dirToTarget = target.position - position;
                    float distToTarget = dirToTarget.magnitude;

                    dirToTarget = dirToTarget.normalized;

                    float angle = Vector3.Angle(transform.forward, dirToTarget);

                    // If this target is between the both lines of cone view
                    if (angle < _viewAngle / 2)
                    {
                        bool isVisible = !Physics.Raycast(position, dirToTarget, distToTarget, _obstacleMask);

                        if (isVisible)
                            _visibleTargets.Add(target);
                    }
                }
            }
        }

        #endregion
    }
}