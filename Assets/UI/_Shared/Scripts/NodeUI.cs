using UnityEngine;
using GameMaster;
using PhaseManager.Presenter;
using Gotchi.Lickquidator.Model;

public class NodeUI : MonoBehaviour
{
    #region Public Variables
    public TowerInventory TowerInventory
    {
        get { return towerInventory; }
    }

    public EnemyInventory EnemyInventory
    {
        get { return enemyInventory; }
    }

    public UpgradeInventory UpgradeInventory
    {
        get { return upgradeInventory; }
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private TowerInventory towerInventory = null;
    [SerializeField] private EnemyInventory enemyInventory = null;
    [SerializeField] private UpgradeInventory upgradeInventory = null;
    [SerializeField] private GameObject enemyQueueUI = null;
    [SerializeField] private Animator uiAnimator = null;
    #endregion

    #region Private Variables
    private NodeManager nodeManager = null;
    private PhasePresenter phasePresenter = null;
    private BuildProgressPool_UI progressBarManager = null;
    #endregion

    #region Unity Functions
    void Awake()
    {
        nodeManager = NodeManager.Instance;
        progressBarManager = BuildProgressPool_UI.Instance;
        phasePresenter = PhasePresenter.Instance;
    }

    void OnEnable()
    {
        GameMasterEvents.PhaseEvents.SurvivalPhaseStarted += Close;
    }

    void OnDisable()
    {
        GameMasterEvents.PhaseEvents.SurvivalPhaseStarted -= Close;
    }
    #endregion

    #region Public Functions
    public void Close()
    {
        gameObject.SetActive(false);
        towerInventory.gameObject.SetActive(false);
        enemyInventory.gameObject.SetActive(false);
        upgradeInventory.gameObject.SetActive(false);
        enemyQueueUI.SetActive(false);
    }

    public void Open()
    {
        GameMasterEvents.MenuEvents.MenuItemSelectedShort();
        gameObject.SetActive(true);
        uiAnimator.SetTrigger("Open");
    }

    public void OpenNodeUpgradeUI(Transform transformHolder, BaseTower tower)
    {
        Vector3 nodeUIPosition = transformHolder.position;
        nodeUIPosition.y = 25f;
        nodeUIPosition.z -= 8f;

        transform.position = nodeUIPosition;

        Close();
        upgradeInventory.Open(transformHolder, tower);
        Open();
    }  

    public void OpenNodeUpgradeUI(Transform transformHolder, LickquidatorModel enemy)
    {
        Vector3 nodeUIPosition = transformHolder.position;
        nodeUIPosition.y = 25f;
        nodeUIPosition.z -= 8f;

        transform.position = nodeUIPosition;

        Close();
        upgradeInventory.Open(transformHolder, enemy);
        Open();
    }  
    #endregion
}
