using Gotchi.Audio;
using UnityEngine;

[CreateAssetMenu(fileName = "TurretTower", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Tower/TurretTower")]
public class TurretTowerObjectSO : BaseTowerObjectSO
{
    [Header("Attack")]
    public float AttackRange = 2f;
    public float AttackCountdown = 2.5f;
    public float AttackDamageMultiplier = 1f;

    [Header("Movement")]
    public float AttackRotationSpeed = 2f;

    [Header("Death Effect Type")]
    public ImpactManager.ImpactType deathEffectType;
    public override ImpactManager.ImpactType DeathEffectType => deathEffectType;

    [Header("Projectile")]
    public ProjectileObjectSO projectile;
}
