using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PhaseManager;
using PhaseManager.Presenter;

public class BuildSphere : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private AttackerNode node = null;
    [SerializeField] private EnemyQueue_UI enemyQueueUI = null;
    #endregion

    #region Unity Functions
    private void OnMouseDown()
    {
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Prep) return;

        node.OpenNodeUI();
        enemyQueueUI.gameObject.SetActive(true);
        enemyQueueUI.SetButtons(node.SpawnedEnemyBlueprints, node.MaxEnemiesPerNode);
    }
    #endregion
}
