using UnityEngine;

namespace DudeResqueSquad
{
    public class TwoHandsWeaponFollower : MonoBehaviour
    {
        [SerializeField] private bool _isEnabled = false;
        [SerializeField] private Transform _leftHandPivot = null;

        private Transform _transform = null;

        private void Start()
        {
            _transform = transform;
        }

        private void Update()
        {
            if (!_isEnabled)
                return;

            var dir =  _leftHandPivot.position - _transform.position;

            var rotation = Quaternion.LookRotation(dir);

            _transform.rotation = rotation;
        }

        internal void Init(Transform leftHandPivot)
        {
            _leftHandPivot = leftHandPivot;

            _isEnabled = true;
        }
    }
}