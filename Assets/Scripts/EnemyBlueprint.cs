using UnityEngine;

[System.Serializable]
public class EnemyBlueprint
{
    public EnemyManager.EnemyType type;
    public int cost;
    public float buildTime;
    public BaseNode node;
}

