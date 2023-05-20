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

    public override IDataTemplate DrawDataInspectors(GameplayData data)
    {
        IDataTemplate returnVal = base.DrawDataInspectors(data);

        if (returnVal != this)
        {
            return returnVal;
        }

        EditorGUILayout.LabelField("Projectile Data", EditorStyles.boldLabel);

        ProjectileType = (ProjectileInstance.ProjectileTypeID)EditorGUILayout.EnumPopup("Projectile Type", ProjectileType);
        ProjectileDamage = EditorGUILayout.IntField("Projectile Damage", ProjectileDamage);
        ProjectileSpeed = EditorGUILayout.FloatField("Projectile Speed", ProjectileSpeed);

        return this;
    }
}
