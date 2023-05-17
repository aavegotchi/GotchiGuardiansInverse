using UnityEngine;
using UnityEngine.UI;
using TMPro;
using Gotchi.Lickquidators;

public class UpgradeInventory : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private GeneralSO generalSO = null;
    [SerializeField] private BaseTowerObjectSO basicTower2SO = null;
    [SerializeField] private BaseTowerObjectSO basicTower3SO = null;
    [SerializeField] private BaseTowerObjectSO arrowTower2SO = null;
    [SerializeField] private BaseTowerObjectSO arrowTower3SO = null;
    [SerializeField] private BaseTowerObjectSO fireTower2SO = null;
    [SerializeField] private BaseTowerObjectSO fireTower3SO = null;
    [SerializeField] private BaseTowerObjectSO iceTower2SO = null;
    [SerializeField] private BaseTowerObjectSO iceTower3SO = null;

    [Header("Required Refs")]
    [SerializeField] private TextMeshProUGUI upgradeCostText = null;
    [SerializeField] private TextMeshProUGUI upgradeText = null;
    [SerializeField] private TextMeshProUGUI sellRewardText = null;
    [SerializeField] private Button upgradeButton = null;
    [SerializeField] private Button sellButton = null;
    [SerializeField] private CanvasGroup upgradeCanvasGroup = null;
    [SerializeField] private Sprite basicTower1Sprite = null;
    [SerializeField] private Sprite basicTower2Sprite = null;
    [SerializeField] private Sprite basicTower3Sprite = null;
    [SerializeField] private Sprite arrowTower1Sprite = null;
    [SerializeField] private Sprite arrowTower2Sprite = null;
    [SerializeField] private Sprite arrowTower3Sprite = null;
    [SerializeField] private Sprite fireTower1Sprite = null;
    [SerializeField] private Sprite fireTower2Sprite = null;
    [SerializeField] private Sprite fireTower3Sprite = null;
    [SerializeField] private Sprite iceTower1Sprite = null;
    [SerializeField] private Sprite iceTower2Sprite = null;
    [SerializeField] private Sprite iceTower3Sprite = null;
    [SerializeField] private Sprite pawnLickquidatorSprite = null;
    [SerializeField] private Sprite aerialLickquidatorSprite = null;
    [SerializeField] private Sprite bossLickquidatorSprite = null;
    [SerializeField] private NodeUI nodeUI = null;
    #endregion 

    #region Private Variables
    private Transform transformHolder = null;
    private Image upgradeButtonImage = null;
    private Image sellButtonImage = null;
    private BaseTower towerHolder = null;
    private LickquidatorModel enemyHolder = null;
    #endregion

    #region Public Functions
    public void SelectUpgrade()
    {    
        if (towerHolder != null)
        {
            towerHolder.OverrideRangeCircle(false);
            upgradeTower();
        }
        else if (enemyHolder != null)
        {
            upgradeLickquidator();
        }

        reset();
        nodeUI.Close();
    }

    public void SelectSell()
    {
        if (towerHolder != null)
        {
            sellTower();
        }
        else if (enemyHolder != null)
        {
            sellLickquidator();
        }

        reset();
        nodeUI.Close();
    }

    private void sellTower()
    {
        BaseTower tower = transformHolder.GetComponent<BaseTower>();
        TowerBlueprint towerBlueprint = tower.TowerBlueprint;
        StatsManager.Instance.Money += calculateSellReward(towerBlueprint.cost);
        tower.PlayDead();
        towerBlueprint.node.Occupied = false;
    }

    private void sellLickquidator()
    {
        LickquidatorPresenter enemy = transformHolder.GetComponent<LickquidatorPresenter>();
        EnemyBlueprint enemyBlueprint = enemy.Model.EnemyBlueprint;
        StatsManager.Instance.Money += calculateSellReward(enemyBlueprint.cost);
        enemy.PlayDead();
    }

    public void Open(Transform towerTransform, BaseTower tower)
    {
        reset();

        towerHolder = tower;
        transformHolder = towerTransform;
        sellRewardText.text = $"{calculateSellReward(towerHolder.ObjectSO.Cost)}";

        if (upgradeButtonImage == null || sellButtonImage == null)
        {
            upgradeButtonImage = upgradeButton.gameObject.GetComponent<Image>();
            sellButtonImage = sellButton.gameObject.GetComponent<Image>();
        }
        
        if (towerHolder.ObjectSO.Type == TowerPool.TowerType.BasicTower)
        {
            // basic to bomb
            upgradeButtonImage.sprite = basicTower2Sprite;
            sellButtonImage.sprite = basicTower1Sprite;
            upgradeText.text = "Evolve to II";
            upgradeCostText.text = $"{basicTower2SO.Cost}";
            disableUpgradeButtonIfNoMoney(basicTower2SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.BombTower)
        {
            // bomb to slow
            upgradeButtonImage.sprite = basicTower3Sprite;
            sellButtonImage.sprite = basicTower2Sprite;
            upgradeText.text = "Evolve to III";
            upgradeCostText.text = $"{basicTower3SO.Cost}";
            disableUpgradeButtonIfNoMoney(basicTower3SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.SlowTower)
        {
            // slow, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;

            upgradeButtonImage.sprite = basicTower3Sprite;
            sellButtonImage.sprite = basicTower3Sprite;
            upgradeText.text = "";
            upgradeCostText.text = "";
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.ArrowTower1)
        {
            // arrow 1 to arrow 2
            upgradeButtonImage.sprite = arrowTower2Sprite;
            sellButtonImage.sprite = arrowTower1Sprite;
            upgradeText.text = "Evolve to II";
            upgradeCostText.text = $"{arrowTower2SO.Cost}";
            disableUpgradeButtonIfNoMoney(arrowTower2SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.ArrowTower2)
        {
            // arrow 2 to arrow 3
            upgradeButtonImage.sprite = arrowTower3Sprite;
            sellButtonImage.sprite = arrowTower2Sprite;
            upgradeText.text = "Evolve to III";
            upgradeCostText.text = $"{arrowTower3SO.Cost}";
            disableUpgradeButtonIfNoMoney(arrowTower3SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.ArrowTower3)
        {
            // arrow 3, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;

            upgradeButtonImage.sprite = arrowTower3Sprite;
            sellButtonImage.sprite = arrowTower3Sprite;
            upgradeText.text = "";
            upgradeCostText.text = "";
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.FireTower1)
        {
            // fire 1 to fire 2
            upgradeButtonImage.sprite = fireTower2Sprite;
            sellButtonImage.sprite = fireTower1Sprite;
            upgradeText.text = "Evolve to II";
            upgradeCostText.text = $"{fireTower2SO.Cost}";
            disableUpgradeButtonIfNoMoney(fireTower2SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.FireTower2)
        {
            // fire 2 to fire 3
            upgradeButtonImage.sprite = fireTower3Sprite;
            sellButtonImage.sprite = fireTower2Sprite;
            upgradeText.text = "Evolve to III";
            upgradeCostText.text = $"{fireTower3SO.Cost}";
            disableUpgradeButtonIfNoMoney(fireTower3SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.FireTower3)
        {
            // fire 3, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;
            
            upgradeButtonImage.sprite = fireTower3Sprite;
            sellButtonImage.sprite = fireTower3Sprite;
            upgradeText.text = "";
            upgradeCostText.text = "";
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.IceTower1)
        {
            // ice 1 to ice 2
            upgradeButtonImage.sprite = iceTower2Sprite;
            sellButtonImage.sprite = iceTower1Sprite;
            upgradeText.text = "Evolve to II";
            upgradeCostText.text = $"{iceTower2SO.Cost}";
            disableUpgradeButtonIfNoMoney(iceTower2SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.IceTower2)
        {
            // ice 2 to ice 3
            upgradeButtonImage.sprite = iceTower3Sprite;
            sellButtonImage.sprite = iceTower2Sprite;
            upgradeText.text = "Evolve to III";
            upgradeCostText.text = $"{iceTower3SO.Cost}";
            disableUpgradeButtonIfNoMoney(iceTower3SO.Cost);
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.IceTower3)
        {
            // ice 3, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;

            upgradeButtonImage.sprite = iceTower3Sprite;
            sellButtonImage.sprite = iceTower3Sprite;
            upgradeText.text = "";
            upgradeCostText.text = "";
        }
        
        gameObject.SetActive(true);
    }

    public void Open(Transform lickquidatorTransform, LickquidatorModel enemy)
    {
        reset();

        enemyHolder = enemy;
        transformHolder = lickquidatorTransform;
        sellRewardText.text = $"{calculateSellReward(enemyHolder.Config.Cost)}";

        if (upgradeButtonImage == null || sellButtonImage == null)
        {
            upgradeButtonImage = upgradeButton.gameObject.GetComponent<Image>();
            sellButtonImage = sellButton.gameObject.GetComponent<Image>();
        }

        int upgradeCost = calculateNewLickquidatorCost();
        upgradeCostText.text = $"{upgradeCost}";
        upgradeText.text = $"Upgrade to {convertToRomanNumeral(enemyHolder.Config.Level + 1)}";
        disableUpgradeButtonIfNoMoney(upgradeCost);
        
        if (enemyHolder.Config.Type == LickquidatorManager.LickquidatorType.PawnLickquidator)
        {
            upgradeButtonImage.sprite = pawnLickquidatorSprite;
            sellButtonImage.sprite = pawnLickquidatorSprite;
        }
        else if (enemyHolder.Config.Type == LickquidatorManager.LickquidatorType.AerialLickquidator)
        {
            upgradeButtonImage.sprite = aerialLickquidatorSprite;
            sellButtonImage.sprite = aerialLickquidatorSprite;
        }
        else if (enemyHolder.Config.Type == LickquidatorManager.LickquidatorType.BossLickquidator)
        {
            upgradeButtonImage.sprite = bossLickquidatorSprite;
            sellButtonImage.sprite = bossLickquidatorSprite;
        }

        if (enemyHolder.Config.Level == 10) // max level
        {
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;
            upgradeCostText.text = "";
        }

        gameObject.SetActive(true);
    }
    #endregion

    #region Private Functions
    private string convertToRomanNumeral(int num)
    {
        switch (num)
        {
            case 1: return "I";
            case 2: return "II";
            case 3: return "III";
            case 4: return "IV";
            case 5: return "V";
            case 6: return "VI";
            case 7: return "VII";
            case 8: return "VIII";
            case 9: return "IX";
            case 10: return "X";
        }
        return "";
    }

    private void upgradeTower()
    {
        if (towerHolder.ObjectSO.Type == TowerPool.TowerType.BasicTower)
        {
            towerHolder.ObjectSO = basicTower2SO; // basic to bomb
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.BombTower)
        {
            towerHolder.ObjectSO = basicTower3SO; // bomb to slow
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.ArrowTower1)
        {
            towerHolder.ObjectSO = arrowTower2SO; // arrow 1 to arrow 2
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.ArrowTower2)
        {
            towerHolder.ObjectSO = arrowTower3SO; // arrow 2 to arrow 3
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.FireTower1)
        {
            towerHolder.ObjectSO = fireTower2SO; // fire 1 to fire 2
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.FireTower2)
        {
            towerHolder.ObjectSO = fireTower3SO; // fire 2 to fire 3
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.IceTower1)
        {
            towerHolder.ObjectSO = iceTower2SO; // ice 1 to ice 2
        }
        else if (towerHolder.ObjectSO.Type == TowerPool.TowerType.IceTower2)
        {
            towerHolder.ObjectSO = iceTower3SO; // ice 2 to ice 3
        }
        
        StatsManager.Instance.Money -= towerHolder.ObjectSO.Cost;
        TowerBlueprint towerBlueprint = new TowerBlueprint();
        towerBlueprint.type = towerHolder.ObjectSO.Type;
        towerBlueprint.cost = towerHolder.ObjectSO.Cost;
        towerBlueprint.buildTime = towerHolder.ObjectSO.buildTime;

        BaseTower tower = transformHolder.GetComponent<BaseTower>();
        towerBlueprint.node = tower.TowerBlueprint.node;
        tower.PlayDead(true);
        
        BuildProgressPool_UI.Instance.GetAndShowProgressBar(towerBlueprint);
    }

    private void upgradeLickquidator()
    {
        enemyHolder.Config.Cost = calculateNewLickquidatorCost();
        enemyHolder.Config.Level += 1;
        float levelBasedUpgradeValue = enemyHolder.Config.Level * generalSO.GenericUpgradeMultipleByLevel;
        float levelBasedUpgradeValueInverse = enemyHolder.Config.Level / generalSO.GenericUpgradeMultipleByLevel;
        enemyHolder.Config.buildTime += levelBasedUpgradeValueInverse;
        enemyHolder.Config.AttackDamage += (int)levelBasedUpgradeValue;
        enemyHolder.Config.AttackCountdown = Mathf.Max(1f, enemyHolder.Config.AttackCountdown - enemyHolder.Config.AttackCountdown * enemyHolder.Config.Level * 0.1f);
        enemyHolder.Config.Health += Mathf.RoundToInt(levelBasedUpgradeValue);
        enemyHolder.Config.MovementSpeed += levelBasedUpgradeValue;

        StatsManager.Instance.Money -= enemyHolder.Config.Cost;
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint();
        enemyBlueprint.type = enemyHolder.Config.Type;
        enemyBlueprint.cost = enemyHolder.Config.Cost;
        enemyBlueprint.buildTime = enemyHolder.Config.buildTime;

        LickquidatorPresenter enemy = transformHolder.GetComponent<LickquidatorPresenter>();
        enemyBlueprint.node = enemy.Model.EnemyBlueprint.node;
        enemy.PlayDead(true);

        BuildProgressPool_UI.Instance.GetAndShowProgressBar(enemyBlueprint);
    }

    private int calculateNewLickquidatorCost()
    {
        float levelBasedUpgradeValueInverse = (enemyHolder.Config.Level + 1) / generalSO.GenericUpgradeMultipleByLevel;
        return enemyHolder.Config.Cost * Mathf.RoundToInt(Mathf.Max(1f, levelBasedUpgradeValueInverse));
    }

    private int calculateSellReward(int cost)
    {
        float sellReward = cost * generalSO.TowerSellRewardMultipleByCost;
        return Mathf.RoundToInt(sellReward);
    }

    private void reset()
    {
        towerHolder = null;
        enemyHolder = null;
    }

    private void disableUpgradeButtonIfNoMoney(int cost)
    {
        if (StatsManager.Instance.Money < cost)
        {
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;
        }
        else
        {
            upgradeCanvasGroup.alpha = 1f;
            upgradeButton.enabled = true;
        }
    }
    #endregion
}