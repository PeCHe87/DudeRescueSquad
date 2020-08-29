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
            public float damage;

            public DamageEventArgs(float value)
            {
                this.damage = value;
            }
        }
        
        public class HealEventArgs : EventArgs
        {
            public float heal;

            public HealEventArgs(float value)
            {
                this.heal = value;
            }
        }

        public class CollectItemEventArgs : EventArgs
        {
            public ItemData item;
            public string playerUID;

            public CollectItemEventArgs(ItemData item, string playerUID)
            {
                this.item = item;
                this.playerUID = playerUID;
            }
        }

        public class PlayerAttackEventArgs : EventArgs
        {
            public ItemWeaponData weaponData;
            public string playerUID;

            public PlayerAttackEventArgs(ItemWeaponData weapon, string playerUID)
            {
                this.weaponData = weapon;
                this.playerUID = playerUID;
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