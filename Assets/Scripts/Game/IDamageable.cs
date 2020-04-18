using System;

namespace DudeResqueSquad
{
    public interface IDamageable
    {
        event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        event EventHandler OnDied;
        event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;

        float MaxHealth { get; set; }

        float Health { get; set; }

        bool IsDead { get; }

        void TakeDamage(float damage);

        void Heal(float newHealth);
    }
}