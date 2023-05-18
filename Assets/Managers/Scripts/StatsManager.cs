using UnityEngine;
using System.Collections.Generic;
using Gotchi.Lickquidator.Manager;

public class StatsManager : MonoBehaviour
{
    #region Public Variables
    public static StatsManager Instance = null;

    public int Money
    {
        get { return generalSO.SpiritGems; }
        set { generalSO.SpiritGems = value; }
    }
    #endregion

    #region Fields
    [SerializeField] private GeneralSO generalSO = null;
    #endregion

    #region Private Variables
    private List<EnemyBlueprint> enemyBlueprintsCreate = new List<EnemyBlueprint>();
    private List<TowerBlueprint> towerBlueprintsCreate = new List<TowerBlueprint>();
    private List<EnemyBlueprint> enemyBlueprintsKill = new List<EnemyBlueprint>();
    private List<TowerBlueprint> towerBlueprintsKill = new List<TowerBlueprint>();
    #endregion

    #region Unity Functions
    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
        }
        else
        {
            Destroy(gameObject);
        }
    }
    #endregion

    #region Public Functions
    public void TrackCreateEnemy(EnemyBlueprint enemyBlueprint)
    {
        enemyBlueprintsCreate.Add(enemyBlueprint);
    }

    public void TrackCreateTower(TowerBlueprint towerBlueprint)
    {
        towerBlueprintsCreate.Add(towerBlueprint);
    }

    public void TrackKillEnemy(EnemyBlueprint enemyBlueprint)
    {
        enemyBlueprintsKill.Add(enemyBlueprint);
    }

    public void TrackKillTower(TowerBlueprint towerBlueprint)
    {
        towerBlueprintsKill.Add(towerBlueprint);
    }

    public int GetEnemyCreateCosts(LickquidatorManager.LickquidatorType enemyType)
    {
        int sumCosts = 0;
        foreach (EnemyBlueprint enemyBlueprint in enemyBlueprintsCreate)
        {
            if (enemyBlueprint.type == enemyType) 
            {
                sumCosts += enemyBlueprint.cost;
            }
        }
        return sumCosts;
    }

    public int GetTowerCreateCosts(TowerPool.TowerType towerType)
    {
        int sumCosts = 0;
        foreach (TowerBlueprint towerBlueprint in towerBlueprintsCreate)
        {
            if (towerBlueprint.type == towerType)
            {
                sumCosts += towerBlueprint.cost;
            }
        }
        return sumCosts;
    }

    public int GetEnemyKillCosts(LickquidatorManager.LickquidatorType enemyType)
    {
        int sumCosts = 0;
        foreach (EnemyBlueprint enemyBlueprint in enemyBlueprintsKill)
        {
            if (enemyBlueprint.type == enemyType)
            {
                sumCosts += enemyBlueprint.cost;
            }
        }
        return sumCosts;
    }

    public int GetTowerKillCosts(TowerPool.TowerType towerType)
    {
        int sumCosts = 0;
        foreach (TowerBlueprint towerBlueprint in towerBlueprintsKill)
        {
            if (towerBlueprint.type == towerType)
            {
                sumCosts += towerBlueprint.cost;
            }
        }
        return sumCosts;
    }

    public void ClearCreateAndKillStats()
    {
        enemyBlueprintsCreate.Clear();
        towerBlueprintsCreate.Clear();
        enemyBlueprintsKill.Clear();
        towerBlueprintsKill.Clear();        
    }

    public int GetEnemiesSpawnBonus()
    {
        float bonus = 0f;
        foreach (EnemyBlueprint enemy in enemyBlueprintsCreate)
        {
            bonus += enemy.cost * generalSO.EnemySpawnRewardMultipleByCost;
        }
        int roundedBonus = Mathf.RoundToInt(bonus);
        return roundedBonus;
    }
    #endregion
}