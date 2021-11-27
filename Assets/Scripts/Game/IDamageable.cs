using System;
using System.ComponentModel;
using System.Runtime.CompilerServices;

namespace DudeResqueSquad
{
    public interface IDamageable
    {
        event EventHandler<CustomEventArgs.DamageEventArgs> OnTakeDamage;
        event EventHandler<CustomEventArgs.EntityDeadEventArgs> OnDied;
        event EventHandler<CustomEventArgs.HealEventArgs> OnHealed;
        event PropertyChangedEventHandler PropertyChanged;

        float MaxHealth { get; set; }

        float Health
        {
            get;
            set;
        }

        bool IsDead { get; }

        bool IsTakingDamage { get; }

        void TakeDamage(float damage, bool canPushBack, UnityEngine.Vector3 attackDirection);

        void Heal(float newHealth);
        
        /// <summary>
        ///Create the OnPropertyChanged method to raise the event
        /// The calling member's name will be used as the parameter.
        /// </summary>
        /// <param name="name"></param>
        void OnPropertyChanged([CallerMemberName] string name = null);
    }
}