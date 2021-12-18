using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    [CreateAssetMenu(fileName = "AssaultWeaponData", menuName = "Data/Weapons/Assault weapon")]
    public abstract class BaseWeaponDefinition : ScriptableObject, IWeaponDefinition
    {
        #region Implements IWeaponDefinition

        public virtual WeaponType Type { get; set; }
        public virtual string Id { get; set; }
        public virtual string DisplayName => default;
        public virtual Sprite Icon => default;
        public virtual float RadiusDetection { get; set; }
        public virtual float AngleView { get; set; }
        public virtual bool IsLeftHand { get; set; }
        public virtual bool CanMoveWhileAttacking { get; }
        public virtual bool CanPushBackOnHit { get; }

        #endregion 
    }
}