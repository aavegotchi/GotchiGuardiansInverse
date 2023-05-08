using System;
using UnityEngine;
using Gotchi.Events;
using Fusion;

public class DefenderNode : BaseNode
{
    #region Overriden Functions
    protected override void UpdateNodeUI()
    {
        towerInventory.gameObject.SetActive(true);
        enemyInventory.gameObject.SetActive(false);
        upgradeInventory.gameObject.SetActive(false);
        towerInventory.UpdateOptionsBasedOnMoney();
    }
    #endregion

    #region RPC Functions
    [Rpc(RpcSources.All, RpcTargets.All)]
    private void rpc_buildTower(string towerTypeStr, int cost, float buildTime)
    {
        TowerBlueprint towerBlueprint = new TowerBlueprint();
        towerBlueprint.type = (TowerPool.TowerType)Enum.Parse(typeof(TowerPool.TowerType), towerTypeStr);
        towerBlueprint.cost = cost;
        towerBlueprint.buildTime = buildTime;

        this.Occupied = true;
        towerBlueprint.node = this;
        EventBus.TowerEvents.TowerStarted();
        BuildProgressPool_UI.Instance.GetAndShowProgressBar(towerBlueprint);
        this.BuildEffect.SetActive(true);
    }
    #endregion

    #region Public Functions
    public void BuildTower(TowerBlueprint towerBlueprint)
    {
        rpc_buildTower(towerBlueprint.type.ToString(), towerBlueprint.cost, towerBlueprint.buildTime);   

        StatsManager.Instance.Money -= towerBlueprint.cost;
    }
    #endregion
}