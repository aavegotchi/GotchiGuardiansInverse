using UnityEngine;
using UnityEngine.UI;
using System.Collections.Generic;
using Gotchi.Events;

public class HealthBarManager : MonoBehaviour
{
    #region Public Variables
    public static HealthBarManager Instance = null;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject healthbarPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int healthbarPoolSize = 50;
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
        
        return null;
    }
    #endregion

    #region Private Functions
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
