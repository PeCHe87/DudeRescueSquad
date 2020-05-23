using UnityEngine;

namespace DudeResqueSquad
{
    public class PathFollower : MonoBehaviour
    {
        [SerializeField] private float _timeToReachWaypoint = 0;
        [SerializeField] private Transform[] _waypoints = null;

        private int _index = 0;
        private float _elapsed = 0;
        private Transform _target = null;
        private Vector3 _startPosition = Vector3.zero;
        private Vector3 _targetPosition = Vector3.zero;

        private void Start()
        {
            _startPosition = transform.position;
            _target = _waypoints[_index];
            _targetPosition = _target.position;
        }

        private void Update()
        {
            if (_target == null)
                return;

            _elapsed += Time.deltaTime;

            float progress = _elapsed / _timeToReachWaypoint;

            transform.position = Vector3.Lerp(_startPosition, _targetPosition, progress);

            if (progress >= 1)
                NextWaypoint();
        }

        private void NextWaypoint()
        {
            _startPosition = transform.position;
            _elapsed = 0;
            _index = (_index + 1) % _waypoints.Length;
            _target = _waypoints[_index];
            _targetPosition = _target.position;
        }
    }
}