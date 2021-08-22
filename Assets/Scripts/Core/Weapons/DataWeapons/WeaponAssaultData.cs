using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    [CreateAssetMenu(fileName = "AssaultWeaponData", menuName = "Data/Weapons/Assault weapon")]
    public class WeaponAssaultData : ScriptableObject, IWeaponDefinition, IWeaponAssaultDefinition
    {
        #region Inspector properties

        [SerializeField] private string _id;
        [SerializeField] private WeaponType _type;
        [SerializeField] private float _radiusDetection;
        [SerializeField] private float _angleView = 0;
        [Tools.Information("Indicates in which hand this item will be attached when equipped.", Tools.InformationAttribute.InformationType.Info, false)]
        [SerializeField] private bool _isLeftHand = false;
        [SerializeField] private bool _canMoveWhileAttacking = false;
        [SerializeField] private SimpleProjectileData _projectileData;
        [SerializeField] private int _initialAmmo;
        [SerializeField] private int _maxAmmo;
        [SerializeField] private int _ammoConsumptionPerShot = 0;
        [SerializeField] private float _reloadingTime;
        [SerializeField] private bool _isAutoFire = false;
        [SerializeField] private float _fireRate = 0;

        #endregion

        #region Implements IWeaponDefinition

        public WeaponType Type { get => _type; set => _type = value; }
        public string Id { get => _id; set => _id = value; }
        public float RadiusDetection { get => _radiusDetection; set => _radiusDetection = value; }
        public float AngleView { get => _angleView; set => _angleView = value; }
        public bool IsLeftHand { get => _isLeftHand; set => _isLeftHand = value; }
        public bool CanMoveWhileAttacking { get => _canMoveWhileAttacking; set => _canMoveWhileAttacking = value; }

        #endregion

        #region Implements IWeaponAssaultDefinition

        public SimpleProjectileData ProjectileData { get => _projectileData; set => _projectileData = null; }
        public int MaxAmmo { get => _maxAmmo; set => _maxAmmo = value; }
        public int InitialAmmo { get => _initialAmmo; set => _initialAmmo = value; }
        public bool IsAutoFire { get => _isAutoFire; set => _isAutoFire = value; }
        public float FireRate { get => _fireRate; set => _fireRate = value; }
        public float ReloadingTime { get => _reloadingTime; set => _reloadingTime = value; }
        public int AmmoConsumptionPerShot { get => _ammoConsumptionPerShot; }

        #endregion
    }
}