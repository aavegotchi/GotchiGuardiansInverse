using UnityEngine;

public class DefenderNode : BaseNode
{
    #region Private Functions
    protected override void UpdateNodeUI()
    {
        towerInventory.gameObject.SetActive(true);
        enemyInventory.gameObject.SetActive(false);
        upgradeInventory.gameObject.SetActive(false);
        towerInventory.UpdateOptionsBasedOnMoney();
    }
    #endregion
}