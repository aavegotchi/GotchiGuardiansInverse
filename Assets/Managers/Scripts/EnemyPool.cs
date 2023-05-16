// using System.Collections;
// using System.Collections.Generic;
// using System.Linq;
// using UnityEngine;
// using Gotchi.Events;

// public class EnemyPool : MonoBehaviour
// {
//     #region Public Variables
//     public static EnemyPool Instance = null;

//     public enum EnemyType
//     {
//         None,
//         PawnLickquidator,
//         AerialLickquidator,
//         BossLickquidator
//     }

//     public List<GameObject> ActiveEnemies
//     {
//         get { return spawnedEnemies.Where(enemy => enemy.activeSelf).ToList(); }
//     }
//     #endregion

//     #region Fields
//     [Header("Required Refs")]
//     [SerializeField] private GameObject pawnLickquidatorPrefab = null;
//     [SerializeField] private GameObject aerialLickquidatorPrefab = null;
//     [SerializeField] private GameObject bossLickquidatorPrefab = null;

//     [Header("Attributes")]
//     [SerializeField] private int pawnLickquidatorPoolSize = 5;
//     [SerializeField] private int aerialLickquidatorPoolSize = 5;
//     [SerializeField] private int bossLickquidatorPoolSize = 5;
//     #endregion

//     #region Private Variables
//     private List<GameObject> combinedEnemyPool = new List<GameObject>();
//     private List<GameObject> pawnLickquidatorPool = new List<GameObject>();
//     private List<GameObject> aerialLickquidatorPool = new List<GameObject>();
//     private List<GameObject> bossLickquidatorPool = new List<GameObject>();
//     private List<GameObject> spawnedEnemies = new List<GameObject>();
//     private Dictionary<GameObject, Enemy> enemyLookup = new Dictionary<GameObject, Enemy>();
//     #endregion

//     #region Unity Functions
//     void Awake()
//     {
//         if (Instance == null)
//         {
//             Instance = this;
//         }
//         else
//         {
//             Destroy(gameObject);
//         }
//     }

//     void Start()
//     {
//         pawnLickquidatorPool = createEnemyPool(pawnLickquidatorPrefab, pawnLickquidatorPoolSize);
//         aerialLickquidatorPool = createEnemyPool(aerialLickquidatorPrefab, aerialLickquidatorPoolSize);
//         bossLickquidatorPool = createEnemyPool(bossLickquidatorPrefab, bossLickquidatorPoolSize);

//         combinedEnemyPool.AddRange(pawnLickquidatorPool);
//         combinedEnemyPool.AddRange(aerialLickquidatorPool);
//         combinedEnemyPool.AddRange(bossLickquidatorPool);

//         initializeEnemyLookup();
//     }

//     void OnEnable()
//     {
//         EventBus.PhaseEvents.PrepPhaseStarted += freezeEnemies;
//         EventBus.PhaseEvents.SurvivalPhaseStarted += unfreezeEnemies;
//         EventBus.EnemyEvents.EnemyFinished += spawnEnemy;
//     }

//     void OnDisable()
//     {
//         EventBus.PhaseEvents.PrepPhaseStarted -= freezeEnemies;
//         EventBus.PhaseEvents.SurvivalPhaseStarted -= unfreezeEnemies;
//         EventBus.EnemyEvents.EnemyFinished -= spawnEnemy;
//     }
//     #endregion

//     #region Public Functions
//     public Enemy GetEnemyByObject(GameObject enemyObj)
//     {
//         enemyLookup.TryGetValue(enemyObj, out Enemy enemyComponent);
//         return enemyComponent;
//     }

//     public Enemy[] GetEnemiesByObjects(GameObject[] enemyObjs)
//     {
//         List<Enemy> matchingEnemies = new List<Enemy>();

//         foreach (GameObject enemyObj in enemyObjs)
//         {
//             if (enemyLookup.TryGetValue(enemyObj, out Enemy enemyComponent))
//             {
//                 matchingEnemies.Add(enemyComponent);
//             }
//         }

//         return matchingEnemies.ToArray();
//     }
//     #endregion

//     #region Private Functions
//     private GameObject createEnemy(GameObject enemyPrefab)
//     {
//         enemyPrefab.SetActive(false);
//         GameObject enemyObj = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, transform);
//         return enemyObj;
//     }

