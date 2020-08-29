using UnityEngine;

namespace DudeResqueSquad
{
    public class RotateTowardsTarget : MonoBehaviour
    {
        [SerializeField] private Transform _transform = null;
        [SerializeField] private float _speed = 0;
		[SerializeField] private bool _smoothTurn = false;
		[SerializeField] private float _turnSmoothTime = 0.1f;

		private float _turnSmoothVelocity = 0;
		
        public void Rotate(Vector3 target)
        {
            if (target == Vector3.zero)
                return;
			
			if (_smoothTurn)
			{
				float targetAngle = Mathf.Atan2(target.x, target.z) * Mathf.Rad2Deg;
				float angle = Mathf.SmoothDampAngle(_transform.eulerAngles.y, targetAngle, ref _turnSmoothVelocity, _turnSmoothTime);
				_transform.rotation = Quaternion.Euler(0, targetAngle, 0);
			}
			else
			{
				var newDirection = Quaternion.LookRotation(target, Vector3.up);
				_transform.rotation = newDirection;
			}
        }
    }
}