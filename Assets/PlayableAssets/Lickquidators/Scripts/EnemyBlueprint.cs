using UnityEngine;

[System.Serializable]
public class EnemyBlueprint
{
    public EnemyPool.EnemyType type;
    public int cost;
    public float buildTime;
    public BaseNode node;
    public LickquidatorObjectSO objectSO;
}

