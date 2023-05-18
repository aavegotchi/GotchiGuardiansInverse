public class TowerPedastal : GameObjectInstance
{
    private TowerInstance towerInstance = null;

    public TowerInstance TowerInstance
    {
        get { return towerInstance; }
    }

    public TowerPedastal()
    {

    }

    void BuildTower(TowerTypeID type)
    {
        towerInstance = new TowerInstance(GameplayData.GetCurrentData().GetTemplateFromType(type), this);
    }
}
