using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class TowerPrefabDictionary : SerializableDictionary<TowerTemplate.TowerTypeID, GameObject> { }
[Serializable] public class TowerIconDictionary : SerializableDictionary<TowerTemplate.TowerTypeID, Texture> { }

public class TowerManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private TowerPrefabDictionary TowerPrefabs;

    [SerializeField] private TowerIconDictionary TowerIcons;

    private Dictionary<int, TowerInstance> ActiveTowerInstances;

    public static TowerManager Singleton;

    // Start is called before the first frame update
    void Start()
    {
        ActiveTowerInstances = new Dictionary<int, TowerInstance>();
        Singleton = this;
    }

    public TowerInstance GetTowerInstance(int towerInstanceID)
    {
        if (ActiveTowerInstances.ContainsKey(towerInstanceID))
        {
            return ActiveTowerInstances[towerInstanceID];
        }
        return null;
    }

    public TowerInstance SpawnInstanceOfType(TowerTemplate.TowerTypeID towerTypeID)
    {
        if (TowerPrefabs.ContainsKey(towerTypeID) && TowerPrefabs[towerTypeID] != null)
        {
            GameObject newTowerObj = Instantiate(TowerPrefabs[towerTypeID]);
            TowerInstance towerInstance = newTowerObj.GetComponent<TowerInstance>();

            if (towerInstance != null)
            {
                towerInstance.Template = GameplayData.Singleton.GetTemplateFromType(towerTypeID);
                ActiveTowerInstances.Add(towerInstance.ID, towerInstance);
                return towerInstance;
            }
        }

        return null;
    }

    public void RemoveTowerInstance(TowerInstance towerInstance) 
    {
        ActiveTowerInstances.Remove(towerInstance.ID);
        Destroy(towerInstance.gameObject);
    }

    public List<RadialUIButtonData> GetTowerBuildOptions()
    {
        List<RadialUIButtonData> towerBuildOptions = new List<RadialUIButtonData>();
        foreach (TowerTemplate.TowerTypeID towerTypeID in TowerPrefabs.Keys)
        {
            towerBuildOptions.Add(new RadialUIButtonData_Tower(towerTypeID, TowerIcons[towerTypeID]));
        }
        return towerBuildOptions;
    }
}
