using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Gotchi")]
public class GotchiObjectSO : ScriptableObject
{
    public string Name;

    [Header("Abilities")]
    public float SpinAbilityDamage = 40f;
    public float SpinAbilityRange = 5f;
    public float SpinAbilityKnockbackForce = 50f;

    [Header("Attack")]
    public float AttackCountdown = 2.5f;
    public float AttackDamage = 20f;
    public float AttackRange = 2f;

    [Header("Health")]
    public int Health = 250;

    [Header("Movement")] // (Speed, acceleration, angular) are applied to NavMeshAgent
    public float MovementSpeed = 30f;
    public float MovementAcceleration = 10f; 
    public float AngularSpeed = 120f;
    public float AttackRotationSpeed = 2f;

    [Header("Nav Mesh Agent")]
    public float NavMeshAgentHeight = 2f;
    public int NavMeshAgentPriority = 50;
    public float NavMeshAgentRadius = 0.5f;
}
