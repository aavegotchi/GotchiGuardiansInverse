using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;

public class TowerManager : MonoBehaviour
{
    #region Public Variables
    public static TowerManager Instance = null;

    public enum TowerType
    {
        None,
        BasicTower,
        BombTower,
        SlowTower,
        ArrowTower1,
        ArrowTower2,
        ArrowTower3,
        FireTower1,
        FireTower2,
        FireTower3,
        IceTower1,
        IceTower2,
        IceTower3,
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject basicTowerPrefab = null;
    [SerializeField] private GameObject bombTowerPrefab = null;
    [SerializeField] private GameObject slowTowerPrefab = null;
    [SerializeField] private GameObject arrowTower1Prefab = null;
    [SerializeField] private GameObject arrowTower2Prefab = null;
    [SerializeField] private GameObject arrowTower3Prefab = null;
    [SerializeField] private GameObject fireTower1Prefab = null;
    [SerializeField] private GameObject fireTower2Prefab = null;
    [SerializeField] private GameObject fireTower3Prefab = null;
    [SerializeField] private GameObject iceTower1Prefab = null;
    [SerializeField] private GameObject iceTower2Prefab = null;
    [SerializeField] private GameObject iceTower3Prefab = null;

    [Header("Attributes")]
    [SerializeField] private int basicTowerPoolSize = 10;
    [SerializeField] private int bombTowerPoolSize = 10;
    [SerializeField] private int slowTowerPoolSize = 10;
    [SerializeField] private int arrowTowerPoolSize = 10;
    [SerializeField] private int fireTowerPoolSize = 10;
    [SerializeField] private int iceTowerPoolSize = 10;
    #endregion

    #region Private Variables
    private List<GameObject> basicTowerPool = new List<GameObject>();
    private List<GameObject> bombTowerPool = new List<GameObject>();
    private List<GameObject> slowTowerPool = new List<GameObject>();
    private List<GameObject> arrowTower1Pool = new List<GameObject>();
    private List<GameObject> arrowTower2Pool = new List<GameObject>();
    private List<GameObject> arrowTower3Pool = new List<GameObject>();
    private List<GameObject> fireTower1Pool = new List<GameObject>();
    private List<GameObject> fireTower2Pool = new List<GameObject>();
    private List<GameObject> fireTower3Pool = new List<GameObject>();
    private List<GameObject> iceTower1Pool = new List<GameObject>();
    private List<GameObject> iceTower2Pool = new List<GameObject>();
    private List<GameObject> iceTower3Pool = new List<GameObject>();
    #endregion

    #region Unity Functions
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }

    void Start()
    {
        basicTowerPool = createTowerPool(basicTowerPrefab, basicTowerPoolSize);
        bombTowerPool = createTowerPool(bombTowerPrefab, bombTowerPoolSize);
        slowTowerPool = createTowerPool(slowTowerPrefab, slowTowerPoolSize);
        arrowTower1Pool = createTowerPool(arrowTower1Prefab, arrowTowerPoolSize);
        arrowTower2Pool = createTowerPool(arrowTower2Prefab, arrowTowerPoolSize);
        arrowTower3Pool = createTowerPool(arrowTower3Prefab, arrowTowerPoolSize);
        fireTower1Pool = createTowerPool(fireTower1Prefab, fireTowerPoolSize);
        fireTower2Pool = createTowerPool(fireTower2Prefab, fireTowerPoolSize);
        fireTower3Pool = createTowerPool(fireTower3Prefab, fireTowerPoolSize);
        iceTower1Pool = createTowerPool(iceTower1Prefab, iceTowerPoolSize);
        iceTower2Pool = createTowerPool(iceTower2Prefab, iceTowerPoolSize);
        iceTower3Pool = createTowerPool(iceTower3Prefab, iceTowerPoolSize);
    }

    void OnEnable()
    {
        EventBus.TowerEvents.TowerFinished += spawnTower;
    }

    void OnDisable()
    {
        EventBus.TowerEvents.TowerFinished -= spawnTower;
    }
    #endregion

    #region Private Functions
    private void spawnTower(TowerBlueprint towerBlueprint)
    {
        Transform nodeTransform = towerBlueprint.node.transform;
        Vector3 position = nodeTransform.position;
        Quaternion rotation = nodeTransform.rotation;

        StatsManager.Instance.TrackCreateTower(towerBlueprint);

        List<GameObject> towerPool = getTowerPool(towerBlueprint.type);

        foreach (GameObject tower in towerPool)
        {
            bool isTowerNotAvailable = tower.activeSelf;
            if (isTowerNotAvailable) continue;

            tower.GetComponent<BaseTower>().TowerBlueprint = towerBlueprint;
            tower.transform.position = position;
            tower.transform.rotation = rotation;
            tower.SetActive(true);

            BaseNode node = nodeTransform.gameObject.GetComponent<BaseNode>();
            node.Renderer.material = node.OccupiedMaterial;
            return;
        }
    }

    private List<GameObject> createTowerPool(GameObject towerPrefab, int poolSize)
    {
        towerPrefab.SetActive(false);

        List<GameObject> towerPool = new List<GameObject>();
        for (int i = 0; i < poolSize; i++)
        {
            GameObject towerObj = Instantiate(towerPrefab, Vector3.zero, Quaternion.identity, transform);
            towerPool.Add(towerObj);
        }

        return towerPool;
    }

    private List<GameObject> getTowerPool(TowerType type)
    {
        List<GameObject> towerPool = null;
        switch (type)
        {
            case TowerType.BasicTower:
                towerPool = basicTowerPool;
                break;
            case TowerType.BombTower:
                towerPool = bombTowerPool;
                break;
            case TowerType.SlowTower:
                towerPool = slowTowerPool;
                break;
            case TowerType.ArrowTower1:
                towerPool = arrowTower1Pool;
                break;
            case TowerType.ArrowTower2:
                towerPool = arrowTower2Pool;
                break;
            case TowerType.ArrowTower3:
                towerPool = arrowTower3Pool;
                break;
            case TowerType.FireTower1:
                towerPool = fireTower1Pool;
                break;
            case TowerType.FireTower2:
                towerPool = fireTower2Pool;
                break;
            case TowerType.FireTower3:
                towerPool = fireTower3Pool;
                break;
            case TowerType.IceTower1:
                towerPool = iceTower1Pool;
                break;
            case TowerType.IceTower2:
                towerPool = iceTower2Pool;
                break;
            case TowerType.IceTower3:
                towerPool = iceTower3Pool;
                break;
        }
        return towerPool;
    }
    #endregion
}