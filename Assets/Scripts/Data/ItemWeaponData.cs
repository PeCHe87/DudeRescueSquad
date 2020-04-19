using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "WeaponData", menuName = "Data/Weapon Data")]
    public class ItemWeaponData : ItemData
    {
        public Sprite PreviewPic;
        public Enums.WeaponAttackType AttackType;
        public float Damage;
        public float DetectionAreaRadius;
        public float AngleAttackArea;
        public float FireRate;
        public float AttackDelayTime;
        public float DelayToApplyDamage;
    }
}