using System;
using UnityEngine;

namespace DudeRescueSquad.Core.Weapons
{
    [CreateAssetMenu(fileName = "ProjectileData", menuName = "Data/Weapons/Projectile")]
    public class SimpleProjectileData : ScriptableObject
    {
        public GameObject prefab;
        public float speed;
        public float lifetime;
        public float damage;
        public GameObject hitVfx;
        public LayerMask layerMask;
        public float spread;
    }
}