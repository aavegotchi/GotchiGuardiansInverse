using UnityEngine;
using System.Collections.Generic;
using UnityEngine.Assertions;
using GameMaster;

public class BuildProgressPool_UI : MonoBehaviour
{
    #region Public Variables
    public static BuildProgressPool_UI Instance = null;
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject progressBarPrefab = null;

    [Header("Attributes")]
    [SerializeField] private int progressBarPoolSize = 50;
    #endregion

    #region Private Variables
    private List<ProgressBar_UI> progressBarPool = new List<ProgressBar_UI>();
    #endregion

    #region Unity Functions
    void Awake()
    {
        Assert.IsNotNull(progressBarPrefab);

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
        CreateProgressBarPool(progressBarPrefab, progressBarPoolSize);
    }
    #endregion

    #region Public Functions
    public void GetAndShowProgressBar(TowerBlueprint towerBlueprint)
    {
        Transform nodeTransform = towerBlueprint.node.transform;
        ProgressBar_UI progressBar = getProgressBar(nodeTransform, towerBlueprint.buildTime);
        
        progressBar.ShowProgressBarAndSetDuration(towerBlueprint, (TowerBlueprint blueprint) =>
        {
            GameMasterEvents.TowerEvents.TowerFinished(blueprint);
            progressBar.Reset();
        });
    }

    public void GetAndShowProgressBar(EnemyBlueprint enemyBlueprint, bool skipSpawn = false)
    {
        Transform nodeTransform = enemyBlueprint.node.transform;
        ProgressBar_UI progressBar = getProgressBar(nodeTransform, enemyBlueprint.buildTime);
        
        progressBar.ShowProgressBarAndSetDuration(enemyBlueprint, (EnemyBlueprint blueprint) =>
        {
            if (!skipSpawn)
            {
                GameMasterEvents.EnemyEvents.EnemyFinished(blueprint);
            }

            progressBar.Reset();
        });
    }
    #endregion

    #region Private Functions
    private ProgressBar_UI CreateNewProgressBar()
    {
        GameObject progressBarObj = Instantiate(progressBarPrefab, Vector3.zero, Quaternion.identity, transform);
        ProgressBar_UI progressBar = progressBarObj.GetComponent<ProgressBar_UI>();
        return progressBar;
    }

    private ProgressBar_UI getProgressBar(Transform nodeTransform, float duration)
    {
        foreach (ProgressBar_UI progressBar in progressBarPool)
        {
            bool isProgressBarNotAvailable = progressBar.gameObject.activeInHierarchy;
            if (isProgressBarNotAvailable) continue;

            progressBar.transform.SetParent(nodeTransform, true);
            progressBar.transform.localPosition = new Vector3(0f, 25f, 0f);
            progressBar.gameObject.SetActive(true);
            return progressBar;
        }

        // If the code reaches this point, no available progress bars were found in the pool.
        // Instantiate a new progress bar and add it to the pool.
        ProgressBar_UI newProgressBar = CreateNewProgressBar();
        progressBarPool.Add(newProgressBar);
        newProgressBar.transform.SetParent(nodeTransform, true);
        newProgressBar.transform.localPosition = new Vector3(0f, 25f, 0f);
        newProgressBar.gameObject.SetActive(true);
        return newProgressBar;
    }

    private void CreateProgressBarPool(GameObject progressBarPrefab, int maxPoolSize)
    {
        progressBarPrefab.SetActive(false);

        for (int i = 0; i < maxPoolSize; i++)
        {
            GameObject progressBarObj = Instantiate(progressBarPrefab, Vector3.zero, Quaternion.identity, transform);
            ProgressBar_UI progressBar = progressBarObj.GetComponent<ProgressBar_UI>();
            progressBarPool.Add(progressBar);
        }
    }
    #endregion
}
