using System.Collections.Generic;
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
public class TowerTemplate
{
    public string name;
    public string tooltipText;
    public TowerTypeID type;
    public TowerTiers tier;
    public int buildCost;
    public float buildTime;
    public float range;

    public override string ToString()
    {
        return name;
    }
}