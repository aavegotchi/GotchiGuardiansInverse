using UnityEngine;
using UnityEngine.UI;
using TMPro;
using UnityEngine.Assertions;
using Gotchi.Events;

public class EnemyInventory : MonoBehaviour
{
    #region Fields
    [Header("Settings")]
    [SerializeField] private LickquidatorObjectSO pawnLickquidatorSO = null;
    [SerializeField] private LickquidatorObjectSO aerialLickquidatorSO = null;
    [SerializeField] private LickquidatorObjectSO bossLickquidatorSO = null;

    [Header("Required Refs")]
    [SerializeField] private TextMeshProUGUI pawnLickquidatorCostText = null;
    [SerializeField] private TextMeshProUGUI aerialLickquidatorCostText = null;
    [SerializeField] private TextMeshProUGUI bossLickquidatorCostText = null;
    [SerializeField] private Button pawnLickquidatorButton = null;
    [SerializeField] private Button aerialLickquidatorButton = null;
    [SerializeField] private Button bossLickquidatorButton = null;
    [SerializeField] private CanvasGroup pawnLickquidatorCostCanvasGroup = null;
    [SerializeField] private CanvasGroup aerialLickquidatorCostCanvasGroup = null;
    [SerializeField] private CanvasGroup bossLickquidatorCostCanvasGroup = null;
    [SerializeField] private NodeUI nodeUI = null;
    #endregion

    #region Private Variables
    private EnemyBlueprint pawnLickquidator = new EnemyBlueprint();
    private EnemyBlueprint aerialLickquidator = new EnemyBlueprint();
    private EnemyBlueprint bossLickquidator = new EnemyBlueprint();
    #endregion

    #region Unity Functions

    private void Awake()
    {
        Assert.IsNotNull(pawnLickquidatorSO);
        Assert.IsNotNull(aerialLickquidatorSO);
        Assert.IsNotNull(bossLickquidatorSO);
        Assert.IsNotNull(pawnLickquidatorCostText);
        Assert.IsNotNull(aerialLickquidatorCostText);
        Assert.IsNotNull(bossLickquidatorCostText);
        Assert.IsNotNull(pawnLickquidatorButton);
        Assert.IsNotNull(aerialLickquidatorButton);
        Assert.IsNotNull(bossLickquidatorButton);
        Assert.IsNotNull(pawnLickquidatorCostCanvasGroup);
        Assert.IsNotNull(aerialLickquidatorCostCanvasGroup);
        Assert.IsNotNull(bossLickquidatorCostCanvasGroup);
        Assert.IsNotNull(nodeUI);
    }

    void Start()
    {
        pawnLickquidator.type = EnemyManager.EnemyType.PawnLickquidator;
        aerialLickquidator.type = EnemyManager.EnemyType.AerialLickquidator;
        bossLickquidator.type = EnemyManager.EnemyType.BossLickquidator;

        pawnLickquidator.buildTime = pawnLickquidatorSO.buildTime;
        aerialLickquidator.buildTime = aerialLickquidatorSO.buildTime;
        bossLickquidator.buildTime = bossLickquidatorSO.buildTime;

    }
    #endregion

    #region Public Functions
    public void SelectPawnLickquidator()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = EnemyManager.EnemyType.PawnLickquidator,
            buildTime = pawnLickquidatorSO.buildTime,
            cost = pawnLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void SelectAerialLickquidator()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = EnemyManager.EnemyType.AerialLickquidator,
            buildTime = aerialLickquidatorSO.buildTime,
            cost = aerialLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void SelectBossLickquidator()
    {
        EnemyBlueprint enemyBlueprint = new EnemyBlueprint
        {
            type = EnemyManager.EnemyType.BossLickquidator,
            buildTime = bossLickquidatorSO.buildTime,
            cost = bossLickquidatorSO.Cost
        };
        buildSelectedEnemy(enemyBlueprint);
    }

    public void UpdateOptionsBasedOnMoney()
    {
        updateCostsFromSO();
        disableButtonIfNoMoney(pawnLickquidator.cost, pawnLickquidatorButton, pawnLickquidatorCostCanvasGroup);
        disableButtonIfNoMoney(aerialLickquidator.cost, aerialLickquidatorButton, aerialLickquidatorCostCanvasGroup);
        disableButtonIfNoMoney(bossLickquidator.cost, bossLickquidatorButton, bossLickquidatorCostCanvasGroup);
    }
    #endregion

    #region Private Functions
    private void updateCostsFromSO()
    {
        pawnLickquidator.cost = pawnLickquidatorSO.Cost;
        aerialLickquidator.cost = aerialLickquidatorSO.Cost;
        bossLickquidator.cost = bossLickquidatorSO.Cost;

        pawnLickquidatorCostText.text = $"{pawnLickquidatorSO.Cost}";
        aerialLickquidatorCostText.text = $"{aerialLickquidatorSO.Cost}";
        bossLickquidatorCostText.text = $"{bossLickquidatorSO.Cost}";
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

    private void buildSelectedEnemy(EnemyBlueprint enemyBlueprint)
    {
        BaseNode selectedNode = NodeManager.Instance.SelectedNode;

        if(selectedNode != null) selectedNode.Occupied = true;

        StatsManager.Instance.Money -= enemyBlueprint.cost;
        enemyBlueprint.node = selectedNode;
        ProgressBarManager.Instance.GetAndShowProgressBar(enemyBlueprint);
        selectedNode.BuildEffect.SetActive(true);
        EventBus.EnemyEvents.EnemyStarted();
        nodeUI.Close();

        if (selectedNode is AttackerNode attackerNode)
        {
            attackerNode.SetSpawnedEnemy(enemyBlueprint);
        }

        NodeManager.Instance.SelectedNode = null;
    }
    #endregion
}
