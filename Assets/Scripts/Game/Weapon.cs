using UnityEngine;

namespace DudeResqueSquad
{
    public class Weapon : MonoBehaviour
    {
        #region Inspector properties

        [Header("IK")]
        [Tooltip("the transform to which the character's left hand should be attached to")]
        [SerializeField] private Transform _leftHandHandle = null;
        [Tooltip("the transform to which the character's right hand should be attached to")]
        [SerializeField] private Transform _rightHandHandle = null;

        #endregion

        #region Public properties

        public Transform LeftHandHandle { get => _leftHandHandle; }
        public Transform RightHandHandle { get => _rightHandHandle; }

        #endregion

        #region Public methods

        public void Attack()
        {
        }

        #endregion
    }
}