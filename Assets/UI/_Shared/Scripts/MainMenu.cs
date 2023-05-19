using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using Gotchi.Events;
using Gotchi.Network;
using PhaseManager.Presenter;

public class MainMenu : MonoBehaviour
{
    #region Fields
    [Header("Required Refs")]
    [SerializeField] private GameObject mainMenuCanvas = null;
    [SerializeField] private Animator mainMenuCanvasAnimator = null;
    [SerializeField] private GameObject loadingMenuCanvas = null;
    [SerializeField] private Animator loadingMenuCanvasAnimator = null;
    [SerializeField] private GameObject usernameMenuCanvas = null;
    [SerializeField] private Animator usernameMenuCanvasAnimator = null;
    [SerializeField] private TextMeshProUGUI usernameText = null;
    [SerializeField] private TextMeshProUGUI lobbyIdText = null;

    [Header("Attributes")]
    [SerializeField] private string mainMenuCanvasAnimatorCloseTrigger = "Close";
    [SerializeField] private string loadingMenuCanvasAnimatorOpenTrigger = "Open";
    [SerializeField] private string loadingMenuCanvasAnimatorCloseTrigger = "Close";
    [SerializeField] private string usernameMenuCanvasAnimatorOpenTrigger = "Open";
    [SerializeField] private string usernameMenuCanvasAnimatorCloseTrigger = "Close";
    #endregion

    #region Unity Functions
    void Start()
    {
        EventBus.PhaseEvents.MainMenuStarted();

        if (PlayerPrefs.HasKey("username"))
        {
            usernameText.text = PlayerPrefs.GetString("username");
        }
    }
    #endregion

    #region Public Functions
    public void ShowUsernameMenu()
    {
        EventBus.MenuEvents.MenuItemSelectedLong();
        usernameMenuCanvas.SetActive(true);
        usernameMenuCanvasAnimator.SetTrigger(usernameMenuCanvasAnimatorOpenTrigger);
    }

    public void StartFreePlay()
    {
        EventBus.MenuEvents.MenuItemSelectedLong();
        StartCoroutine(waitAndStart());
        PlayerPrefs.SetString("username", usernameText.text);
    }
    #endregion

    #region Private Functions
    private IEnumerator waitAndStart()
    {
        usernameMenuCanvasAnimator.SetTrigger(usernameMenuCanvasAnimatorCloseTrigger);
        yield return new WaitForSeconds(1f);
        usernameMenuCanvas.SetActive(false);

        loadingMenuCanvas.SetActive(true);
        loadingMenuCanvasAnimator.SetTrigger(loadingMenuCanvasAnimatorOpenTrigger);
        
        NetworkManager.Instance.InitializeNetworkRunner(lobbyIdText.text);
        while (!NetworkManager.Instance.IsReady)
        {
            yield return new WaitForSeconds(0.5f);
        }
        mainMenuCanvasAnimator.SetTrigger(mainMenuCanvasAnimatorCloseTrigger);
        loadingMenuCanvasAnimator.SetTrigger(loadingMenuCanvasAnimatorCloseTrigger);
        yield return new WaitForSeconds(1f);
        loadingMenuCanvas.SetActive(false);
        mainMenuCanvas.SetActive(false);

        PhasePresenter.Instance.StartFirstPrepPhase();
    }
    #endregion
}
