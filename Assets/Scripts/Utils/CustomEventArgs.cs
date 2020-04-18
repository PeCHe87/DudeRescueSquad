using System;

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
    }
}