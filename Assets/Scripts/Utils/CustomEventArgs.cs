using System;
using UnityEngine;

namespace DudeResqueSquad
{
    public class CustomEventArgs
    {
        public class MovementEventArgs : EventArgs
        {
            public float x;
            public float y;

            public MovementEventArgs(float x, float y)
            {
                this.x = x;
                this.y = y;
            }
        }

        public class DamageEventArgs : EventArgs
        {
            public string entityUID;
            public float damage;

            public DamageEventArgs(string uid, float value)
            {
                this.entityUID = uid;
                this.damage = value;
            }
        }
        
        public class HealEventArgs : EventArgs
        {
            public string entityUID;
            public float heal;

            public HealEventArgs(string uid, float value)
            {
                this.entityUID = uid;
                this.heal = value;
            }
        }

        public class EntityDeadEventArgs : EventArgs
        {
            public string entityUID;

            public EntityDeadEventArgs(string uid)
            {
                this.entityUID = uid;
            }
        }

        public class CollectItemEventArgs : EventArgs
        {
            public string playerUID;
            public ItemData item;

            public CollectItemEventArgs(ItemData item, string playerUID)
            {
                this.playerUID = playerUID;
                this.item = item;
            }
        }

        public class PlayerAttackEventArgs : EventArgs
        {
            public string playerUID;
            public ItemWeaponData weaponData;

            public PlayerAttackEventArgs(ItemWeaponData weapon, string playerUID)
            {
                this.playerUID = playerUID;
                this.weaponData = weapon;
            }
        }

        public class TouchEventArgs
        {
            public Vector3 touchPosition;

            public TouchEventArgs(Vector3 position)
            {
                this.touchPosition = position;
            }
        }

        public class SpawnProjectileEventArgs
        {
            public GameObject prefab;
            public Vector3 positionInitial;
            public Vector3 velocity;
            public float dropSpeed;
            public float lifeTime;
            public float damage;
            public string entityOwnerUID;
            public GameObject prefabHitVFX;
            public LayerMask layerMask;
            public Quaternion initialRotation;
            
            public SpawnProjectileEventArgs(GameObject prefab, Vector3 positionInitial, Vector3 velocity, float dropSpeed, float lifeTime, float damage, string entityOwnerUid, GameObject hitVFX, LayerMask layerMask, Quaternion initialRotation)
            {
                this.prefab = prefab;
                this.positionInitial = positionInitial;
                this.velocity = velocity;
                this.dropSpeed = dropSpeed;
                this.lifeTime = lifeTime;
                this.damage = damage;
                this.entityOwnerUID = entityOwnerUid;
                this.prefabHitVFX = hitVFX;
                this.layerMask = layerMask;
                this.initialRotation = initialRotation;
            }
        }
    }
}