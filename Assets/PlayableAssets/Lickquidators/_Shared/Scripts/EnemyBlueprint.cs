using Gotchi.Lickquidator.Manager;

[System.Serializable]
public class EnemyBlueprint
{
    public LickquidatorManager.LickquidatorType type;
    public int cost;
    public float buildTime;
    public BaseNode node;
    public LickquidatorObjectSO objectSO;
}

