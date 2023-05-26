using UnityEngine;
using UnityEngine.UI;
using TMPro;
using System.Collections;
using UnityEngine.Assertions;
using GameMaster;

public class TowerInventory : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private BaseTowerObjectSO basicTowerSO = null;
    [SerializeField] private BaseTowerObjectSO arrowTowerSO = null;
    [SerializeField] private BaseTowerObjectSO fireTowerSO = null;
    [SerializeField] private BaseTowerObjectSO iceTowerSO = null;

    [Header("Required Refs")]
    [SerializeField] private TextMeshProUGUI basicTowerCostText = null;
    [SerializeField] private TextMeshProUGUI arrowTowerCostText = null;
    [SerializeField] private TextMeshProUGUI fireTowerCostText = null;
    [SerializeField] private TextMeshProUGUI iceTowerCostText = null;
    [SerializeField] private Button basicTowerButton = null;
    [SerializeField] private Button arrowTowerButton = null;
    [SerializeField] private Button fireTowerButton = null;
    [SerializeField] private Button iceTowerButton = null;
    [SerializeField] private CanvasGroup basicTowerCostCanvasGroup = null;
    [SerializeField] private CanvasGroup arrowTowerCostCanvasGroup = null;
    [SerializeField] private CanvasGroup fireTowerCostCanvasGroup = null;
    [SerializeField] private CanvasGroup iceTowerCostCanvasGroup = null;
    [SerializeField] private NodeUI nodeUI = null;
    #endregion

    #region Private Variablesull;
    private TowerBlueprint basicTower = new TowerBlueprint();
    private TowerBlueprint arrowTower = new TowerBlueprint();
    private TowerBlueprint fireTower = new TowerBlueprint();
    private TowerBlueprint iceTower = new TowerBlueprint();
    #endregion

    #region Unity Functions
    void Awake()
    {
        Assert.IsNotNull(basicTowerSO);
        Assert.IsNotNull(arrowTowerSO);
        Assert.IsNotNull(fireTowerSO);
        Assert.IsNotNull(iceTowerSO);
        Assert.IsNotNull(basicTowerCostText);
        Assert.IsNotNull(arrowTowerCostText);
        Assert.IsNotNull(fireTowerCostText);
        Assert.IsNotNull(iceTowerCostText);
        Assert.IsNotNull(basicTowerButton);
        Assert.IsNotNull(arrowTowerButton);
        Assert.IsNotNull(fireTowerButton);
        Assert.IsNotNull(iceTowerButton);
        Assert.IsNotNull(basicTowerCostCanvasGroup);
        Assert.IsNotNull(arrowTowerCostCanvasGroup);
        Assert.IsNotNull(fireTowerCostCanvasGroup);
        Assert.IsNotNull(iceTowerCostCanvasGroup);
        Assert.IsNotNull(nodeUI);
    }

    void Start()
    {
        basicTower.type = basicTowerSO.Type;
        arrowTower.type = arrowTowerSO.Type;
        fireTower.type = fireTowerSO.Type;
        iceTower.type = iceTowerSO.Type;

        basicTower.buildTime = basicTowerSO.buildTime;
        arrowTower.buildTime = arrowTowerSO.buildTime;
        fireTower.buildTime = fireTowerSO.buildTime;
        iceTower.buildTime = iceTowerSO.buildTime;
    }
    #endregion

    #region Public Functions
    public void SelectBasicTower()
    {
        TowerBlueprint basicTowerBlueprint = new TowerBlueprint
        {
            type = basicTowerSO.Type,
            buildTime = basicTowerSO.buildTime,
            cost = basicTowerSO.Cost
        };
        buildSelectedTower(basicTowerBlueprint);
    }

    public void SelectArrowTower()
    {
        TowerBlueprint arrowTowerBlueprint = new TowerBlueprint
        {
            type = arrowTowerSO.Type,
            buildTime = arrowTowerSO.buildTime,
            cost = arrowTowerSO.Cost
        };
        buildSelectedTower(arrowTowerBlueprint);
    }

    public void SelectFireTower()
    {
        TowerBlueprint fireTowerBlueprint = new TowerBlueprint
        {
            type = fireTowerSO.Type,
            buildTime = fireTowerSO.buildTime,
            cost = fireTowerSO.Cost
        };
        buildSelectedTower(fireTowerBlueprint);
    }

    public void SelectIceTower()
    {
        TowerBlueprint iceTowerBlueprint = new TowerBlueprint
        {
            type = iceTowerSO.Type,
            buildTime = iceTowerSO.buildTime,
            cost = iceTowerSO.Cost
        };
        buildSelectedTower(iceTowerBlueprint);
    }

    public void UpdateOptionsBasedOnMoney()
    {
        updateCostsFromSO();
        disableButtonIfNoMoney(basicTower.cost, basicTowerButton, basicTowerCostCanvasGroup);
        disableButtonIfNoMoney(arrowTower.cost, arrowTowerButton, arrowTowerCostCanvasGroup);
        disableButtonIfNoMoney(fireTower.cost, fireTowerButton, fireTowerCostCanvasGroup);
        disableButtonIfNoMoney(iceTower.cost, iceTowerButton, iceTowerCostCanvasGroup);
    }
    #endregion

    #region Private Functions
    private void updateCostsFromSO()
    {
        basicTower.cost = basicTowerSO.Cost;
        arrowTower.cost = arrowTowerSO.Cost;
        fireTower.cost = fireTowerSO.Cost;
        iceTower.cost = iceTowerSO.Cost;

        basicTowerCostText.text = $"{basicTowerSO.Cost}";
        arrowTowerCostText.text = $"{arrowTowerSO.Cost}";
        fireTowerCostText.text = $"{fireTowerSO.Cost}";
        iceTowerCostText.text = $"{iceTowerSO.Cost}";
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

    private void buildSelectedTower(TowerBlueprint towerBlueprint)
    {
        BaseNode selectedNode = NodeManager.Instance.SelectedNode;

        if (selectedNode is DefenderNode defenderNode)
        {
            defenderNode.BuildTower(towerBlueprint);
        }

        NodeManager.Instance.SelectedNode = null;
        nodeUI.Close();
    }
    #endregion
}