//     private void spawnEnemy(EnemyBlueprint enemyBlueprint)
//     {
//         Transform nodeTransform = enemyBlueprint.node.transform;
//         Vector3 position = nodeTransform.position;
//         Quaternion rotation = nodeTransform.rotation * Quaternion.Euler(Vector3.up * -90f); // face toward movement dir;

//         StatsManager.Instance.Money -= enemyBlueprint.cost;
//         StatsManager.Instance.TrackCreateEnemy(enemyBlueprint);

//         List<GameObject> enemyPool = new List<GameObject>();
//         enemyPool = getEnemyPool(enemyBlueprint.type);

//         foreach (GameObject enemy in enemyPool)
//         {
//             bool isEnemyNotAvailable = enemy.activeSelf;
//             if (isEnemyNotAvailable) continue;

//             enemy.GetComponent<Enemy>().EnemyBlueprint = enemyBlueprint;
//             enemy.transform.position = position;
//             enemy.transform.rotation = rotation;
//             enemy.gameObject.SetActive(true);

//             Enemy enemyScript = GetEnemyByObject(enemy);
//             enemyScript.SetHealthbarAndResetHealth(HealthBarPool_UI.Instance.GetHealthbar(enemyScript.HealthbarOffset));
//             enemyScript.Freeze(); // to prevent prep phase 'pushing'
            
//             spawnedEnemies.Add(enemy);
            
//             return;
//         }
//     }

//     private void initializeEnemyLookup()
//     {
//         foreach (GameObject enemyObj in combinedEnemyPool)
//         {
//             Enemy enemyComponent = enemyObj.GetComponent<Enemy>();
//             enemyLookup.Add(enemyObj, enemyComponent);
//         }
//     }

//     private List<GameObject> createEnemyPool(GameObject enemyPrefab, int poolSize)
//     {
//         enemyPrefab.SetActive(false);

//         List<GameObject> enemyPool = new List<GameObject>();
//         for (int i = 0; i < poolSize; i++)
//         {
//             GameObject enemyObj = Instantiate(enemyPrefab, Vector3.zero, Quaternion.identity, transform);
//             enemyPool.Add(enemyObj);
//         }

//         return enemyPool;
//     }

//     private List<GameObject> getEnemyPool(EnemyType type)
//     {
//         List<GameObject> enemyPool = null;
//         switch (type)
//         {
//             case EnemyType.PawnLickquidator:
//                 enemyPool = pawnLickquidatorPool;
//                 break;
//             case EnemyType.AerialLickquidator:
//                 enemyPool = aerialLickquidatorPool;
//                 break;
//             case EnemyType.BossLickquidator:
//                 enemyPool = bossLickquidatorPool;
//                 break;
//         }

//         if (enemyPool.All(enemy => enemy.activeSelf))
//         {
//             GameObject enemyPrefab;
//             switch (type)
//             {
//                 case EnemyType.PawnLickquidator:
//                     enemyPrefab = pawnLickquidatorPrefab;
//                     break;
//                 case EnemyType.AerialLickquidator:
//                     enemyPrefab = aerialLickquidatorPrefab;
//                     break;
//                 case EnemyType.BossLickquidator:
//                     enemyPrefab = bossLickquidatorPrefab;
//                     break;
//                 default:
//                     return enemyPool;
//             }

//             GameObject newEnemy = createEnemy(enemyPrefab);
//             enemyPool.Add(newEnemy);
//             combinedEnemyPool.Add(newEnemy);
//             enemyLookup.Add(newEnemy, newEnemy.GetComponent<Enemy>());
//         }

//         return enemyPool;
//     }

//     private List<Enemy> getActiveEnemyScripts()
//     {
//         List<Enemy> activeEnemyScripts = new List<Enemy>();

//         foreach (GameObject enemyObj in ActiveEnemies)
//         {
//             Enemy enemyScript = GetEnemyByObject(enemyObj);
//             if (enemyScript != null)
//             {
//                 activeEnemyScripts.Add(enemyScript);
//             }
//         }

//         return activeEnemyScripts;
//     }

//     private void unfreezeEnemies()
//     {
//         List<Enemy> activeEnemies = getActiveEnemyScripts();

//         foreach (Enemy enemy in activeEnemies)
//         {
//             enemy.Unfreeze();
//         }
//     }

//     private void freezeEnemies()
//     {
//         List<Enemy> activeEnemies = getActiveEnemyScripts();

//         foreach (Enemy enemy in activeEnemies)
//         {
//             enemy.Freeze();
//         }
//     }
//     #endregion
// }
