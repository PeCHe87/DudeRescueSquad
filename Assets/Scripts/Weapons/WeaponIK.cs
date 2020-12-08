using UnityEngine;

namespace DudeResqueSquad
{
    /// <summary>
    /// This class allows for a 3D character to grab its current weapon's handles, and look wherever it's aiming.
    /// There's a bit of setup involved. You need to have a CharacterHandleWeapon component on your character, it needs an animator with IKPass active (this is set in the Layers tab of the animator)
    /// the animator's avatar MUST be set as humanoid
    /// And you need to put that script on the same gameobject as the animator (otherwise it won't work). 
    /// Finally, you need to set left and right handles (or only one of these) on your weapon(s). 
    /// </summary>
    public class WeaponIK : MonoBehaviour
    {
        #region Inspector properties

        [Header("Bindings")]
        [Tooltip("The transform to use as a target for the left hand")]
        [SerializeField] private Transform LeftHandTarget = null;
        [Tooltip("The transform to use as a target for the right hand")]
        [SerializeField] private Transform RightHandTarget = null;

        [Header("Attachments")]
        [Tooltip("whether or not to attach the left hand to its target")]
        [SerializeField] private bool AttachLeftHand = true;
        [Tooltip("whether or not to attach the right hand to its target")]
        [SerializeField] private bool AttachRightHand = true;

        [SerializeField] private bool _canDebug = true;

        #endregion

        #region Private properties

        protected Animator _animator;

        #endregion

        #region Private methods

        protected virtual void Start()
        {
            _animator = GetComponent<Animator>();
        }

        /// <summary>
        /// During the animator's IK pass, tries to attach the avatar's hands to the weapon
        /// </summary>
        protected virtual void OnAnimatorIK(int layerIndex)
        {
            if (_animator == null)
            {
                return;
            }

            /*//if the IK is active, set the position and rotation directly to the goal. 
            if (AttachLeftHand)
            {
                if (LeftHandTarget != null)
                {
                    AttachHandToHandle(AvatarIKGoal.LeftHand, LeftHandTarget);

                    _animator.SetLookAtWeight(1);
                    _animator.SetLookAtPosition(LeftHandTarget.position);
                }
                else
                {
                    DetachHandFromHandle(AvatarIKGoal.LeftHand);
                }
            }

            if (AttachRightHand)
            {
                if (RightHandTarget != null)
                {
                    AttachHandToHandle(AvatarIKGoal.RightHand, RightHandTarget);
                }
                else
                {
                    DetachHandFromHandle(AvatarIKGoal.RightHand);
                }
            }*/

            // Set the look target position, if one has been assigned
            if (LeftHandTarget != null)
            {
                _animator.SetLookAtWeight(1, 1);
                _animator.SetLookAtPosition(LeftHandTarget.position);
            }

            // Set the right hand target position and rotation, if one has been assigned
            if (LeftHandTarget != null)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.LeftHand, 1);
                _animator.SetIKRotationWeight(AvatarIKGoal.LeftHand, 1);
                _animator.SetIKPosition(AvatarIKGoal.LeftHand, LeftHandTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.LeftHand, LeftHandTarget.rotation);

                if (_canDebug)
                    Debug.Log($"WeaponIK: {transform.name} LeftHand");
            }

            // Set the right hand target position and rotation, if one has been assigned
            if (RightHandTarget != null)
            {
                _animator.SetIKPositionWeight(AvatarIKGoal.RightHand, 1);
                _animator.SetIKRotationWeight(AvatarIKGoal.RightHand, 1);
                _animator.SetIKPosition(AvatarIKGoal.RightHand, RightHandTarget.position);
                _animator.SetIKRotation(AvatarIKGoal.RightHand, RightHandTarget.rotation);

                if (_canDebug)
                    Debug.Log($"WeaponIK: {transform.name} RightHand");
            }
        }

        /// <summary>
        /// Attaches the hands to the handles
        /// </summary>
        /// <param name="hand"></param>
        /// <param name="handle"></param>
        protected virtual void AttachHandToHandle(AvatarIKGoal hand, Transform handle)
        {
            _animator.SetIKPositionWeight(hand, 1);
            _animator.SetIKRotationWeight(hand, 1);
            _animator.SetIKPosition(hand, handle.position);
            _animator.SetIKRotation(hand, handle.rotation);
        }

        /// <summary>
        /// Detachs the hand from handle, if the IK is not active, set the position and rotation of the hand and head back to the original position
        /// </summary>
        /// <param name="hand">Hand.</param>
        protected virtual void DetachHandFromHandle(AvatarIKGoal hand)
        {
            _animator.SetIKPositionWeight(hand, 0);
            _animator.SetIKRotationWeight(hand, 0);
            _animator.SetLookAtWeight(0);
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Binds the character hands to the handles targets
        /// </summary>
        /// <param name="leftHand">Left hand.</param>
        /// <param name="rightHand">Right hand.</param>
        public virtual void SetHandles(Transform leftHand, Transform rightHand)
        {
            if (AttachLeftHand)
                LeftHandTarget = leftHand;

            if (AttachRightHand)
                RightHandTarget = rightHand;
        }

        #endregion
    }
}