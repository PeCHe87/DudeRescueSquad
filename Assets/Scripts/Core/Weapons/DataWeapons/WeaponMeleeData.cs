using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    [CreateAssetMenu(fileName = "MeleeWeaponData", menuName = "Data/Weapons/Melee weapon")]
    public class WeaponMeleeData : ScriptableObject, IWeaponDefinition, IWeaponMeleeDefinition
    {
        #region Inspector properties

        [SerializeField] private string _id;
        [SerializeField] private WeaponType _type;
        [SerializeField] private Sprite _icon = default;
        [SerializeField] private float _attackDamage;
        [SerializeField] private float _attackRange;
        [SerializeField] private float _attackDuration;
        [SerializeField] private float _radiusDetection;
        [SerializeField] private float _angleView = 0;
        [Tools.Information("Indicates in which hand this item will be attached when equipped.", Tools.InformationAttribute.InformationType.Info, false)]
        [SerializeField] private bool _isLeftHand = false;
        [SerializeField] private bool _canMoveWhileAttacking = false;
        [SerializeField] private bool _canPushBackOnHit = false;
        [SerializeField] private bool _canMoveForwardDuringAttack = false;
        [SerializeField] private float _attackMoveForwardSpeed = 0;
        [SerializeField] private float _attackMoveForwardDuration = 0;

        #endregion

        #region Implements IWeaponDefinition

        public WeaponType Type { get => _type; set => _type = value; }
        public string Id { get => _id; set => _id = value; }
        public Sprite Icon => _icon;
        public float RadiusDetection { get => _radiusDetection; set => _radiusDetection = value; }
        public float AngleView { get => _angleView; set => _angleView = value; }
        public bool IsLeftHand { get => _isLeftHand; set => _isLeftHand = value; }
        public bool CanMoveWhileAttacking => _canMoveWhileAttacking;
        public bool CanPushBackOnHit { get => _canPushBackOnHit; }
        public bool CanMoveForwardDuringAttack => _canMoveForwardDuringAttack;
        public float AttackMoveForwardSpeed => _attackMoveForwardSpeed;
        public float AttackMoveForwardDuration => _attackMoveForwardDuration;

        #endregion

        #region Implements IWeaponMeleeDefinition

        public float AttackRange { get => _attackRange; set => _attackRange = value; }
        public float AttackDuration { get => _attackDuration; set => _attackDuration = value; }
        public float AttackDamage { get => _attackDamage; set => _attackDamage = value; }

        #endregion
    }
}