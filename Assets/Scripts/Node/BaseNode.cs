using System.Collections.Generic;
using UnityEngine;
using UnityEngine.EventSystems;
using UnityEngine.UI;

public abstract class BaseNode : MonoBehaviour
{
    #region Public Variables
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

    public Renderer Renderer
    {
        get { return render; }
    }

    public Material UnoccupiedMaterial
    {
        get { return unoccupiedMaterial; }
    }

    public Material OccupiedMaterial
    {
        get { return occupiedMaterial; }
    }
    #endregion

    #region Fields
    [Header("Required Refs")]
    [SerializeField] protected GameObject buildEffect = null; // TODO: follow pooling pattern
    [SerializeField] protected NodeUI nodeUI = null;
    [SerializeField] protected Renderer visualRenderer = null;
    [SerializeField] protected Material unoccupiedMaterial = null;
    [SerializeField] protected Material occupiedMaterial = null;

    // Add these fields for handling UI
    [SerializeField] private GraphicRaycaster graphicRaycaster;
    [SerializeField] private EventSystem eventSystem;
    #endregion

    #region Private Variables
    protected bool occupied = false;
    protected TowerInventory towerInventory = null;
    protected EnemyInventory enemyInventory = null;
    protected UpgradeInventory upgradeInventory = null;
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

    protected virtual void Update()
    {
        // TODO: potentially move this logic so that it's based on an event listener rather than running it in Update()
        if (PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Prep && occupied)
        {
            occupied = false;
        }
    }

    protected virtual void OnMouseEnter()
    {
        // Add this check to see if the pointer is over a UI element
        if (isPointerOverUI())
        {
            return;
        }

        if (occupied || PhaseManager.Instance.CurrentPhase != PhaseManager.Phase.Prep) return;

        visualRenderer.enabled = true;
    }

    protected virtual void OnMouseExit()
    {
        visualRenderer.enabled = false;
    }

    protected virtual void OnMouseDown()
    {
        if (!visualRenderer.enabled) return;

        openNodeUI();
    }
    #endregion

    #region Public Functions
    protected abstract void UpdateNodeUI();
    #endregion

    #region Private Functions
    // Add this new method to check if the pointer is over any UI elements
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

    private void openNodeUI()
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
