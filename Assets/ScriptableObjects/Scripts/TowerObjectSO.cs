using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Tower")]
public class TowerObjectSO : ScriptableObject
{
    public string Name;
    public TowerPool.TowerType Type;

    [Header("Attack")]
    public float AttackDamage = 20f;
    public float AttackRange = 2f;
    public float AttackCountdown = 2.5f;

    [Header("Cost")]
    public int Cost = 75;
    public float BuildTime = 2f;

    [Header("Health")]
    public int Health = 75;

    [Header("Movement")]
    public float AttackRotationSpeed = 2f;
}
