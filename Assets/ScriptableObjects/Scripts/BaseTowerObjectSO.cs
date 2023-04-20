using Gotchi.Audio;
using UnityEngine;

public abstract class BaseTowerObjectSO : ScriptableObject
{
    public string Name;
    public TowerManager.TowerType Type;

    [Header("Cost")]
    public int Cost = 75;
    public float buildTime = 2f;

    [Header("Health")]
    public int Health = 75;
   
    public abstract ImpactManager.ImpactType DeathEffectType { get; }
}