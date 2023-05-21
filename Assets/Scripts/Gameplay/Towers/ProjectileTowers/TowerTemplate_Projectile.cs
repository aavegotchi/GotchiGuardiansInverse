using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TowerTemplate_Projectile : TowerTemplate
{
    public ProjectileInstance.ProjectileTypeID ProjectileType;
    public int ProjectileDamage;
    public float ProjectileSpeed;
    public bool BuildLoaded = false;
    public float ProjectileSpawnTime;
    public float ProjectileAttackCooldown;

    public override IDataTemplate DrawDataInspectors(GameplayData data)
    {
        IDataTemplate returnVal = base.DrawDataInspectors(data);

        if (returnVal != this)
        {
            return returnVal;
        }

        EditorGUILayout.LabelField("Projectile Data", EditorStyles.boldLabel);

        ProjectileInstance.ProjectileTypeID selectedType = (ProjectileInstance.ProjectileTypeID)EditorGUILayout.EnumPopup("Projectile Type", ProjectileType);

        if (selectedType != ProjectileInstance.ProjectileTypeID.INVALID)
        {
            ProjectileType = selectedType;
        }


        // Validate and clamp the property values
        ProjectileDamage = Mathf.Max(0, EditorGUILayout.IntField("Projectile Damage", ProjectileDamage));
        ProjectileSpeed = Mathf.Max(0f, EditorGUILayout.FloatField("Projectile Speed", ProjectileSpeed));
        ProjectileSpawnTime = Mathf.Max(0f, EditorGUILayout.FloatField("Projectile Spawn Time", ProjectileSpawnTime));

        BuildLoaded = EditorGUILayout.Toggle("Build Loaded", BuildLoaded);

        float attackCooldown = EditorGUILayout.FloatField("Attack Cooldown", ProjectileAttackCooldown);

        // Ensure the Attack Cooldown is not less than the Spawn Time
        if (attackCooldown < ProjectileSpawnTime)
        {
            ProjectileAttackCooldown = ProjectileSpawnTime;
        }
        else
        {
            ProjectileAttackCooldown = Mathf.Max(0f, attackCooldown);
        }

        return this;
    }

}
