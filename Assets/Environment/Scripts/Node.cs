using UnityEngine;
using UnityEngine.EventSystems;
using PhaseManager;
using PhaseManager.Presenter;

public class Node : MonoBehaviour
{
    #region Public Variables
    public enum NodeType
    {
        Defender,
        Attacker
    };

    public bool Occupied 
    {
        set { occupied = value; }
    }

    public GameObject BuildEffect
    {
        get { return buildEffect; }
    }

    public NodeUI NodeUI
    {
        get { return nodeUI; }
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject buildEffect = null; // TODO: follow pooling pattern
    [SerializeField] private NodeUI nodeUI = null;
    [SerializeField] private Renderer render = null;

    [Header("Attributes")]
    [SerializeField] private NodeType type = NodeType.Defender;
    #endregion

    #region Private Variables
    private bool occupied = false;
    private TowerInventory towerInventory = null;
    private EnemyInventory enemyInventory = null;
    #endregion

    #region Unity Functions
    void Start()
    {
        towerInventory = nodeUI.TowerInventory;
        enemyInventory = nodeUI.EnemyInventory;
    }

    void Update()
    {
        // TODO: potentially move this logic so that it's based on an event listener rather than running it in Update()
        if (PhasePresenter.Instance.GetCurrentPhase() != Phase.Prep && occupied)
        {
            occupied = false;
        }
    }

    void OnMouseEnter()
    {
        if (occupied || PhasePresenter.Instance.GetCurrentPhase() != PhaseManager.Phase.Prep) return;

        render.enabled = true;
    }

    void OnMouseExit()
    {
        render.enabled = false;
    }

    void OnMouseDown()
    {
        if (!render.enabled) return;
        
        openNodeUI();
    }
    #endregion

    #region Private Functions
    private void openNodeUI()
    {
        Vector3 nodeUIPosition = transform.position;
        nodeUIPosition.y = 10f;
        nodeUIPosition.z -= 8f;
        nodeUI.transform.position = nodeUIPosition;
        nodeUI.gameObject.SetActive(true);
        
        if (type == NodeType.Attacker)
        {
            towerInventory.gameObject.SetActive(false);
            enemyInventory.gameObject.SetActive(true);
            enemyInventory.UpdateOptionsBasedOnMoney();
        }
        else if (type == NodeType.Defender)
        {
            towerInventory.gameObject.SetActive(true);
            enemyInventory.gameObject.SetActive(false);
            towerInventory.UpdateOptionsBasedOnMoney();
        }
    }
    #endregion
}
