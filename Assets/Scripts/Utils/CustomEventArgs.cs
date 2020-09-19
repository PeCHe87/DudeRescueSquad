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
    }
}