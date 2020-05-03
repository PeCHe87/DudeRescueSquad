using UnityEngine;

namespace DudeResqueSquad
{
    public class RotateTowardsTarget : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private float _speed = 0;

        //private Vector3 _target = Vector3.zero;
        //private bool _isRotating = false;

        /*private void Update()
        {
            if (!_isRotating)
                return;

            _transform.rotation = Quaternion.LookRotation(_target, Vector3.up);
        }*/

        /*private void RotateTowardsTarget()
        {
            Transform characterTransform = _characterBase.Model.transform;

            // Determine which direction to rotate towards
            Vector3 targetDirection = _target.position - characterTransform.position;

            // The step size is equal to speed times frame time.
            float singleStep = _rotationSpeed * Time.deltaTime;

            // Rotate the forward vector towards the target direction by one step
            Vector3 newDirection = Vector3.RotateTowards(characterTransform.forward, targetDirection, singleStep, 0.0f);

            // Draw a ray pointing at our target in
            Debug.DrawRay(characterTransform.position, newDirection, Color.blue);

            // Calculate a rotation a step closer to the target and applies rotation to this object
            characterTransform.rotation = Quaternion.LookRotation(newDirection);
        }*/

        public void Rotate(Vector3 target)
        {
            //_transform.rotation = Quaternion.LookRotation(target, Vector3.up);

            var newDirection = Quaternion.LookRotation(target, Vector3.up);
            _transform.rotation = newDirection; //Quaternion.Slerp(_transform.rotation, newDirection, Time.deltaTime * _speed );
        }
    }
}