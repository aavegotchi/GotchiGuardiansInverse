using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Gotchi.Lickquidators;

public class EnemyQueue_UI : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private Image button1Image = null;
    [SerializeField] private Image button2Image = null;
    [SerializeField] private Image button3Image = null;
    [SerializeField] private Image button4Image = null;
    [SerializeField] private Image button5Image = null;
    [SerializeField] private Image button6Image = null;
    [SerializeField] private Image button7Image = null;
    [SerializeField] private Image button8Image = null;
    [SerializeField] private Button button1Btn = null;
    [SerializeField] private Button button2Btn = null;
    [SerializeField] private Button button3Btn = null;
    [SerializeField] private Button button4Btn = null;
    [SerializeField] private Button button5Btn = null;
    [SerializeField] private Button button6Btn = null;
    [SerializeField] private Button button7Btn = null;
    [SerializeField] private Button button8Btn = null;

    [Header("Attributes")]
    [SerializeField] private Sprite pawnLickquidatorSprite = null;
    [SerializeField] private Sprite aerialLickquidatorSprite = null;
    [SerializeField] private Sprite bossLickquidatorSprite = null;
    [SerializeField] private Sprite noneSprite = null;
    #endregion

    #region Public Functions
    public void SetButtons(List<EnemyBlueprint> spawnedEnemyBlueprints, int maxEnemiesPerNode)
    {
        resetButtons();

        EnemyBlueprint blueprint;
        for (int i=0; i<spawnedEnemyBlueprints.Count; i++)
        {
            blueprint = spawnedEnemyBlueprints[i];
            if (i == 0)
            {
                setButton(button1Image, button1Btn, blueprint.type);
            }
            else if (i == 1)
            {
                setButton(button2Image, button2Btn, blueprint.type);
            }
            else if (i == 2)
            {
                setButton(button3Image, button3Btn, blueprint.type);
            }
            else if (i == 3)
            {
                setButton(button4Image, button4Btn, blueprint.type);
            }
            else if (i == 4)
            {
                setButton(button5Image, button5Btn, blueprint.type);
            }
            else if (i == 5)
            {
                setButton(button6Image, button6Btn, blueprint.type);
            }
            else if (i == 6)
            {
                setButton(button7Image, button7Btn, blueprint.type);
            }
            else if (i == 7)
            {
                setButton(button8Image, button8Btn, blueprint.type);
            }
        }
    }
    #endregion

    #region Private Functions
    private void resetButtons()
    {
        button1Image.sprite = noneSprite;
        button2Image.sprite = noneSprite;
        button3Image.sprite = noneSprite;
        button4Image.sprite = noneSprite;
        button5Image.sprite = noneSprite;
        button6Image.sprite = noneSprite;
        button7Image.sprite = noneSprite;
        button8Image.sprite = noneSprite;
        button1Btn.enabled = false;
        button2Btn.enabled = false;
        button3Btn.enabled = false;
        button4Btn.enabled = false;
        button5Btn.enabled = false;
        button6Btn.enabled = false;
        button7Btn.enabled = false;
        button8Btn.enabled = false;
    }

    private void setButton(Image buttonImage, Button buttonBtn, LickquidatorManager.LickquidatorType type)
    {
        if (type == LickquidatorManager.LickquidatorType.PawnLickquidator)
        {
            buttonImage.sprite = pawnLickquidatorSprite;
        }
        else if (type == LickquidatorManager.LickquidatorType.AerialLickquidator)
        {
            buttonImage.sprite = aerialLickquidatorSprite;
        }
        else if (type == LickquidatorManager.LickquidatorType.BossLickquidator)
        {
            buttonImage.sprite = bossLickquidatorSprite;
        }
        buttonBtn.enabled = true;
    }
    #endregion
}