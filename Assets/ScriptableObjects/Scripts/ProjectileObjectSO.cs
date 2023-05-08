using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Projectile")]
public class ProjectileObjectSO : ScriptableObject
{
    [Header("Attack")]
    public float ProjectileSpeed = 20f;
    public float ProjectileExplosiveRadius = 2f;
    public float ProjectileDamage = 2.5f;

    [Header ("Type")]
    public ProjectilePool_FX.ProjectileType ProjectileType = ProjectilePool_FX.ProjectileType.Fireball;
    public ImpactPool_FX.ImpactType ProjectileImpactType = ImpactPool_FX.ImpactType.Fireball;
}
