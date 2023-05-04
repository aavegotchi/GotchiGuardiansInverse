using UnityEngine;
using System;
using System.Collections.Generic;
using Gotchi.Events;

public class Projectile : MonoBehaviour
{
    #region Public Variables
    public Transform Target
    {
        set { target = value; }
    }

    public Transform AttackPoint
    {
        set { attackPoint = value; }
    }

    public TurretTowerObjectSO TurretTowerObjectSO
    {
        set { turretTowerObjectSO = value; }
    }
    #endregion

    #region Private Variables
    private Transform target = null;
    private Transform attackPoint = null;
    private LineRenderer lineRenderer = null;
    private GameObject impact = null;
    private float laserTimeoutDefault = 1f;
    private float laserTimeout = 0f;
    private TurretTowerObjectSO turretTowerObjectSO = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        lineRenderer = GetComponent<LineRenderer>();
    }

    void OnEnable()
    {
        transform.position = attackPoint.position;
    }

    void OnDisable()
    {
        if (impact == null) return;
        impact.SetActive(false);
        impact = null;
    }

    void Update()
    {
        if (!target.gameObject.activeInHierarchy)
        {
            gameObject.SetActive(false);

            if (impact == null) return;
            impact.SetActive(false);
            impact = null;
            target = null;
            return;
        }

        if (isLaserAttack())
        {
            lineRenderer.positionCount = 2;
            lineRenderer.SetPosition(0, attackPoint.position);
            lineRenderer.SetPosition(1, target.position);

            if (laserTimeout >= laserTimeoutDefault)
            {
                hitTarget();
                laserTimeout = 0f;
            }
            laserTimeout += Time.deltaTime;
        }
        else
        {
            Vector3 dir = target.position - transform.position;
            float distance = turretTowerObjectSO.projectile.ProjectileSpeed * Time.deltaTime;
            if (dir.magnitude <= distance)
            {
                hitTarget();
                return;
            }

            transform.Translate(dir.normalized * distance, Space.World);
            transform.LookAt(target);
        }
    }

    void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.yellow;
        Gizmos.DrawWireSphere(transform.position, turretTowerObjectSO.projectile.ProjectileExplosiveRadius);
    }
    #endregion

    #region Public Functions() {
        public void ClearLaser()
    {
        lineRenderer.positionCount = 0;
    }
    #endregion

    #region Private Functions
    private bool isLaserAttack()
    {
        return turretTowerObjectSO.projectile.ProjectileType == ProjectileManager.ProjectileType.Laser;
    }

    private void hitTarget()
    {
        if (isLaserAttack())
        {
            Vector3 dir = attackPoint.position - target.position;
            Quaternion rotation = Quaternion.LookRotation(dir);

            if (impact != null)
            {
                impact.transform.position = target.position;
                impact.transform.rotation = rotation;
            }
            else
            {
                impact = ImpactManager.Instance.SpawnImpact(turretTowerObjectSO.projectile.ProjectileImpactType, target.position, rotation);
            }
        }
        else
        {
            ImpactManager.Instance.SpawnImpact(turretTowerObjectSO.projectile.ProjectileImpactType, transform.position, transform.rotation);
        }

        if (turretTowerObjectSO.projectile.ProjectileExplosiveRadius > 0f)
        {
            explode();
        }
        else
        {
            damage(target.gameObject);
        }

        if (isLaserAttack()) return;

        EventBus.TowerEvents.TowerHit(turretTowerObjectSO.Type);

        gameObject.SetActive(false);
    }

    private void explode()
    {
        Collider[] hitColliders = Physics.OverlapSphere(transform.position, turretTowerObjectSO.projectile.ProjectileExplosiveRadius);
        foreach (Collider hitCollider in hitColliders)
        {
            if (hitCollider.tag == "Enemy")
            {
                damage(hitCollider.gameObject);
            }
        }
    }

    private void damage(GameObject enemyObj)
    {
        float attackDamage = turretTowerObjectSO.projectile.ProjectileDamage * turretTowerObjectSO.AttackDamageMultiplier;

        if (enemyObj.tag == "Enemy") 
        {
            Enemy enemy = EnemyManager.Instance.GetEnemyByObject(enemyObj);
            if (enemy == null) return;
            enemy.TakeDamage(attackDamage);
        }
        else if (enemyObj.tag == "Tower")
        {
            var baseTower = enemyObj.GetComponent<BaseTower>();
            var playerGotchi = enemyObj.GetComponent<Player_Gotchi>();
            if (baseTower != null) 
            {
                baseTower.TakeDamage(attackDamage);
            }
            else if (playerGotchi != null)
            {
                playerGotchi.TakeDamage(attackDamage);
            }
            // TODO: this can probably be refactored + we have to assume projectiles can also be fired by enemies/monsters e.g. Aerial Lickquidator
        }
    }
    #endregion
}
