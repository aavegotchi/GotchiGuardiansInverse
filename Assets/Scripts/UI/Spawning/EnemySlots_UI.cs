using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class EnemySlots_UI : MonoBehaviour
{
    #region Fields
    [SerializeField] Image[] enemySlots;

    [SerializeField] Color maxOccupancyColor;
    [SerializeField] Color withinMaxOccupancyColor;

    #endregion

    #region Private Variables
    private int activeSlots = 0;
    #endregion

    #region Unity Functions
    private void Start()
    {
        deactivateAllSlots();
    }
    #endregion

    #region Public Functions
    public void ActivateNextSlot()
    {
        if (activeSlots >= enemySlots.Length) return;

        enemySlots[activeSlots].gameObject.SetActive(true);
        activeSlots++;

        if (activeSlots == enemySlots.Length)
        {
            setSlotsColor(maxOccupancyColor);
        }
        else
        {
            setSlotsColor(withinMaxOccupancyColor);
        }
    }

    // Deactivate the last active slot and update the color
    // public void DeactivateLastSlot()
    // {
    //     if (activeSlots > 0)
    //     {
    //         activeSlots--;
    //         enemySlots[activeSlots].gameObject.SetActive(false);
    //         SetSlotsColor(withinMaxOccupancyColor);
    //     }
    // }
    #endregion

    #region Private Functions
    // Deactivate all slots at the beginning
    private void deactivateAllSlots()
    {
        foreach (Image enemySlot in enemySlots)
        {
            enemySlot.gameObject.SetActive(false);
        }
    }

    // Set the color of the enemySlots
    private void setSlotsColor(Color color)
    {
        foreach (Image enemySlot in enemySlots)
        {
            enemySlot.color = color;
        }
    }
    #endregion
}