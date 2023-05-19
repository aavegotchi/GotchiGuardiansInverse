using System;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

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


[System.Serializable]
public class TowerTemplate : IDataTemplate
{
    public string name;
    public string tooltipText;
    public TowerTypeID type;
    public TowerTiers tier;
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

    public override void DrawDataInspectors()
    {
        name = EditorGUILayout.TextField("Tower Name", name);
        tooltipText = EditorGUILayout.TextField("Tooltip Text", tooltipText);
        type = (TowerTypeID)EditorGUILayout.EnumPopup("Tower Type", type);
        tier = (TowerTiers)EditorGUILayout.EnumPopup("Tower Tier", tier);
        buildCost = EditorGUILayout.IntField("Build Cost", buildCost);
        buildTime = EditorGUILayout.FloatField("Build Time", buildTime);
        range = EditorGUILayout.FloatField("Range", range);
    }
}