using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public int MaxHealth;
        public int CurrentHealth;
        public float SpeedPatrollingMovement;
        public float SpeedChasingMovement;
        public float RadiusDetection;
        public float RadiusAttack;
        public float DelayBetweenAttacks;
        public Enums.EnemyStates State;
        public float MinDamage;
        public float MaxDamage;
    }
}