using Gotchi.Audio;
using System.Runtime.Serialization.Formatters;
using UnityEngine;

public class TurretTower : BaseTower
{
    #region Fields
    [SerializeField] private Transform partToRotate;
    [SerializeField] private Transform attackPoint;
    #endregion

    #region Private Variables
    private float attackCountdownTracker = 1f;
    private Transform target;
    private Projectile projectile;
    private TurretTowerObjectSO turretTowerObjectSO;
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

        if (target == null) return;

        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Survival) return;

        float distanceToTarget = Vector3.Distance(transform.position, target.position);

        if (distanceToTarget > turretTowerObjectSO.AttackRange || !target.gameObject.activeInHierarchy)
        {
            resetTarget();
            return;
        }

        lockOntoTarget();

        if (projectile != null && isLaserAttack()) return;

        attack();
    }

    private void OnMouseEnter()
    {
        if(!isNodeUIOpenOnThis && PhaseManager.Instance.CurrentPhase == PhaseManager.Phase.Prep)
        {
            if (rangeCircle != null)
            {
                rangeCircle.ToggleActive(true);
            }
        }
    }

    // Add this new function to handle mouse hover exit event
    private void OnMouseExit()
    {
        if (rangeCircle != null && !isNodeUIOpenOnThis)
        {
            rangeCircle.ToggleActive(false);
        }
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
        return turretTowerObjectSO.projectile.ProjectileType == ProjectilePool_FX.ProjectileType.Laser;
    }

    private void attack()
    {
        if (isLaserAttack())
        {
            projectile = ProjectilePool_FX.Instance.SpawnProjectile(turretTowerObjectSO, attackPoint, target);
        }
        else
        {
            if (attackCountdownTracker <= 0f)
            {
                ProjectilePool_FX.Instance.SpawnProjectile(turretTowerObjectSO, attackPoint, target);
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
            setTarget(nearestTarget.transform);
        }
        else
        {
            resetTarget();
        }
    }

    private void setTarget(Transform newTarget)
    {
        if (target != newTarget)
        {
            resetTarget();
        }

        target = newTarget;
    }

    private void resetTarget()
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
        resetTarget();
        partToRotate.rotation = Quaternion.identity;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, turretTowerObjectSO.AttackRange);
    }
    #endregion
}

