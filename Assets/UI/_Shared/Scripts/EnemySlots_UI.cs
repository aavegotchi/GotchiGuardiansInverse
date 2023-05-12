using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using TMPro;

public class EnemySlots_UI : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject buildSphereObj = null;
    [SerializeField] private GameObject buildSphereBallObj = null;
    [SerializeField] private TextMeshProUGUI numEnemiesText = null;
    #endregion

    #region Private Variables
    private int numSlotsOccupied = 0;
    #endregion

    #region Public Functions
    public void OccupyNextSlot(int maxEnemiesPerNode)
    {
        if (numSlotsOccupied >= maxEnemiesPerNode) return;

        if (numSlotsOccupied == 0)
        {
            buildSphereObj.SetActive(true);
            numEnemiesText.gameObject.SetActive(true);
        }

        numSlotsOccupied++;

        numEnemiesText.text = $"{numSlotsOccupied}/{maxEnemiesPerNode}";

        float newScale = 1f + 0.25f * numSlotsOccupied;
        buildSphereBallObj.transform.localScale = new Vector3(newScale, newScale, newScale);
    }

    // Deactivate the last active slot and update the color
    // public void DeactivateLastSlot()
    // {
    //      if (activeSlots == 1)
    //      { buildSphereObj.SetActive(false); }
    //     if (activeSlots > 0)
    //     {
    //         activeSlots--;
    //         enemySlots[activeSlots].gameObject.SetActive(false);
    //         SetSlotsColor(withinMaxOccupancyColor);
    //     }
    // }
    #endregion
}