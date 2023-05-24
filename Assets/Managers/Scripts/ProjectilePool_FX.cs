using System.Collections.Generic;
using UnityEngine;
using GameMaster;
using Gotchi.Lickquidator.Manager;

// DEPRECATED
public class ProjectilePool_FX : MonoBehaviour
{
    #region Public Variables
    public static ProjectilePool_FX Instance = null;

    public enum ProjectileType
    {
        Cannonball,
        Missile,
        Arrow,
        Fireball,
        Iceball,
        Laser
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject cannonballPrefab = null;
    [SerializeField] private GameObject missilePrefab = null;
    [SerializeField] private GameObject arrowPrefab = null;
    [SerializeField] private GameObject fireballPrefab = null;
    [SerializeField] private GameObject iceballPrefab = null;
    [SerializeField] private GameObject laserPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int cannonballPoolSize = 40;
    [SerializeField] private int missilePoolSize = 10;
    [SerializeField] private int arrowPoolSize = 10;
    [SerializeField] private int fireballPoolSize = 10;
    [SerializeField] private int iceballPoolSize = 10;
    [SerializeField] private int laserPoolSize = 10;
    #endregion

    #region Private Variables
    private List<Projectile> cannonballPool = new List<Projectile>();
    private List<Projectile> missilePool = new List<Projectile>();
    private List<Projectile> arrowPool = new List<Projectile>();
    private List<Projectile> fireballPool = new List<Projectile>();
    private List<Projectile> iceballPool = new List<Projectile>();
    private List<Projectile> laserPool = new List<Projectile>();
    #endregion

    #region Unity Functions
    private void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    private void Start()
    {
        cannonballPool = createProjectilePool(cannonballPrefab, cannonballPoolSize);
        missilePool = createProjectilePool(missilePrefab, missilePoolSize);
        arrowPool = createProjectilePool(arrowPrefab, arrowPoolSize);
        fireballPool = createProjectilePool(fireballPrefab, fireballPoolSize);
        iceballPool = createProjectilePool(iceballPrefab, iceballPoolSize);
        laserPool = createProjectilePool(laserPrefab, laserPoolSize);
    }
    #endregion

    #region Public Functions
    public Projectile SpawnProjectile(TurretTowerObjectSO turretTowerObjectSO, Transform attackPoint, Transform target)
    {
        ProjectileType projectileType = turretTowerObjectSO.projectile.ProjectileType;
        List<Projectile> projectilePool = getProjectilePool(turretTowerObjectSO.projectile.ProjectileType);

        foreach (Projectile projectile in projectilePool)
        {
            bool isProjectileNotAvailable = projectile.gameObject.activeSelf;
            if (isProjectileNotAvailable) continue;

            projectile.AttackPoint = attackPoint;
            projectile.Target = target;
            projectile.TurretTowerObjectSO = turretTowerObjectSO;
            projectile.gameObject.SetActive(true);

            if (projectileType == ProjectileType.Laser)
            {
                GameMasterEvents.EnemyEvents.EnemyAttacked(LickquidatorManager.LickquidatorType.AerialLickquidator);
            }
            else
            {
                GameMasterEvents.TowerEvents.TowerAttacked(turretTowerObjectSO.Type);
            }
            
            return projectile;
        }

        return null;
    }
    #endregion

    #region Private Functions
    private List<Projectile> createProjectilePool(GameObject projectilePrefab, int poolSize)
    {
        projectilePrefab.SetActive(false);

        List<Projectile> projectilePool = new List<Projectile>();
        for (int i=0; i<poolSize; i++)
        {
            GameObject projectileObj = Instantiate(projectilePrefab, Vector3.zero, Quaternion.identity, transform);
            Projectile projectile = projectileObj.GetComponent<Projectile>();
            projectilePool.Add(projectile);
        }
        
        return projectilePool;
    }

    private List<Projectile> getProjectilePool(ProjectileType type)
    {
        List<Projectile> projectilePool = null;
        switch (type)
        {
            case ProjectileType.Cannonball:
                projectilePool = cannonballPool;
                break;
            case ProjectileType.Missile:
                projectilePool = missilePool;
                break;
            case ProjectileType.Arrow:
                projectilePool = arrowPool;
                break;
            case ProjectileType.Fireball:
                projectilePool = fireballPool;
                break;
            case ProjectileType.Iceball:
                projectilePool = iceballPool;
                break;
            case ProjectileType.Laser:
                projectilePool = laserPool;
                break;
        }
        return projectilePool;
    }
    #endregion
}