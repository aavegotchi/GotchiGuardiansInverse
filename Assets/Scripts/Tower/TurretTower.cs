using Gotchi.Audio;
using UnityEngine;

public class TurretTower : BaseTower
{
    #region Public Variables
    [Header("Required Refs")]
    [SerializeField] private Transform partToRotate = null;
    [SerializeField] private Transform attackPoint = null;
    #endregion

    #region Private Variables
    private float attackCountdownTracker = 1f;
    private Transform target = null;
    private TurretTowerObjectSO turretTowerObjectSO;
    private Projectile projectile = null;
    #endregion

    #region Unity Functions
    protected override void Start()
    {
        base.Start();
        turretTowerObjectSO = towerObjectSO as TurretTowerObjectSO;
        InvokeRepeating("updateTarget", 0f, 1f);
    }

    protected override void Update()
    {
        base.Update();

        if (target == null) // Check if the target is not active in the hierarchy
        {
            if (projectile != null && isLaserAttack())
            {
                projectile.gameObject.SetActive(false);
                projectile.ClearLaser();
                projectile = null;
            }
            target = null;
            return;
        }

        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Survival) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);
        if (distanceToTarget > turretTowerObjectSO.AttackRange) // Check if the target is still within the attack range
        {
            target = null;

            if (isLaserAttack())
            {
                projectile.gameObject.SetActive(false);
                projectile.ClearLaser();
                projectile = null;
            }
            return;
        }

        lockOntoTarget();

        if (projectile != null && isLaserAttack()) return; // NOTE: lasers only should ever have one projectile that's in range of target

        attack();
    }


    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turretTowerObjectSO.AttackRange);
    }

    void OnDisable()
    {
        if (projectile == null) return;

        projectile.gameObject.SetActive(false); // disable the aerial lickquidator's laser when lickquidator dies
        projectile = null;
        partToRotate.rotation = Quaternion.identity;
    }
    #endregion

    #region Public Functions
    public override void OnEnemyEnter(Collider collider)
    {
        // Implement your logic for when an enemy enters the collider
    }
    #endregion

    #region Private Functions
    private bool isLaserAttack()
    {
        return turretTowerObjectSO.projectile.ProjectileType == ProjectileManager.ProjectileType.Laser;
    }

    private void attack()
    {
        if (isLaserAttack()) // TODO: AerialLickquidator is using this class instead of LickquidatorAerialAttackLogic -> should generalize attacks to come from either towers or monsters
        {
            projectile = ProjectileManager.Instance.SpawnProjectile(turretTowerObjectSO, attackPoint, target);
        }
        else
        {
            if (attackCountdownTracker <= 0f)
            {
                ProjectileManager.Instance.SpawnProjectile(turretTowerObjectSO, attackPoint, target);
                attackCountdownTracker = turretTowerObjectSO.AttackCountdown;
            }

            attackCountdownTracker -= Time.deltaTime;
        }
    }

    private void lockOntoTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Quaternion parentRotation = transform.parent.gameObject.transform.rotation;
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation * Quaternion.Inverse(parentRotation), lookRotation, Time.deltaTime * turretTowerObjectSO.AttackRotationSpeed).eulerAngles;
        partToRotate.rotation = isLaserAttack()
            ? Quaternion.Euler(0f, rotation.y, rotation.z)
            : Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    private void updateTarget()
    {
        GameObject[] enemies = GameObject.FindGameObjectsWithTag(enemyTag);
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            bool isCloserTarget = distanceToTarget < shortestDistance;
            if (isCloserTarget)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = enemy;
            }
        }

        bool isClosestTarget = nearestTarget != null && shortestDistance <= turretTowerObjectSO.AttackRange;
        if (isClosestTarget)
        {
            target = nearestTarget.transform;
            return;
        }

        target = null;
    }
    #endregion
}
