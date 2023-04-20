using UnityEngine;

[CreateAssetMenu(fileName = "GameSettings", menuName = "ScriptableObjects/Games/GotchiTowerDefense/GameSettings/Projectile")]
public class ProjectileObjectSO : ScriptableObject
{
    [Header("Attack")]
    public float ProjectileSpeed = 20f;
    public float ProjectileExplosiveRadius = 2f;
    public float ProjectileDamage = 2.5f;

    [Header ("Type")]
    public ProjectileManager.ProjectileType ProjectileType = ProjectileManager.ProjectileType.Fireball;
    public ImpactManager.ImpactType ProjectileImpactType = ImpactManager.ImpactType.Fireball;
}
