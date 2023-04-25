using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public TowerManager.TowerType type;
    public int cost;
    public float buildTime;
    public BaseNode node;
    public BaseTowerObjectSO objectSO;
}

