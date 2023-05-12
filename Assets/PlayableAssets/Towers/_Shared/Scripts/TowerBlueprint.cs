using UnityEngine;

[System.Serializable]
public class TowerBlueprint
{
    public TowerPool.TowerType type;
    public int cost;
    public float buildTime;
    public BaseNode node;
    public BaseTowerObjectSO objectSO;
}

