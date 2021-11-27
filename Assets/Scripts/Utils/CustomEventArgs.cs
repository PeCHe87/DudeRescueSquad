using DudeRescueSquad.Core;
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
            public bool attackCanPushBack;
            public Vector3 attackDirection;

            public DamageEventArgs(string uid, float value, bool canPushBack, Vector3 direction)
            {
                this.entityUID = uid;
                this.damage = value;
                this.attackCanPushBack = canPushBack;
                this.attackDirection = direction;
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
            public bool canPushBack;
            public Transform owner;
            
            public SpawnProjectileEventArgs(GameObject prefab, Vector3 positionInitial, Vector3 velocity, float dropSpeed, float lifeTime, float damage, string entityOwnerUid, GameObject hitVFX, LayerMask layerMask, Quaternion initialRotation, bool canPushBack, Transform owner)
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
                this.canPushBack = canPushBack;
                this.owner = owner;
            }
        }

        public class EnemyTargetedArgs
        {
            public Transform target;

            public EnemyTargetedArgs(Transform target)
            {
                this.target = target;
            }
        }

        #region Input event triggers to start and stop action

        public class StartActionEventArgs
        {
            public Enums.ActionType Type;

            public StartActionEventArgs(Enums.ActionType type)
            {
                this.Type = type;
            }
        }

        public class StopActionEventArgs
        {
            public Enums.ActionType Type;

            public StopActionEventArgs(Enums.ActionType type)
            {
                this.Type = type;
            }
        }

        #endregion

        #region Weapon events

        public class WeaponStartReloadingEventArgs
        {
            public float time;

            public WeaponStartReloadingEventArgs(float time)
            {
                this.time = time;
            }
        }

        public class WeaponStopReloadingEventArgs
        {
            public WeaponStopReloadingEventArgs()
            {
            }
        }

        #endregion

        #region Interactable events

        public class InteractableArgs
        {
            public Transform interactableTransform;
            public IInteractable interactable;

            public InteractableArgs(Transform transform, IInteractable interactable)
            {
                this.interactableTransform = transform;
                this.interactable = interactable;
            }
        }

        #endregion
    }
}