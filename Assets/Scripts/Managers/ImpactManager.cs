using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ImpactManager : MonoBehaviour
{
    #region Public Variables
    public static ImpactManager Instance = null;

    public enum ImpactType
    {
        Cannonball,
        Missile,
        Arrow,
        Fireball,
        Iceball,
        BasicTower,
        BombTower,
        SlowTower,
        Sword,
        Spin,
        Laser
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject cannonballImpactPrefab = null;
    [SerializeField] private GameObject missileImpactPrefab = null;
    [SerializeField] private GameObject arrowImpactPrefab = null;
    [SerializeField] private GameObject fireballImpactPrefab = null;
    [SerializeField] private GameObject iceballImpactPrefab = null;
    [SerializeField] private GameObject basicTowerExplosionPrefab = null;
    [SerializeField] private GameObject bombTowerExplosionPrefab = null;
    [SerializeField] private GameObject slowTowerExplosionPrefab = null;
    [SerializeField] private GameObject laserImpactPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int cannonballImpactPoolSize = 30;
    [SerializeField] private int missileImpactPoolSize = 20;
    [SerializeField] private int arrowImpactPoolSize = 20;
    [SerializeField] private int fireballImpactPoolSize = 20;
    [SerializeField] private int iceballImpactPoolSize = 20;
    [SerializeField] private int basicTowerExplosionPoolSize = 10;
    [SerializeField] private int bombTowerExplosionPoolSize = 10;
    [SerializeField] private int slowTowerExplosionPoolSize = 10;
    [SerializeField] private int laserImpactPoolSize = 10;
    #endregion

    #region Private Variables
    private List<GameObject> cannonballImpactPool = new List<GameObject>();
    private List<GameObject> missileImpactPool = new List<GameObject>();
    private List<GameObject> arrowImpactPool = new List<GameObject>();
    private List<GameObject> fireballImpactPool = new List<GameObject>();
    private List<GameObject> iceballImpactPool = new List<GameObject>();
    private List<GameObject> basicTowerExplosionPool = new List<GameObject>();
    private List<GameObject> bombTowerExplosionPool = new List<GameObject>();
    private List<GameObject> slowTowerExplosionPool = new List<GameObject>();
    private List<GameObject> laserImpactPool = new List<GameObject>();
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
        cannonballImpactPool = createImpactPool(cannonballImpactPrefab, cannonballImpactPoolSize);
        missileImpactPool = createImpactPool(missileImpactPrefab, missileImpactPoolSize);
        arrowImpactPool = createImpactPool(arrowImpactPrefab, arrowImpactPoolSize);
        fireballImpactPool = createImpactPool(fireballImpactPrefab, fireballImpactPoolSize);
        iceballImpactPool = createImpactPool(iceballImpactPrefab, iceballImpactPoolSize);
        basicTowerExplosionPool = createImpactPool(basicTowerExplosionPrefab, basicTowerExplosionPoolSize);
        bombTowerExplosionPool = createImpactPool(bombTowerExplosionPrefab, bombTowerExplosionPoolSize);
        slowTowerExplosionPool = createImpactPool(slowTowerExplosionPrefab, slowTowerExplosionPoolSize);
        laserImpactPool = createImpactPool(laserImpactPrefab, laserImpactPoolSize);
    }
    #endregion

    #region Public Functions
    public GameObject SpawnImpact(ImpactType type, Vector3 position, Quaternion rotation)
    {
        List<GameObject> impactPool = getImpactPool(type);

        foreach (GameObject impact in impactPool)
        {
            bool isImpactNotAvailable = impact.gameObject.activeSelf;
            if (isImpactNotAvailable) continue;
            
            impact.transform.position = position;
            impact.transform.rotation = rotation;
            impact.SetActive(true);

            return impact;
        }

        return null;
    }
    #endregion

    #region Private Functions
    private List<GameObject> createImpactPool(GameObject impactPrefab, int poolSize)
    {
        impactPrefab.SetActive(false);

        List<GameObject> impactPool = new List<GameObject>();
        for (int i=0; i<poolSize; i++)
        {
            GameObject impactObj = Instantiate(impactPrefab, Vector3.zero, Quaternion.identity, transform);
            impactPool.Add(impactObj);
        }

        return impactPool;
    }

    private List<GameObject> getImpactPool(ImpactType type)
    {
        List<GameObject> impactPool = null;
        switch (type)
        {
            case ImpactType.Cannonball:
                impactPool = cannonballImpactPool;
                break;
            case ImpactType.Missile:
                impactPool = missileImpactPool;
                break;
            case ImpactType.Arrow:
                impactPool = arrowImpactPool;
                break;
            case ImpactType.Fireball:
                impactPool = fireballImpactPool;
                break;
            case ImpactType.Iceball:
                impactPool = iceballImpactPool;
                break;
            case ImpactType.BasicTower:
                impactPool = basicTowerExplosionPool;
                break;
            case ImpactType.BombTower:
                impactPool = bombTowerExplosionPool;
                break;
            case ImpactType.SlowTower:
                impactPool = slowTowerExplosionPool;
                break;
            case ImpactType.Laser:
                impactPool = laserImpactPool;
                break;
        }
        return impactPool;
    }
    #endregion
}