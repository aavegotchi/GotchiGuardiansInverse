using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using Gotchi.Events;

namespace Gotchi.New
{
    public class LickquidatorManager : MonoBehaviour
    {
        #region Public Variables
        public static LickquidatorManager Instance = null;

        public enum LickquidatorType
        {
            None,
            PawnLickquidator,
            AerialLickquidator,
            BossLickquidator
        }

        public List<LickquidatorPresenter> ActiveLickquidators
        {
            get 
            { 
                return spawnedLickquidators
                    .Where(lickquidatorObj => lickquidatorObj.activeSelf)
                    .Select(lickquidatorObj => lickquidatorObj.GetComponent<LickquidatorPresenter>())
                    .ToList(); 
            }
        }
        #endregion

        #region Fields
        [Header("Required Refs")]
        [SerializeField] private GameObject pawnLickquidatorPrefab = null;
        [SerializeField] private GameObject aerialLickquidatorPrefab = null;
        [SerializeField] private GameObject bossLickquidatorPrefab = null;

        [Header("Attributes")]
        [SerializeField] private int pawnLickquidatorPoolSize = 5;
        [SerializeField] private int aerialLickquidatorPoolSize = 5;
        [SerializeField] private int bossLickquidatorPoolSize = 5;
        #endregion

        #region Private Variables
        private List<GameObject> combinedPool = new List<GameObject>();
        private List<GameObject> pawnLickquidatorPool = new List<GameObject>();
        private List<GameObject> aerialLickquidatorPool = new List<GameObject>();
        private List<GameObject> bossLickquidatorPool = new List<GameObject>();
        private List<GameObject> spawnedLickquidators = new List<GameObject>();
        private Dictionary<GameObject, LickquidatorPresenter> lickquidatorLookup = new Dictionary<GameObject, LickquidatorPresenter>();
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
            pawnLickquidatorPool = createPool(pawnLickquidatorPrefab, pawnLickquidatorPoolSize);
            aerialLickquidatorPool = createPool(aerialLickquidatorPrefab, aerialLickquidatorPoolSize);
            bossLickquidatorPool = createPool(bossLickquidatorPrefab, bossLickquidatorPoolSize);

            combinedPool.AddRange(pawnLickquidatorPool);
            combinedPool.AddRange(aerialLickquidatorPool);
            combinedPool.AddRange(bossLickquidatorPool);

            initializeLookup();
        }

        void OnEnable()
        {
            EventBus.PhaseEvents.PrepPhaseStarted += freezeLickquidators;
            EventBus.PhaseEvents.SurvivalPhaseStarted += unfreezeLickquidators;
            EventBus.EnemyEvents.EnemyFinished += spawnLickquidator;
        }

        void OnDisable()
        {
            EventBus.PhaseEvents.PrepPhaseStarted -= freezeLickquidators;
            EventBus.PhaseEvents.SurvivalPhaseStarted -= unfreezeLickquidators;
            EventBus.EnemyEvents.EnemyFinished -= spawnLickquidator;
        }
        #endregion

        #region Public Functions
        public LickquidatorPresenter GetByObject(GameObject lickquidatorObj)
        {
            lickquidatorLookup.TryGetValue(lickquidatorObj, out LickquidatorPresenter lickquidator);
            return lickquidator;
        }
        #endregion

        #region Private Functions
        private GameObject instantiate(GameObject prefab)
        {
            prefab.SetActive(false);
            GameObject lickquidatorObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
            return lickquidatorObj;
        }

        private void spawnLickquidator(EnemyBlueprint enemyBlueprint)
        {
            Transform nodeTransform = enemyBlueprint.node.transform;
            Vector3 position = nodeTransform.position;
            Quaternion rotation = nodeTransform.rotation * Quaternion.Euler(Vector3.up * -90f); // face toward movement dir;

            StatsManager.Instance.Money -= enemyBlueprint.cost;
            StatsManager.Instance.TrackCreateEnemy(enemyBlueprint);

            List<GameObject> pool = getPool((LickquidatorType)enemyBlueprint.type);

            foreach (GameObject lickquidatorObj in pool)
            {
                if (lickquidatorObj.activeSelf) continue;

                LickquidatorModel lickquidatorModel = lickquidatorObj.GetComponent<LickquidatorModel>();
                lickquidatorModel.EnemyBlueprint = enemyBlueprint;
                lickquidatorObj.transform.position = position;
                lickquidatorObj.transform.rotation = rotation;
                lickquidatorObj.SetActive(true);

                LickquidatorPresenter lickquidator = GetByObject(lickquidatorObj);

                lickquidator.AssignHealthBar(HealthBarPool_UI.Instance.GetHealthbar(lickquidatorModel.HealthBarOffset));
                lickquidator.Freeze(); // to prevent prep phase 'pushing'

                spawnedLickquidators.Add(lickquidatorObj);

                return;
            }
        }

        private void initializeLookup()
        {
            foreach (GameObject lickquidatorObj in combinedPool)
            {
                LickquidatorPresenter lickquidator = lickquidatorObj.GetComponent<LickquidatorPresenter>();
                lickquidatorLookup.Add(lickquidatorObj, lickquidator);
            }
        }

        private List<GameObject> createPool(GameObject prefab, int poolSize)
        {
            prefab.SetActive(false);

            List<GameObject> pool = new List<GameObject>();
            for (int i = 0; i < poolSize; i++)
            {
                GameObject lickquidatorObj = Instantiate(prefab, Vector3.zero, Quaternion.identity, transform);
                pool.Add(lickquidatorObj);
            }

            return pool;
        }

        private List<GameObject> getPool(LickquidatorType type)
        {
            List<GameObject> pool = null;
            switch (type)
            {
                case LickquidatorType.PawnLickquidator:
                    pool = pawnLickquidatorPool;
                    break;
                case LickquidatorType.AerialLickquidator:
                    pool = aerialLickquidatorPool;
                    break;
                case LickquidatorType.BossLickquidator:
                    pool = bossLickquidatorPool;
                    break;
            }

            if (pool.All(lickquidator => lickquidator.activeSelf))
            {
                GameObject prefab;
                switch (type)
                {
                    case LickquidatorType.PawnLickquidator:
                        prefab = pawnLickquidatorPrefab;
                        break;
                    case LickquidatorType.AerialLickquidator:
                        prefab = aerialLickquidatorPrefab;
                        break;
                    case LickquidatorType.BossLickquidator:
                        prefab = bossLickquidatorPrefab;
                        break;
                    default:
                        return pool;
                }

                GameObject lickquidatorObj = instantiate(prefab);
                pool.Add(lickquidatorObj);
                combinedPool.Add(lickquidatorObj);
                lickquidatorLookup.Add(lickquidatorObj, lickquidatorObj.GetComponent<LickquidatorPresenter>());
            }

            return pool;
        }

        private void unfreezeLickquidators()
        {
            foreach (LickquidatorPresenter lickquidator in ActiveLickquidators)
            {
                lickquidator.UnFreeze();
            }
        }

        private void freezeLickquidators()
        {
            foreach (LickquidatorPresenter lickquidator in ActiveLickquidators)
            {
                lickquidator.Freeze();
            }
        }
        #endregion
    }
}