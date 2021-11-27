using System;
using UnityEngine;

namespace DudeResqueSquad
{
    /// <summary>
    /// Class in charge of the logic related with the push back movement applied when an entity takes damage
    /// </summary>
    public class EntityKnockback : MonoBehaviour
    {
        #region Inspector properties

        [SerializeField] private float _debugForce = 5;
        [SerializeField] private ForceMode _pushBackForceMode = ForceMode.Force;
        [SerializeField] private BoxCollider _collider = default;

        #endregion

        #region Private properties

        private Entity _entity = default;
        private EntityMovementController _movementController = default;
        private IDamageable _damageable = default;
        private Rigidbody _rigidBody = default;
        private bool _isPushingBack = default;

        #endregion

        #region Private methods

        private void TakingDamage(object sender, CustomEventArgs.DamageEventArgs e)
        {
            if (_damageable.IsDead) return;

            if (_isPushingBack) return;

            if (!e.attackCanPushBack) return;

            // Check if push back should be applied
            if (!_entity.Data.canBePushBackOnTakingDamage) return;

            _isPushingBack = true;

            Debug.Log($"<color=magenta>EntityKnockBack</color> - {e.attackCanPushBack}, attack direction: {e.attackDirection}");

            // Communicates to the entity that the push back has started and should cancel its logic until the knockback finish
            _entity.StartKnockBack();

            _movementController.StopMovementDuringKnockback();

            ApplyPushBackBasedOnDamage(e.damage, e.attackDirection);
        }

        private void ApplyPushBackBasedOnDamage(float damage, Vector3 direction)
        {
            // Activate collider to detect collisions
            _collider.enabled = true;

            var pushBackForce = _debugForce;    // TODO: calculate the force based on damage and min and max possible force

            _rigidBody.velocity = Vector3.zero;
            _rigidBody.angularVelocity = Vector3.zero;
            _rigidBody.isKinematic = false;

            _rigidBody.AddForce(pushBackForce * direction, _pushBackForceMode);

            Invoke(nameof(StopKnockBack), _entity.Data.DelayToResumeAfterKnockBack);
        }

        private void StopKnockBack()
        {
            // Deactivate collider to detect collisions
            _collider.enabled = false;

            _movementController.ResumeMovementAfterKnockback();

            // Communicates to the entity that knockback has finished
            _entity.StopKnockBack();

            _isPushingBack = false;
        }

        #endregion

        #region Public methods

        public void Init(Entity entity, IDamageable damageable, Rigidbody rb)
        {
            _entity = entity;

            _movementController = _entity.GetComponent<EntityMovementController>();

            _damageable = damageable;
            _rigidBody = rb;

            _damageable.OnTakeDamage += TakingDamage;
        }

        public void Teardown()
        {
            _damageable.OnTakeDamage -= TakingDamage;
        }

        #endregion
    }
}