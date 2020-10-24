using UnityEngine;

[CreateAssetMenu(fileName = "EntityData", menuName = "Data/Entity Data")]
public class EntityData : ScriptableObject
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
    public float PatrollingDistanceToStop;
    public LayerMask TargetMaskDetection;
    public LayerMask ObstacleMaskDetection;

    [Header("Chasing")]
    public float SpeedChasingMovement;
    public float MinChasingDistance;
    public float RadiusDetection;
    public float AngleDetection;
    public float ChasingDistanceToStop;

    [Header("Attack")]
    public float RadiusAttack;
    public float DelayBetweenAttacks;
    public float MinDamage;
    public float MaxDamage;
    public float DistanceToStop;

    [Header("Take damage")]
    public float TimeForRecoveringAfterDamage;
}
