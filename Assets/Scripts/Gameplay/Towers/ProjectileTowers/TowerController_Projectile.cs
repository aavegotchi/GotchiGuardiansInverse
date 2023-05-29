using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerController_Projectile : TowerController
{
    #region Fields
    [SerializeField] 
    private GameObject ProjectileParent;
    [SerializeField] 
    private GameObject ProjectileSpawnPoint;
    [SerializeField]
    private GameObject ProjectileIdlePoint;
    [SerializeField]
    private GameObject TurretRoot; // Optional root that will look at current target (if any)
    #endregion

    #region Private Variables
    private TowerInstance_Projectile _tip;
    private TowerInstance_Projectile TowerInstanceProjectile
    {
        get
        {
            if (_tip == null)
            {
                _tip = TowerInstance as TowerInstance_Projectile;
            }
            return _tip;
        }
    }

    public GameObject CurrentTarget { get; private set; } = null;
    #endregion

    #region Internal Funcs
    protected override bool UpdateIdleState()
    {
        if (!base.UpdateIdleState()) 
        {
            return false;
        }

        UpdateTargetting();

        bool hasValidRotation = UpdateTurretTaretRotation();

        if (TowerInstanceProjectile.PreparedProjectile == null)
        {
            if (!SpawnProjectile())
            {
                return false;
            }
        }

        if (CurrentTarget != null && hasValidRotation)
        {
            FireProjectile();
        }

        return true;
    }

    // Return true if the target is still valid and ready to act on
    private void UpdateTargetting()
    {
        // Step 1: Check if we have a target that is no longer valid (meaning we are back on the market for a target ;)
        if (CurrentTarget != null)
        {
            float distanceToTarget = Vector3.Distance(transform.position, CurrentTarget.transform.position);

            if (distanceToTarget <= TowerInstance.Template.range && CurrentTarget.gameObject.activeInHierarchy)
            {
                // There is nothing to do, we have a target!
                return;
            }
            else
            {
                // Reset the target and enter into the talent search
                CurrentTarget = null;
            }
        }

        // JS - When full system in place, we shouldn't need to search like this
        GameObject[] enemies = GameObject.FindGameObjectsWithTag("Enemy");
        GameObject nearestTarget = null;
        float shortestDistance = Mathf.Infinity;

        foreach (GameObject enemy in enemies)
        {
            float distanceToTarget = Vector3.Distance(transform.position, enemy.transform.position);
            if (enemy.activeInHierarchy && distanceToTarget < TowerInstance.Template.range && distanceToTarget < shortestDistance)
            {
                shortestDistance = distanceToTarget;
                nearestTarget = enemy;
            }
        }

        if (nearestTarget != null)
        {
            CurrentTarget = nearestTarget;
        }
    }

    // Returns true if the rotation is ready for firing
    private bool UpdateTurretTaretRotation()
    {
        bool hasValidRotation = true;

        if (TurretRoot != null && CurrentTarget != null)
        {
            // TODO: Might be worth sharing this calculation with the TowerVisual.cs instead of each calculation the same thing seperately and
            // honestly this one should be the real authority
            Vector3 directionToTarget = CurrentTarget.transform.position - TurretRoot.transform.position;
            directionToTarget.y = 0;  // This ensures the turret only rotates around the y-axis

            if (directionToTarget.sqrMagnitude > 0.0f)  // Only update the rotation if the direction is not zero
            {
                Quaternion targetRotation = Quaternion.LookRotation(directionToTarget);

                if (TurretRoot.transform.rotation != targetRotation)
                {
                    // This smooths out the rotation over time (though still quickly) to prevent too aggressive snapping
                    TurretRoot.transform.rotation = Quaternion.RotateTowards(TurretRoot.transform.rotation, targetRotation, 300f * Time.deltaTime);

                    hasValidRotation = TurretRoot.transform.rotation == targetRotation;

                    if (TowerInstanceProjectile.PreparedProjectile != null)
                    {
                        TowerInstanceProjectile.PreparedProjectile.transform.position = ProjectileIdlePoint.transform.position;
                        TowerInstanceProjectile.PreparedProjectile.transform.rotation = ProjectileIdlePoint.transform.rotation;
                    }
                }
            }
        }

        return hasValidRotation;
    }

    private void FireProjectile()
    {
        if (TowerInstanceProjectile.PreparedProjectile == null)
        {
            Debug.LogError("TowerController[" + TowerInstance.ID + "] TowerInstanceProjectile.PreparedProjectile is null when firing projectile!");
            return;
        }

        if (!TowerInstanceProjectile.PreparedProjectile.ProjectileController.FireAtTarget(CurrentTarget))
        {
            // Skipping logging of warning or errors as projectile controller will handle this
            return;
        }

        Debug.Log("TowerController[" + TowerInstance.ID + "] Firing projectile[" + TowerInstanceProjectile.PreparedProjectile.ID + "] at target: " + CurrentTarget.name);

        TowerInstanceProjectile.PreparedProjectile = null;
    }

    protected override void HandleEnter_CooldownState()
    {
        base.HandleEnter_CooldownState();

        if (TowerInstanceProjectile.PreparedProjectile == null)
        {
            Debug.LogError("TowerController[" + TowerInstance.ID + "] TowerInstanceProjectile.PreparedProjectile is null when entering Cooldown State!");
            return;
        }

        ProjectileController controller = TowerInstanceProjectile.PreparedProjectile.gameObject.GetComponent<ProjectileController>();
        controller.Spawn();
    }

    protected override bool UpdateCooldown()
    {
        base.UpdateCooldown();

        UpdateTargetting();

        UpdateTurretTaretRotation();

        // TODO: Refactor for this logic to be limited to the progress calculation which is broadcasted to specialized classes that handle the movement
        // of the projectile in their own way

        // Check if the projectile is currently in progress
        if (ProjectileSpawnPoint != null && ProjectileIdlePoint != null)
        {
            // Calculate the current progress of the projectile
            float progress = Mathf.Clamp01(TowerInstanceProjectile.CooldownTime / TowerInstanceProjectile.TowerTemplateProjectile.ProjectileSpawnTime);

            // Interpolate the position and rotation from the spawn point to the idle point based on the progress
            TowerInstanceProjectile.PreparedProjectile.transform.position = Vector3.Lerp(ProjectileSpawnPoint.transform.position, ProjectileIdlePoint.transform.position, progress);
            TowerInstanceProjectile.PreparedProjectile.transform.rotation = Quaternion.Lerp(ProjectileSpawnPoint.transform.rotation, ProjectileIdlePoint.transform.rotation, progress);
        }

        // The projectile is still in progress
        return true;
    }

    private bool SpawnProjectile()
    {
        TowerTemplate_Projectile ttp = TowerInstanceProjectile.TowerTemplateProjectile;
        ProjectileInstance.ProjectileTypeID projectileType = ttp.ProjectileType;
        ProjectileInstance newInstance = ProjectileManager.Singleton.ClaimProjectileInstance(projectileType);

        if (newInstance == null)
        {
            Debug.LogError("TowerController[" + TowerInstance.ID + "] Failed to spawn projectile of type: " + projectileType);
            return false;
        }

        newInstance.gameObject.SetActive(true);
        newInstance.SetupFromTower(TowerInstanceProjectile);
        TowerInstanceProjectile.PreparedProjectile = newInstance;
        newInstance.transform.SetParent(ProjectileParent.transform);

        Debug.Log("TowerController[" + TowerInstance.ID + "] Claimed ProjectileInstance[" + newInstance.ID + "]");

        newInstance.OnStateChanged += NewInstance_OnStateChanged;

        if (newInstance.Target != null)
        {
            Debug.LogError("TowerController[" + TowerInstance.ID + "] Acquired ProjectileInstance[" + newInstance.ID + "] which still had a target!");
        }

        if (ttp.BuildLoaded || ttp.ProjectileSpawnTime == 0.0f)
        {
            newInstance.transform.position = ProjectileIdlePoint.transform.position;
            newInstance.transform.rotation = ProjectileIdlePoint.transform.rotation;
            newInstance.EnterIdle();
            return true;
        }
        else
        {
            newInstance.transform.position = ProjectileSpawnPoint.transform.position;
            newInstance.transform.rotation = ProjectileSpawnPoint.transform.rotation;
            ProjectileController controller = newInstance.gameObject.GetComponent<ProjectileController>();

            TowerInstanceProjectile.EnterCooldown();
            return false;
        }
    }

    private void NewInstance_OnStateChanged(ProjectileInstance instance, ProjectileInstance.State newState)
    {
        if (newState == ProjectileInstance.State.Dead)
        {
            ReleaseProjectile(instance);
        }
    }

    private void ReleaseProjectile(ProjectileInstance projectile)
    {
        Debug.Log("TowerController[" + TowerInstance.ID + "] Releasing ProjectileInstance[" + projectile.ID + "]");
        projectile.OnStateChanged -= NewInstance_OnStateChanged;
        ProjectileManager.Singleton.FreeProjectileInstance(projectile);
    }
    #endregion
}
