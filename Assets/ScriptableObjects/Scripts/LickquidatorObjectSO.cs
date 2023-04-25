using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Lickquidator")]
public class LickquidatorObjectSO : ScriptableObject
{
    public string Name;
    public EnemyManager.EnemyType Type;
    public int Level = 1;

    [Header("Attack")]
    public float AttackDamage = 20f;
    public float AttackRange = 2f;
    public float AttackCountdown = 2.5f;

    [Header("Cost")]
    public int Cost = 75;
    public float buildTime = 2f;

    [Header("Health")]
    public int Health = 75;
    public float OffsetDistance = 20f;

    [Header("Movement")] // (Speed, acceleration, angular) are applied to NavMeshAgent
    public float MovementSpeed = 5f;
    public float MovementAcceleration = 5f;
    public float AngularSpeed = 100f;
    public float AttackRotationSpeed = 2f;

    [Header("Nav Mesh Agent")]
    public float NavMeshAgentHeight = 2f;
    public int NavMeshAgentPriority = 50;
    public float NavMeshAgentRadius = 0.5f;
    public float NavMeshAgentStoppingDistance = 10f;
}
