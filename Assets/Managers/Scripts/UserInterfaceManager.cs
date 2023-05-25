using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    #region Public Variables
    public static UserInterfaceManager Instance = null;

    public Players_List_UI PlayersListUI
    {
        get { return playersListUI; }
    }
    #endregion

    #region Fields
    [SerializeField] private GameObject gameOverUI = null;
    [SerializeField] private Players_List_UI playersListUI = null;
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

    #region Public Functions
    public void ShowGameOverUI()
    {
        if (!gameOverUI.activeInHierarchy) gameOverUI.SetActive(true);
    }
    #endregion
}
