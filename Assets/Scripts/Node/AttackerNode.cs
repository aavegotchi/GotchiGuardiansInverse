using System.Collections;
using UnityEngine;
using Gotchi.Events;

public class AttackerNode : BaseNode
{
    private EnemyBlueprint spawnedEnemyBlueprint = null;

    #region Unity Functions
    void OnEnable()
    {
        EventBus.PhaseEvents.PrepPhaseStarted += delaySpawnEnemy;
    }

    void OnDisable()
    {
        EventBus.PhaseEvents.PrepPhaseStarted -= delaySpawnEnemy;
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

    private void delaySpawnEnemy()
    {
        if (spawnedEnemyBlueprint == null || spawnedEnemyBlueprint.type == EnemyManager.EnemyType.None) return;
        
        StartCoroutine(delaySpawn());
    }

    private IEnumerator delaySpawn()
    {
        occupied = true;
        yield return new WaitForSeconds(2f);
        EventBus.EnemyEvents.EnemyFinished(spawnedEnemyBlueprint);
    }

    public void SetSpawnedEnemy(EnemyBlueprint enemyBlueprint)
    {
        spawnedEnemyBlueprint = enemyBlueprint;
    }
    #endregion
}
