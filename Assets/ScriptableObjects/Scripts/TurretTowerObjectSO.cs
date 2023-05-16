using Gotchi.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretTower", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Tower/TurretTower")]
public class TurretTowerObjectSO : BaseTowerObjectSO
{
    [Header("Attack")]
    public float AttackRange = 2f;
    public float AttackCountdown = 2.5f;
    public int AttackDamageMultiplier = 1;

    [Header("Movement")]
    public float AttackRotationSpeed = 2f;

    [Header("Death Effect Type")]
    public ImpactPool_FX.ImpactType deathEffectType;
    public override ImpactPool_FX.ImpactType DeathEffectType => deathEffectType;

    [Header("Projectile")]
    public ProjectileObjectSO projectile;
}
