using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
    public class ItemWeaponData : ItemData
    {
        public Sprite PreviewPic;
        public Enums.WeaponAttackType AttackType;
        public float AnimationSpeed;
        public GameObject AttackEffect;
        public float Damage;
        public float DetectionAreaRadius;
        public float AngleAttackArea;
        public float FireRate;
        public float AttackDelayTime;
        public float ComboDelayTime;
        public float DelayToApplyDamage;
        public float DelayFireEffect;

        [Header("Assault configuration")]
        public projectileActor.projectile ProjectileConfiguration;
        public int CurrentBulletsAmount;
        public int MaxBulletsAmount;
        public bool AutoFire;
        public float DelayBetweenBullets;
        public int BulletsMagazine;
        public int CurrentBulletsMagazine;
        public int BulletsToReload;
        public float ReloadTime;
        public float RemainingReloadTime;
        public bool IsReloading;
        [Tooltip("Updates position of projectile spawner")]
        public Vector3 PositionProjectileSpawner;
        [Tooltip("Updates position of muzzle spawner")]
        public Vector3 PositionMuzzleSpawner;
        [Tooltip("Projectile lifetime duration, after this time it explodes")]
        public float projectileLifetime;

        [Header("Melee configuration")]
        public float CurrentDurability;
        public float MaxDurability;
        [Tooltip("Percentage of durability to consume per effective use of this item")]
        public float DurabilityAmountConsumptionByUse;
        public float DurabilityRecoveryTime;
        public float RemainingDurabilityRecoveryTime;
        public bool IsRecoveringDurability;
    }
}