
public class TowerInstance : GameObjectInstance
{
    private TowerTemplate template;
    public TowerTemplate Template
    { 
        get 
        { 
            return template; 
        } 
    }

    private TowerPedastal pedastal;

    bool _isActive = false;

    public TowerInstance(TowerTemplate temp, TowerPedastal pedastal) 
    {
        template = temp;
        Build();
    }

    private void Build()
    {

    }
}
