using System.Collections;
using UnityEngine;
using Gotchi.Events;

public class AttackerNode : BaseNode
{
    private EnemyBlueprint spawnedEnemyBlueprint = null;

    #region Unity Functions
    void OnEnable()
    {
        EventBus.PhaseEvents.PrepPhaseStarted += SpawnEnemy;
    }

    void OnDisable()
    {
        EventBus.PhaseEvents.PrepPhaseStarted -= SpawnEnemy;
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

    private void SpawnEnemy()
    {
        if (spawnedEnemyBlueprint == null || spawnedEnemyBlueprint.type == EnemyManager.EnemyType.None) return;
        occupied = true;
        EventBus.EnemyEvents.EnemyFinished(spawnedEnemyBlueprint);
    }

    public void SetSpawnedEnemy(EnemyBlueprint enemyBlueprint)
    {
        spawnedEnemyBlueprint = enemyBlueprint;
    }
    #endregion
}
