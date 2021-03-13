using System.ComponentModel;
using UnityEngine;

namespace DudeResqueSquad
{
    [RequireComponent(typeof(Entity))]
    public class IEntityAttack : MonoBehaviour
    {
        #region Inspector properties
        
        [SerializeField] protected Transform _originPojectile = null;
        [SerializeField] protected bool _canDebugReloading = false;
        [SerializeField] protected LayerMask _targetLayerMask;

        #endregion
        
        #region Protected properties

        protected Entity _entity = null;
        protected bool _attackStarted = false;
        protected ItemWeaponData _weapon = null;

        #endregion

        #region Unity methods

        protected void Awake()
        {
            _entity = GetComponent<Entity>();
            _entity.OnInitialized += Initialized;
        }

        protected void OnDestroy()
        {
            _entity.OnInitialized -= Initialized;

            _entity.StateMachine.PropertyChanged -= StateMachineHasChanged;
        }

        #endregion

        #region Private methods

        #endregion

        #region Overridable methods

        protected virtual void Initialized()
        {
            _weapon = _entity.Weapon;
            
            _entity.StateMachine.PropertyChanged += StateMachineHasChanged;
        }

        protected virtual void Attack(){}

        protected virtual void StateMachineHasChanged(object sender, PropertyChangedEventArgs e){}

        /// <summary>
        /// Checks if entity is near enough to its target to attack it
        /// </summary>
        /// <returns></returns>
        protected virtual bool IsOnAttackingRange()
        {
            if (_entity.Follower == null)
                return false;

            var target = _entity.FieldOfView.NearestTarget;

            if (target == null)
                return false;

            var distanceToTarget = (target.position - _entity.Follower.Agent.transform.position).magnitude;

            //return (distanceToTarget <= _entity.Data.RadiusAttack);
            return (distanceToTarget <= _weapon.AttackAreaRadius);
        }

        #endregion
    }
}