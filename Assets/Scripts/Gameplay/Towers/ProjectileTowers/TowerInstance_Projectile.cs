using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerInstance_Projectile : TowerInstance
{
    [Header("Dynamic Debug - null in prefab")]
    public ProjectileInstance PreparedProjectile;
    public List<ProjectileInstance> LaunchedProjectiles;

    [HideInInspector]
    private TowerTemplate_Projectile _TTP = null;
    public TowerTemplate_Projectile TowerTemplateProjectile
    {
        get 
        { 
            if (_TTP == null)
            {
                _TTP = Template as TowerTemplate_Projectile;
            }
            return _TTP; 
        }
    }

    protected override void UpdateCooldown()
    {
        base.UpdateCooldown();

        CooldownTime = Mathf.Clamp(CooldownTime + Time.deltaTime, 0.0f, TowerTemplateProjectile.ProjectileAttackCooldown);

        if (CooldownTime >= TowerTemplateProjectile.ProjectileAttackCooldown && PreparedProjectile.CurrentState == ProjectileInstance.State.Idle)
        {
            FinishCooldown();
        }
    }
}
