using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;
using Fusion;

public abstract class BaseNode : NetworkBehaviour
{
    #region Public Variables
    public bool Occupied
    {
        set { 
            occupied = value; 
            render.material = occupied ? occupiedMaterial : unoccupiedMaterial;
        }
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
    [SerializeField] private Renderer visualRenderer = null;
    [SerializeField] private Material unoccupiedMaterial = null;
    [SerializeField] private Material occupiedMaterial = null;
    [SerializeField] private GraphicRaycaster graphicRaycaster = null;
    [SerializeField] private EventSystem eventSystem = null;
    [SerializeField] private float distanceToNodeUI = 15;
    #endregion

    #region Private Variables
    protected TowerInventory towerInventory = null;
    protected EnemyInventory enemyInventory = null;
    protected UpgradeInventory upgradeInventory = null;

    private bool occupied = false;
    private PointerEventData pointerEventData = null;
    private Renderer render = null;
    #endregion

    #region Unity Functions
    protected virtual void Start()
    {
        towerInventory = nodeUI.TowerInventory;
        enemyInventory = nodeUI.EnemyInventory;
        upgradeInventory = nodeUI.UpgradeInventory;
        render = GetComponent<Renderer>();
    }

    protected virtual void OnMouseEnter()
    {
        if (isPointerOverUI()) return;

        if (occupied || PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Prep) return;
        
        // Check if nodeUI is within a certain distance and active in the scene
        if (nodeUI.gameObject.activeInHierarchy &&
            Vector3.Distance(transform.position, nodeUI.transform.position) <= distanceToNodeUI)
        {
            return;
        }
        
        visualRenderer.enabled = true;
    }

    protected virtual void OnMouseExit()
    {
        visualRenderer.enabled = false;
    }

    protected virtual void OnMouseDown()
    {
        if (!visualRenderer.enabled) return;

        OpenNodeUI();
    }
    #endregion

    #region Public Functions
    protected abstract void UpdateNodeUI();
    #endregion

    #region Private Functions
    private bool isPointerOverUI()
    {
        pointerEventData = new PointerEventData(eventSystem)
        {
            position = Input.mousePosition
        };

        List<RaycastResult> results = new List<RaycastResult>();
        graphicRaycaster.Raycast(pointerEventData, results);

        return results.Count > 0;
    }

    public void OpenNodeUI()
    {
        NodeManager.Instance.SelectedNode = this;

        Vector3 nodeUIPosition = transform.position;
        nodeUIPosition.y = 10f;
        nodeUIPosition.z -= 8f;
        nodeUI.transform.position = nodeUIPosition;

        nodeUI.Close();
        UpdateNodeUI();
        nodeUI.Open();
    }
    #endregion
}
