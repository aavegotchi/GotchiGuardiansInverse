using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Common;

public class TowerGOManager : MonoBehaviour
{
    [Header("Prefabs")]
    [SerializeField] private SerializedDictionary<TowerTypeID, GameObject> TowerPrefabs;

    private Dictionary<TowerTypeID, List<GameObject>> PooledTowerPrefabs;

    // Start is called before the first frame update
    void Start()
    {
        PooledTowerPrefabs = new Dictionary<TowerTypeID, List<GameObject>>();
    }

    TowerInstanceGO GetTowerInstanceGO(TowerInstance instance)
    {
        return null;
    }

    TowerInstanceGO GetTowerInstance(int towerInstanceID)
    {
        return null;
    }
}
