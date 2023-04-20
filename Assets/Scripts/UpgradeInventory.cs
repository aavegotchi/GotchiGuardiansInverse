using UnityEngine;
using UnityEngine.UI;
using TMPro;

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
    [SerializeField] private NodeUI nodeUI = null;
    #endregion 

    #region Private Variables
    private BaseTowerObjectSO towerObjectSO = null;
    private LickquidatorObjectSO lickquidatorObjectSO = null;
    private BaseTowerObjectSO newTowerObjectSO = null;
    // private LickquidatorObjectSO newLickquidatorObjectSO = null;
    private Transform transformHolder = null;
    private Image upgradeButtonImage = null;
    private Image sellButtonImage = null;
    #endregion

    #region Public Functions
    public void SelectUpgrade()
    {
        if (lickquidatorObjectSO != null)
        {
            // TODO enemy upgrades
            Debug.Log("upgrades not implemented for enemies yet");
            close();
            return;
        }

        if (towerObjectSO == null) return;

        if (towerObjectSO.Type == TowerManager.TowerType.BasicTower)
        {
            newTowerObjectSO = basicTower2SO; // basic to bomb
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.BombTower)
        {
            newTowerObjectSO = basicTower3SO; // bomb to slow
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.ArrowTower1)
        {
            newTowerObjectSO = arrowTower2SO; // arrow 1 to arrow 2
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.ArrowTower2)
        {
            newTowerObjectSO = arrowTower3SO; // arrow 2 to arrow 3
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.FireTower1)
        {
            newTowerObjectSO = fireTower2SO; // fire 1 to fire 2
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.FireTower2)
        {
            newTowerObjectSO = fireTower3SO; // fire 2 to fire 3
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.IceTower1)
        {
            newTowerObjectSO = iceTower2SO; // ice 1 to ice 2
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.IceTower2)
        {
            newTowerObjectSO = iceTower3SO; // ice 2 to ice 3
        }

        StatsManager.Instance.Money -= newTowerObjectSO.Cost;
        TowerBlueprint towerBlueprint = new TowerBlueprint();
        towerBlueprint.type = newTowerObjectSO.Type;
        towerBlueprint.cost = newTowerObjectSO.Cost;
        towerBlueprint.buildTime = newTowerObjectSO.buildTime;

        BaseTower tower = transformHolder.GetComponent<BaseTower>();
        towerBlueprint.node = tower.TowerBlueprint.node;
        tower.PlayDead();
        
        ProgressBarManager.Instance.GetAndShowProgressBar(towerBlueprint);
        
        close();
    }

    public void SelectSell()
    {
        // TODO enemy sell
        if (lickquidatorObjectSO != null)
        {
            Debug.Log("selling not implemented for enemies yet");
            close();
            return;
        }

        if (towerObjectSO == null) return;

        BaseTower tower = transformHolder.GetComponent<BaseTower>();
        TowerBlueprint towerBlueprint = tower.TowerBlueprint;
        StatsManager.Instance.Money += calculateSellReward(towerBlueprint.cost);
        tower.PlayDead();

        towerBlueprint.node.Occupied = false;
        towerBlueprint.node.Renderer.material = towerBlueprint.node.UnoccupiedMaterial;
        
        close();
    }

    public void Open(BaseTowerObjectSO objectSO, Transform towerTransform)
    {
        towerObjectSO = objectSO;
        transformHolder = towerTransform;
        sellRewardText.text = $"{calculateSellReward(towerObjectSO.Cost)}";

        if (upgradeButtonImage == null || sellButtonImage == null)
        {
            upgradeButtonImage = upgradeButton.gameObject.GetComponent<Image>();
            sellButtonImage = sellButton.gameObject.GetComponent<Image>();
        }
        
        if (towerObjectSO.Type == TowerManager.TowerType.BasicTower)
        {
            // basic to bomb
            upgradeButtonImage.sprite = basicTower2Sprite;
            sellButtonImage.sprite = basicTower1Sprite;
            upgradeCostText.text = $"{basicTower2SO.Cost}";
            disableButtonIfNoMoney(basicTower2SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.BombTower)
        {
            // bomb to slow
            upgradeButtonImage.sprite = basicTower3Sprite;
            sellButtonImage.sprite = basicTower2Sprite;
            upgradeCostText.text = $"{basicTower3SO.Cost}";
            disableButtonIfNoMoney(basicTower3SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.SlowTower)
        {
            // slow, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;

            upgradeButtonImage.sprite = basicTower3Sprite;
            sellButtonImage.sprite = basicTower3Sprite;
            upgradeCostText.text = "";
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.ArrowTower1)
        {
            // arrow 1 to arrow 2
            upgradeButtonImage.sprite = arrowTower2Sprite;
            sellButtonImage.sprite = arrowTower1Sprite;
            upgradeCostText.text = $"{arrowTower2SO.Cost}";
            disableButtonIfNoMoney(arrowTower2SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.ArrowTower2)
        {
            // arrow 2 to arrow 3
            upgradeButtonImage.sprite = arrowTower3Sprite;
            sellButtonImage.sprite = arrowTower2Sprite;
            upgradeCostText.text = $"{arrowTower3SO.Cost}";
            disableButtonIfNoMoney(arrowTower3SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.ArrowTower3)
        {
            // arrow 3, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;

            upgradeButtonImage.sprite = arrowTower3Sprite;
            sellButtonImage.sprite = arrowTower3Sprite;
            upgradeCostText.text = "";
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.FireTower1)
        {
            // fire 1 to fire 2
            upgradeButtonImage.sprite = fireTower2Sprite;
            sellButtonImage.sprite = fireTower1Sprite;
            upgradeCostText.text = $"{fireTower2SO.Cost}";
            disableButtonIfNoMoney(fireTower2SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.FireTower2)
        {
            // fire 2 to fire 3
            upgradeButtonImage.sprite = fireTower3Sprite;
            sellButtonImage.sprite = fireTower2Sprite;
            upgradeCostText.text = $"{fireTower3SO.Cost}";
            disableButtonIfNoMoney(fireTower3SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.FireTower3)
        {
            // fire 3, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;
            
            upgradeButtonImage.sprite = fireTower3Sprite;
            sellButtonImage.sprite = fireTower3Sprite;
            upgradeCostText.text = "";
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.IceTower1)
        {
            // ice 1 to ice 2
            upgradeButtonImage.sprite = iceTower2Sprite;
            sellButtonImage.sprite = iceTower1Sprite;
            upgradeCostText.text = $"{iceTower2SO.Cost}";
            disableButtonIfNoMoney(iceTower2SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.IceTower2)
        {
            // ice 2 to ice 3
            upgradeButtonImage.sprite = iceTower3Sprite;
            sellButtonImage.sprite = iceTower2Sprite;
            upgradeCostText.text = $"{iceTower3SO.Cost}";
            disableButtonIfNoMoney(iceTower3SO.Cost, upgradeButton, upgradeCanvasGroup);
        }
        else if (towerObjectSO.Type == TowerManager.TowerType.IceTower3)
        {
            // ice 3, so don't show upgrade
            upgradeCanvasGroup.alpha = 0f;
            upgradeButton.enabled = false;

            upgradeButtonImage.sprite = iceTower3Sprite;
            sellButtonImage.sprite = iceTower3Sprite;
            upgradeCostText.text = "";
        }
        
        gameObject.SetActive(true);
    }

    public void Open(LickquidatorObjectSO objectSO, Transform enemyTransform)
    {
        lickquidatorObjectSO = objectSO;
        transformHolder = enemyTransform;
        disableButtonIfNoMoney(lickquidatorObjectSO.Cost, upgradeButton, upgradeCanvasGroup);
        
        if (lickquidatorObjectSO != null)
        {
            // TODO enemy upgrades
            Debug.Log("enemy upgrade/sell sprites are not yet implemented");
        }

        gameObject.SetActive(true);
    }
    #endregion

    #region Private Functions
    private int calculateSellReward(int cost)
    {
        float sellReward = cost * generalSO.TowerSellRewardMultipleByCost;
        return Mathf.RoundToInt(sellReward);
    }

    private void close()
    {
        towerObjectSO = null;
        lickquidatorObjectSO = null;
        nodeUI.Close();
    }

    private void disableButtonIfNoMoney(int cost, Button button, CanvasGroup canvasGroup)
    {
        if (StatsManager.Instance.Money < cost)
        {
            canvasGroup.alpha = 0f;
            button.enabled = false;
        }
        else
        {
            canvasGroup.alpha = 1f;
            button.enabled = true;
        }
    }
    #endregion
}