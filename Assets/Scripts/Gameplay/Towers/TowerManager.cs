using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

[Serializable] public class TowerPrefabDictionary : SerializableDictionary<TowerTypeID, TowerInstance> { }
[Serializable] public class TowerIconDictionary : SerializableDictionary<TowerTypeID, Texture> { }

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

    public TowerInstance SpawnInstanceOfType(TowerTypeID towerTypeID)
    {
        if (TowerPrefabs.ContainsKey(towerTypeID) && TowerPrefabs[towerTypeID] != null)
        {
            TowerInstance towerInstance = Instantiate(TowerPrefabs[towerTypeID]);
            towerInstance.Template = GameplayData.Singleton.GetTemplateFromType(towerTypeID);
            ActiveTowerInstances.Add(towerInstance.ID, towerInstance);
            return towerInstance;
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
        foreach (TowerTypeID towerTypeID in TowerPrefabs.Keys)
        {
            towerBuildOptions.Add(new RadialUIButtonData(TowerIcons[towerTypeID]));
        }
        return towerBuildOptions;
    }
}
