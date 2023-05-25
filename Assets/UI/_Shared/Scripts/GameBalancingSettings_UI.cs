using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.UI;
using GameMaster;

public class GameBalancingSettings_UI : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject settingsWindow = null;
    [SerializeField] private GameObject pauseWindow = null;
    [SerializeField] private List<Button> tabButtons = new List<Button>();
    [SerializeField] private List<GameObject> balanceMenus = new List<GameObject>();
    #endregion

    #region Private Variables
    private int currentTabIndex = 0;
    #endregion

    #region Unity Functions
    void Awake()
    {
        for (int i = 0; i < tabButtons.Count; i++)
        {
            int index = i;
            tabButtons[i].onClick.AddListener(() => switchTab(index));
        }
    }

    void Update()
    {
        if (Keyboard.current.escapeKey.wasPressedThisFrame)
        {
            TogglePausedWindow();
        }
    }
    #endregion

    #region Public Functions
    public void ToggleSettingsWindow()
    {
        if (settingsWindow.activeSelf)
        {
            settingsWindow.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            GameMasterEvents.MenuEvents.MenuItemSelectedLong();
            Time.timeScale = 0;
            settingsWindow.SetActive(true);
        }
    }

    public void TogglePausedWindow()
    {
        if (pauseWindow.activeSelf) 
        {
            pauseWindow.SetActive(false);
            Time.timeScale = 1;
        }
        else
        {
            GameMasterEvents.MenuEvents.MenuItemSelectedLong();
            Time.timeScale = 0;
            pauseWindow.SetActive(true);
        }
    }
    #endregion

    #region Private Functions
    private void switchTab(int tabIndex)
    {
        balanceMenus[currentTabIndex].SetActive(false);
        balanceMenus[tabIndex].SetActive(true);
        currentTabIndex = tabIndex;
    }
    #endregion
}