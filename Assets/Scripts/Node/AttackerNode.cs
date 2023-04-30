using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;

public class AttackerNode : BaseNode
{
    [SerializeField] private List<EnemyBlueprint> spawnedEnemyBlueprints = new List<EnemyBlueprint>();
    [SerializeField] private int maxEnemiesPerNode = 8;

    #region Unity Functions
    void OnEnable()
    {
        EventBus.PhaseEvents.SurvivalPhaseStarted += StartSpawnEnemies;
    }

    void OnDisable()
    {
        EventBus.PhaseEvents.SurvivalPhaseStarted -= StartSpawnEnemies;
    }
    #endregion

    #region Private Functions
    protected override void UpdateNodeUI()
    {
        towerInventory.gameObject.SetActive(false);
        enemyInventory.gameObject.SetActive(true);
        upgradeInventory.gameObject.SetActive(false);
        enemyInventory.UpdateOptionsBasedOnMoney();
    }

    private void StartSpawnEnemies()
    {
        StartCoroutine(SpawnEnemies());
    }

    private IEnumerator SpawnEnemies()
    {
        if (spawnedEnemyBlueprints.Count == 0) yield break;

        occupied = true;
        List<EnemyBlueprint> tempList = new List<EnemyBlueprint>(spawnedEnemyBlueprints);
        foreach (EnemyBlueprint enemyBlueprint in tempList)
        {
            BuildSelectedEnemy(enemyBlueprint);
            //EventBus.EnemyEvents.EnemyFinished(enemyBlueprint);
            yield return new WaitForSeconds(3f);
        }
    }
    #endregion

    #region Public Functions
    public void AddSpawnedEnemy(EnemyBlueprint enemyBlueprint)
    {
        spawnedEnemyBlueprints.Add(enemyBlueprint);
    }

    public void BuildSelectedEnemy(EnemyBlueprint enemyBlueprint)
    {
        if (this.occupied != true) this.occupied = true;

        StatsManager.Instance.Money -= enemyBlueprint.cost;
        enemyBlueprint.node = this;
        ProgressBarManager.Instance.GetAndShowProgressBar(enemyBlueprint);
        this.BuildEffect.SetActive(true);
        EventBus.EnemyEvents.EnemyStarted();
        NodeManager.Instance.SelectedNode = null;
    }
    #endregion
}