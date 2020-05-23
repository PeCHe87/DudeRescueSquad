using UnityEngine;

namespace DudeResqueSquad
{
    public class RotateTowardsTarget : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private float _speed = 0;

        public void Rotate(Vector3 target)
        {
            if (target == Vector3.zero)
                return;

            var newDirection = Quaternion.LookRotation(target, Vector3.up);
            _transform.rotation = newDirection;
        }
    }
}