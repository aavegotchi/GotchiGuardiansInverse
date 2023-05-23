using UnityEngine;

public class NodeManager : MonoBehaviour
{
    #region Fields
    [SerializeField] private NodeUI nodeUI = null;
    #endregion

    #region Public Variables
    public static NodeManager Instance = null;

    public BaseNode SelectedNode
    {
        get { return selectedNode; }
        set { selectedNode = value; }
    }

    public NodeUI NodeUI
    {
        get { return nodeUI; }
    }
    #endregion

    #region Private Variables
    private BaseNode selectedNode = null;
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
}
