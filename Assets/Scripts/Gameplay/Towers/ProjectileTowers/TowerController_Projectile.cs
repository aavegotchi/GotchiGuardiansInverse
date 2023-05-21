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
    #endregion

    #region Internal Funcs
    protected override bool UpdateIdleState()
    {
        if (!base.UpdateIdleState()) 
        {
            return false;
        }

        if (TowerInstanceProjectile.PreparedProjectile == null)
        {
            if (!SpawnProjectile())
            {
                return false;
            }
        }

        return true;
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

        newInstance.SetupFromTower(TowerInstanceProjectile);
        TowerInstanceProjectile.PreparedProjectile = newInstance;
        newInstance.transform.SetParent(ProjectileParent.transform);
        

        if (ttp.BuildLoaded || ttp.ProjectileSpawnTime == 0.0f)
        {
            newInstance.transform.position = ProjectileIdlePoint.transform.position;
            newInstance.transform.rotation = ProjectileIdlePoint.transform.rotation;
            newInstance.CurrentState = ProjectileInstance.State.Idle;
            return true;
        }
        else
        {
            newInstance.transform.position = ProjectileSpawnPoint.transform.position;
            newInstance.transform.rotation = ProjectileSpawnPoint.transform.rotation;

            TowerInstanceProjectile.CurrentState = TowerInstance.State.Cooldown;
            return false;
        }
    }
    #endregion
}
