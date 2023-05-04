using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;
using Gotchi.Network;

public class MainMenu : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject mainMenuCanvas = null;
    [SerializeField] private Animator mainMenuCanvasAnimator = null;
    [SerializeField] private GameObject loadingMenuCanvas = null;
    [SerializeField] private Animator loadingMenuCanvasAnimator = null;

    [Header("Attributes")]
    [SerializeField] private string mainMenuCanvasAnimatorCloseTrigger = "Close";
    [SerializeField] private string loadingMenuCanvasAnimatorOpenTrigger = "Open";
    [SerializeField] private string loadingMenuCanvasAnimatorCloseTrigger = "Close";
    [SerializeField] private bool skipMainMenu;
    #endregion

    #region Unity Functions
    private void Start()
    {
        if(skipMainMenu)
        {
            StartOnlineCoop();
        }
    }
    #endregion

    #region Public Functions
    public void StartOnlineCoop()
    {
        EventBus.MenuEvents.MenuItemSelectedLong();
        StartCoroutine(waitAndStart());
    }
    #endregion

    #region Private Functions
    private IEnumerator waitAndStart()
    {
        loadingMenuCanvas.SetActive(true);
        loadingMenuCanvasAnimator.SetTrigger(loadingMenuCanvasAnimatorOpenTrigger);
        while (!NetworkManager.Instance.IsReady)
        {
            yield return new WaitForSeconds(0.5f);
        }
        mainMenuCanvasAnimator.SetTrigger(mainMenuCanvasAnimatorCloseTrigger);
        loadingMenuCanvasAnimator.SetTrigger(loadingMenuCanvasAnimatorCloseTrigger);
        yield return new WaitForSeconds(1f);
        loadingMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);
        PhaseManager.Instance.StartFirstPrepPhase();
    }
    #endregion
}
