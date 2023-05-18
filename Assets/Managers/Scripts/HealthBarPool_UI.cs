using UnityEngine;
using System.Collections.Generic;

public class HealthBarPool_UI : MonoBehaviour
{
    #region Public Variables
    public static HealthBarPool_UI Instance = null;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject healthbarPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int healthbarPoolSize = 15;
    #endregion

    #region Private Variables
    private List<HealthBar_UI> healthbarPool = new List<HealthBar_UI>();
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
        createHealthbarPool(healthbarPrefab, healthbarPoolSize);
    }
    #endregion

    #region Public Functions
    public HealthBar_UI GetHealthbar(Transform healthbarOffset)
    {
        foreach (HealthBar_UI healthbar in healthbarPool)
        {
            bool isHealthbarNotAvailable = healthbar.gameObject.activeInHierarchy;
            if (isHealthbarNotAvailable) continue;

            healthbar.transform.SetParent(healthbarOffset, true);
            healthbar.transform.localPosition = Vector3.zero;
            healthbar.gameObject.SetActive(true);

            return healthbar;
        }

        // Create a new healthbar if none are available
        HealthBar_UI newHealthbar = createHealthbar(healthbarPrefab);
        newHealthbar.transform.SetParent(healthbarOffset, true);
        newHealthbar.transform.localPosition = Vector3.zero;
        newHealthbar.gameObject.SetActive(true);
        healthbarPool.Add(newHealthbar);

        return newHealthbar;
    }
    #endregion

    #region Private Functions
    private HealthBar_UI createHealthbar(GameObject healthbarPrefab)
    {
        healthbarPrefab.SetActive(false);
        GameObject healthbarObj = Instantiate(healthbarPrefab, Vector3.zero, Quaternion.identity, transform);
        HealthBar_UI healthbar = healthbarObj.GetComponent<HealthBar_UI>();
        return healthbar;
    }


    private void createHealthbarPool(GameObject healthbarPrefab, int maxPoolSize)
    {
        healthbarPrefab.SetActive(false);

        for (int i=0; i<maxPoolSize; i++)
        {
            GameObject healthbarObj = Instantiate(healthbarPrefab, Vector3.zero, Quaternion.identity, transform);
            HealthBar_UI healthbar = healthbarObj.GetComponent<HealthBar_UI>();
            healthbarPool.Add(healthbar);
        }

        //EventBus.PoolEvents.HealthBarPoolReady();
    }
    #endregion
}
