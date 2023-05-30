using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;
using Gotchi.Lickquidator.Manager;

public class EnemyInventory : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private LickquidatorObjectSO aerialLickquidatorSO = null;
    [SerializeField] private LickquidatorObjectSO bossLickquidatorSO = null;
    [SerializeField] private LickquidatorObjectSO speedyBoiLickquidatorSO = null;
    [SerializeField] private LickquidatorObjectSO splitterLickquidatorSO = null;

    [Header("Required Refs")]
    [SerializeField] private TextMeshProUGUI aerialLickquidatorCostText = null;
    [SerializeField] private TextMeshProUGUI bossLickquidatorCostText = null;
    [SerializeField] private TextMeshProUGUI speedyBoiLickquidatorCostText = null;
    [SerializeField] private TextMeshProUGUI splitterLickquidatorCostText = null;

    [SerializeField] private Button aerialLickquidatorButton = null;
    [SerializeField] private Button bossLickquidatorButton = null;
    [SerializeField] private Button speedyBoiLickquidatorButton = null;
    [SerializeField] private Button splitterLickquidatorButton = null;

    [SerializeField] private CanvasGroup aerialLickquidatorCostCanvasGroup = null;
    [SerializeField] private CanvasGroup bossLickquidatorCostCanvasGroup = null;
    [SerializeField] private CanvasGroup speedyBoiLickquidatorCostCanvasGroup = null;
    [SerializeField] private CanvasGroup splitterLickquidatorCostCanvasGroup = null;
    
    [SerializeField] private NodeUI nodeUI = null;
    #endregion

    #region Private Variables
    private EnemyBlueprint splitterLickquidator = new EnemyBlueprint();
    private EnemyBlueprint aerialLickquidator = new EnemyBlueprint();
    private EnemyBlueprint bossLickquidator = new EnemyBlueprint();
    private EnemyBlueprint speedyBoiLickquidator = new EnemyBlueprint();
    #endregion

    #region Unity Functions

    private void Awake()
    {
        Assert.IsNotNull(splitterLickquidatorSO);
        Assert.IsNotNull(aerialLickquidatorSO);
        Assert.IsNotNull(bossLickquidatorSO);
        Assert.IsNotNull(speedyBoiLickquidatorSO);
        Assert.IsNotNull(splitterLickquidatorCostText);
        Assert.IsNotNull(aerialLickquidatorCostText);
        Assert.IsNotNull(bossLickquidatorCostText);
        Assert.IsNotNull(speedyBoiLickquidatorCostText);
        Assert.IsNotNull(splitterLickquidatorButton);
        Assert.IsNotNull(aerialLickquidatorButton);
        Assert.IsNotNull(bossLickquidatorButton);
        Assert.IsNotNull(speedyBoiLickquidatorButton);
        Assert.IsNotNull(splitterLickquidatorCostCanvasGroup);
        Assert.IsNotNull(aerialLickquidatorCostCanvasGroup);
        Assert.IsNotNull(bossLickquidatorCostCanvasGroup);
        Assert.IsNotNull(speedyBoiLickquidatorCostCanvasGroup);
        Assert.IsNotNull(nodeUI);
    }

    void Start()
    {
        splitterLickquidator.type = LickquidatorManager.LickquidatorType.SplitterLickquidator;
        aerialLickquidator.type = LickquidatorManager.LickquidatorType.AerialLickquidator;
        bossLickquidator.type = LickquidatorManager.LickquidatorType.BossLickquidator;
        speedyBoiLickquidator.type = LickquidatorManager.LickquidatorType.SpeedyBoiLickquidator;

        splitterLickquidator.buildTime = splitterLickquidatorSO.buildTime;
        aerialLickquidator.buildTime = aerialLickquidatorSO.buildTime;
        bossLickquidator.buildTime = bossLickquidatorSO.buildTime;
        speedyBoiLickquidator.buildTime = speedyBoiLickquidatorSO.buildTime;

    }
    #endregion

    #region Public Functions
    public void SelectSplitterLickquidator()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = LickquidatorManager.LickquidatorType.SplitterLickquidator,
            buildTime = splitterLickquidatorSO.buildTime,
            cost = splitterLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void SelectAerialLickquidator()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = LickquidatorManager.LickquidatorType.AerialLickquidator,
            buildTime = aerialLickquidatorSO.buildTime,
            cost = aerialLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void SelectBossLickquidator()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = LickquidatorManager.LickquidatorType.BossLickquidator,
            buildTime = bossLickquidatorSO.buildTime,
            cost = bossLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void SelectSpeedyBoi()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = LickquidatorManager.LickquidatorType.SpeedyBoiLickquidator,
            buildTime = speedyBoiLickquidatorSO.buildTime,
            cost = speedyBoiLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void UpdateOptionsBasedOnMoney()
    {
        updateCostsFromSO();
        disableButtonIfNoMoney(splitterLickquidator.cost, splitterLickquidatorButton, splitterLickquidatorCostCanvasGroup);
        disableButtonIfNoMoney(aerialLickquidator.cost, aerialLickquidatorButton, aerialLickquidatorCostCanvasGroup);
        disableButtonIfNoMoney(bossLickquidator.cost, bossLickquidatorButton, bossLickquidatorCostCanvasGroup);
        disableButtonIfNoMoney(speedyBoiLickquidator.cost, speedyBoiLickquidatorButton, speedyBoiLickquidatorCostCanvasGroup);
    }

    public void DisableOptions()
    {
        disableButton(splitterLickquidatorButton, splitterLickquidatorCostCanvasGroup);
        disableButton(aerialLickquidatorButton, aerialLickquidatorCostCanvasGroup);
        disableButton(bossLickquidatorButton, bossLickquidatorCostCanvasGroup);
        disableButton(speedyBoiLickquidatorButton, speedyBoiLickquidatorCostCanvasGroup);
    }
    #endregion

    #region Private Functions
    private void updateCostsFromSO()
    {
        splitterLickquidator.cost = splitterLickquidatorSO.Cost;
        aerialLickquidator.cost = aerialLickquidatorSO.Cost;
        bossLickquidator.cost = bossLickquidatorSO.Cost;
        speedyBoiLickquidator.cost = speedyBoiLickquidatorSO.Cost;

        splitterLickquidatorCostText.text = $"{splitterLickquidatorSO.Cost}";
        aerialLickquidatorCostText.text = $"{aerialLickquidatorSO.Cost}";
        bossLickquidatorCostText.text = $"{bossLickquidatorSO.Cost}";
        speedyBoiLickquidatorCostText.text = $"{speedyBoiLickquidatorSO.Cost}";
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

    private void disableButton(Button button, CanvasGroup canvasGroup)
    {
        canvasGroup.alpha = 0f;
        button.enabled = false;
    }

    private void buildSelectedEnemy(EnemyBlueprint enemyBlueprint)
    {
        BaseNode selectedNode = NodeManager.Instance.SelectedNode;

        if (selectedNode is AttackerNode attackerNode)
        {
            attackerNode.AddSpawnedEnemy(enemyBlueprint);
        }

        NodeManager.Instance.SelectedNode = null;
        nodeUI.Close();
    }
    #endregion
}
