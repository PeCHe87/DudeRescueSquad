using UnityEngine;

namespace DudeResqueSquad
{
    [CreateAssetMenu(fileName = "EnemyData", menuName = "Data/Enemy Data")]
    public class EnemyData : ScriptableObject
    {
        public string UID;

        [Header("Health")]
        public int MaxHealth;
        public int CurrentHealth;

        [Header("Idle")]
        public float MinIdleTime;
        public float MaxIdleTime;

        [Header("Patrolling")]
        public float SpeedPatrollingMovement;
        public float MinPatrollingTime;
        public float MaxPatrollingTime;
        public LayerMask TargetMaskDetection;
        public LayerMask ObstacleMaskDetection;

        [Header("Chasing")]
        public float SpeedChasingMovement;
        public float MinChasingDistance;
        public float RadiusDetection;
        public float AngleDetection;

        [Header("Attack")]
        public float RadiusAttack;
        public float DelayBetweenAttacks;
        public float MinDamage;
        public float MaxDamage;

        [Header("Take damage")]
        public float TimeForRecoveringAfterDamage;
    }
}