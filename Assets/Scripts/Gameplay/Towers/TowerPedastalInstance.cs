using UnityEngine;

public class TowerPedastalInstance : GameObjectInstance
{
    private TowerInstance towerInstance = null;

    public TowerInstance TowerInstance
    {
        get { return towerInstance; }
    }

    public delegate void TowerInstanceChanged(TowerPedastalInstance pedastal, TowerInstance newInstance);
    public event TowerInstanceChanged OnTowerInstanceChanged;

    // Start is called before the first frame update
    void Start()
    {
        gameObject.name = "Tower Pedastal Instance: " + ID;
    }

    void SpawnTower(TowerTypeID type)
    {
        if (towerInstance != null)
        {
            Debug.LogError("Tower Pedastal Instance " + ID + " already has a tower instance but tried to created one of type: " + type.ToString() + "!");
            return;
        }

        towerInstance = TowerManager.Singleton.SpawnInstanceOfType(type);

        if (towerInstance != null)
        {
            towerInstance.Pedastal = this;
            towerInstance.transform.SetParent(transform, false);

            if (towerInstance != null)
            {
                OnTowerInstanceChanged?.Invoke(this, towerInstance);
            }
        }
    }

    void RemoveTower()
    {
        if (towerInstance != null)
        {
            TowerManager.Singleton.RemoveTowerInstance(towerInstance);
            towerInstance = null;
            OnTowerInstanceChanged?.Invoke(this, null);
        }
    }
}
