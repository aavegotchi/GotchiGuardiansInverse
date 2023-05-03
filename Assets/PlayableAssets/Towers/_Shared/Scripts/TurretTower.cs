using Gotchi.Audio;
using UnityEngine;

public class TurretTower : BaseTower
{
    [SerializeField] private Transform partToRotate;
    [SerializeField] private Transform attackPoint;

    private float attackCountdownTracker = 1f;
    private Transform target;
    private Projectile projectile;
    private TurretTowerObjectSO turretTowerObjectSO;

    protected override void Start()
    {
        base.Start();
        turretTowerObjectSO = towerObjectSO as TurretTowerObjectSO;
        InvokeRepeating("UpdateTarget", 0f, 1f);
    }

    protected override void Update()
    {
        base.Update();

        if (target == null) return;

        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Survival) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > turretTowerObjectSO.AttackRange || !target.gameObject.activeInHierarchy)
        {
            ResetTarget();
            return;
        }

        LockOntoTarget();

        if (projectile != null && isLaserAttack()) return;

        Attack();
    }


    private bool isLaserAttack()
    {
        return turretTowerObjectSO.projectile.ProjectileType == ProjectileManager.ProjectileType.Laser;
    }

    private void Attack()
    {
        if (isLaserAttack())
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

    private void LockOntoTarget()
    {
        Vector3 dir = target.position - transform.position;
        Quaternion lookRotation = Quaternion.LookRotation(dir);
        Quaternion parentRotation = transform.parent.gameObject.transform.rotation;
        Vector3 rotation = Quaternion.Lerp(partToRotate.rotation * Quaternion.Inverse(parentRotation), lookRotation, Time.deltaTime * turretTowerObjectSO.AttackRotationSpeed).eulerAngles;
        partToRotate.rotation = isLaserAttack()
            ? Quaternion.Euler(0f, rotation.y, rotation.z)
            : Quaternion.Euler(rotation.x, rotation.y, 0f);
    }

    private void UpdateTarget()
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
            SetTarget(nearestTarget.transform);
        }
        else
        {
            ResetTarget();
        }
    }

    private void SetTarget(Transform newTarget)
    {
        if (target != newTarget)
        {
            ResetTarget();
        }

        target = newTarget;
    }

    private void ResetTarget()
    {
        target = null;

        if (isLaserAttack() && projectile != null)
        {
            projectile.gameObject.SetActive(false);
            projectile.ClearLaser();
            projectile = null;
        }
    }

    private void OnDisable()
    {
        ResetTarget();
        partToRotate.rotation = Quaternion.identity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turretTowerObjectSO.AttackRange);
    }

    public override void OnEnemyEnter(Collider collider)
    {
        // Implement your logic for when an enemy enters the collider
    }
}