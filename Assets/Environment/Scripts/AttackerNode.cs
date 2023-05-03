using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;

public class AttackerNode : BaseNode
{
    #region Public Variables
    public List<EnemyBlueprint> SpawnedEnemyBlueprints
    {
        get { return spawnedEnemyBlueprints; }
    }

    public int MaxEnemiesPerNode
    {
        get { return maxEnemiesPerNode; }
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private EnemySlots_UI enemySlotsUI = null;

    [Header("Attributes")]
    [SerializeField] private int maxEnemiesPerNode = 8;
    #endregion

    #region Private Variables
    private List<EnemyBlueprint> spawnedEnemyBlueprints = new List<EnemyBlueprint>();
    #endregion

    #region Unity Functions
    void OnEnable()
    {
        EventBus.PhaseEvents.SurvivalPhaseStarted += startSpawnEnemies;
    }

    void OnDisable()
    {
        EventBus.PhaseEvents.SurvivalPhaseStarted -= startSpawnEnemies;
    }
    #endregion

    #region Overriden Functions
    protected override void UpdateNodeUI()
    {
        towerInventory.gameObject.SetActive(false);
        enemyInventory.gameObject.SetActive(true);
        upgradeInventory.gameObject.SetActive(false);
        enemyInventory.UpdateOptionsBasedOnMoney();
        if (spawnedEnemyBlueprints.Count >= maxEnemiesPerNode)
        {
            enemyInventory.DisableOptions();
        }
    }
    #endregion

    #region Public Functions
    public void AddSpawnedEnemy(EnemyBlueprint enemyBlueprint)
    {
        this.Occupied = true;
        enemyBlueprint.node = this;
        StatsManager.Instance.Money -= enemyBlueprint.cost;
        EventBus.EnemyEvents.EnemyStarted();
        spawnedEnemyBlueprints.Add(enemyBlueprint);
        ProgressBarManager.Instance.GetAndShowProgressBar(enemyBlueprint, true);
        this.BuildEffect.SetActive(true);
        enemySlotsUI.OccupyNextSlot(maxEnemiesPerNode);
        NodeManager.Instance.SelectedNode = null;
    }
    #endregion

    #region Private Functions
    private void startSpawnEnemies()
    {
        if (spawnedEnemyBlueprints.Count == 0) return;
        StartCoroutine(spawnEnemies());
    }

    private IEnumerator spawnEnemies()
    {
        foreach (EnemyBlueprint enemyBlueprint in spawnedEnemyBlueprints)
        {
            EventBus.EnemyEvents.EnemyFinished(enemyBlueprint);
            yield return new WaitForSeconds(enemyBlueprint.buildTime * 0.5f); // for now, spawn enemies at half the time it takes to build them to avoid clumping
        }
    }

    // private void removeSpawnedEnemy(int index)
    // {
    //     if (index >= 0 && index < spawnedEnemyBlueprints.Count)
    //     {
    //         spawnedEnemyBlueprints.RemoveAt(index);
    //         enemySlotsUI.DeactivateLastSlot();
    //     }
    //     else
    //     {
    //         Debug.LogError("Invalid index provided for RemoveSpawnedEnemy");
    //     }
    // }
    #endregion
}