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
    [SerializeField] private TowerPrefabDictionary TowerVisualPrefabs;
    [SerializeField] private GameObject TowerVisualPoolRoot;
    [SerializeField] private int InitialPoolSize = 4;

    [SerializeField] private TowerIconDictionary TowerIcons;

    private Dictionary<int, TowerInstance> ActiveTowerInstances = new Dictionary<int, TowerInstance>();

    private Dictionary<TowerTemplate.TowerTypeID, List<TowerVisual>> TowerVisualPool = new Dictionary<TowerTemplate.TowerTypeID, List<TowerVisual>>();

    [HideInInspector]
    public static TowerManager Singleton;

    // Start is called before the first frame update
    void Start()
    {
        Singleton = this;

        foreach (KeyValuePair<TowerTemplate.TowerTypeID, GameObject> kvp in TowerVisualPrefabs)
        {
            if (kvp.Value != null)
            {
                IncreasePoolOfVisuals(kvp.Key, InitialPoolSize);
            }
        }
    }

    #region public interfaces

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

    public TowerVisual ClaimTowerVisual(TowerTemplate.TowerTypeID typeID)
    {
        if (!TowerVisualPool.ContainsKey(typeID) || TowerVisualPool[typeID].Count == 0)
        {
            IncreasePoolOfVisuals(typeID, 1);
        }

        if (TowerVisualPool.ContainsKey(typeID) && TowerVisualPool[typeID].Count > 0)
        {
            TowerVisual newVisual = TowerVisualPool[typeID][0];
            TowerVisualPool[typeID].RemoveAt(0);
            return newVisual;
        }
        else
        {
            Debug.LogError("Failed to claim tower visual of type " + typeID.ToString() + " even after increasing pool size");
        }

        return null;
    }

    public void FreeTowerVisual(TowerVisual towerVisual)
    {
        // For this to be true, the Tower had to be created in a non-traditional way
        if (!TowerVisualPool.ContainsKey(towerVisual.TypeID))
        {
            TowerVisualPool[towerVisual.TypeID] = new List<TowerVisual>();
            Debug.LogWarning("Tower Visual Pool Map lacked entry for " + towerVisual.TypeID.ToString() + " when freeing Tower visual");
        }

        if (TowerVisualPool[towerVisual.TypeID].Contains(towerVisual))
        {
            Debug.LogError("Tower Visual Pool Map already contained " + towerVisual.TypeID.ToString() + " when freeing Tower visual");
            return;
        }

        TowerVisualPool[towerVisual.TypeID].Add(towerVisual);
        towerVisual.transform.SetParent(TowerVisualPoolRoot.transform);
        towerVisual.gameObject.SetActive(false);
        towerVisual.Cleanup();
    }
    #endregion

    #region Internal functions
    private void IncreasePoolOfVisuals(TowerTemplate.TowerTypeID typeID, int initialPoolSize)
    {
        if (!TowerVisualPrefabs.ContainsKey(typeID) || TowerVisualPrefabs[typeID] == null)
        {
            Debug.LogError("Tower Visual " + typeID.ToString() + " tried to increase tower visual pool size without valid prefab assigned");
            return;
        }

        if (!TowerVisualPool.ContainsKey(typeID))
        {
            TowerVisualPool.Add(typeID, new List<TowerVisual>());
        }

        for (int i = 0; i < initialPoolSize; i++)
        {
            TowerVisual newTowerVisual = GenerateTowerVisual(typeID);
            if (newTowerVisual != null)
            {
                newTowerVisual.gameObject.SetActive(false);
                newTowerVisual.transform.SetParent(TowerVisualPoolRoot.transform);
                TowerVisualPool[typeID].Add(newTowerVisual);
            }
        }
    }

    private TowerVisual GenerateTowerVisual(TowerTemplate.TowerTypeID typeID)
    {
        if (!TowerVisualPrefabs.ContainsKey(typeID) || TowerVisualPrefabs[typeID] == null)
        {
            Debug.LogError("Tower Visual " + typeID.ToString() + " tried to generate tower visual without valid prefab assigned");
            return null;
        }

        GameObject newTowerObj = Instantiate(TowerVisualPrefabs[typeID]);
        newTowerObj.SetActive(false);
        TowerVisual towerVisual = newTowerObj.GetComponent<TowerVisual>();
        if (towerVisual == null)
        {
            Debug.LogError("Tower Visual " + typeID.ToString() + " Prefab lacks Tower Visual script!");
            return null;
        }

        if (towerVisual.TypeID != typeID)
        {
            Debug.LogWarning("Tower Visual " + typeID.ToString() + " Prefab has mismatched internal type ID! Force Fixed it!");
            towerVisual.TypeID = typeID;
        }

        return towerVisual;
    }
    #endregion
}
