using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UserInterfaceManager : MonoBehaviour
{
    #region Public Variables
    public static UserInterfaceManager Instance = null;
    #endregion

    #region Fields
    [SerializeField] private GameObject GameOverUI = null;
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
        if (!GameOverUI.activeInHierarchy) GameOverUI.SetActive(true);
    }
    #endregion
}
