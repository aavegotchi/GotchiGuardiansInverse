using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

[System.Serializable]
public class TowerTemplate : IDataTemplate
{
    public enum TowerTypeID
    {
        Brute_Force = 1,
        ROFL_Launcher,
        Bull_Market_Den
    }

    public enum TowerTiers
    {
        TowerTier_1,
        TowerTier_2,
        TowerTier_3,
    }

    public enum TowerBehavior
    {
        TowerBehavior_Basic,
        TowerBehavior_Projectile,
    }

    public string name;
    public string tooltipText;
    public TowerTypeID type;
    public TowerTiers tier;
    public TowerBehavior behavior;
    public int buildCost;
    public float buildTime;
    public float range;

    public static void EnsureAllTowers(GameplayData data)
    {
        foreach (TowerTypeID id in Enum.GetValues(typeof(TowerTypeID)))
        {
            if (data.GetTemplateFromType(id) == null) {
                TowerTemplate newTemplate = new TowerTemplate();
                newTemplate.name = id.ToString();
                newTemplate.type = id;
                data.towerTemplates.Add(newTemplate);
            }
        }
    }

    public override IDataTemplate DrawDataInspectors(GameplayData data)
    {
        EditorGUILayout.LabelField("Tower Type", type.ToString());
        name = EditorGUILayout.TextField("Tower Name", name);
        tooltipText = EditorGUILayout.TextField("Tooltip Text", tooltipText);

        TowerBehavior newBehavior = (TowerBehavior)EditorGUILayout.EnumPopup("Tower Behavior", behavior);
        if (newBehavior != behavior)
        {
            TowerTemplate newTemplate = null;

            switch (newBehavior)
            {
                case TowerBehavior.TowerBehavior_Basic:
                    newTemplate = new TowerTemplate();
                    break;
                case TowerBehavior.TowerBehavior_Projectile:
                    newTemplate = new TowerTemplate_Projectile();
                    break;
            }

            if (newTemplate != null)
            {
                newTemplate.name = name;
                newTemplate.tooltipText = tooltipText;
                newTemplate.type = type;
                newTemplate.tier = tier;
                newTemplate.behavior = newBehavior;
                newTemplate.buildCost = buildCost;
                newTemplate.buildTime = buildTime;
                newTemplate.range = range;

                int index = data.towerTemplates.IndexOf(this);
                data.towerTemplates.Remove(this);
                data.towerTemplates.Insert(index, newTemplate);
            }

            return newTemplate;
        }

        tier = (TowerTiers)EditorGUILayout.EnumPopup("Tower Tier", tier);
        buildCost = EditorGUILayout.IntField("Build Cost", buildCost);
        buildTime = EditorGUILayout.FloatField("Build Time", buildTime);
        range = EditorGUILayout.FloatField("Range", range);

        return this;
    }
}