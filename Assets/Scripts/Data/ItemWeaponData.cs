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

        [Header("Projectile configuration")]
        public projectileActor.projectile ProjectileConfiguration;
    }
}