using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;
using Gotchi.Network;

public class MainMenu : MonoBehaviour
{
    [SerializeField] private GameObject mainMenuCanvas = null;
    [SerializeField] private Animator mainMenuCanvasAnimator = null;
    [SerializeField] private string mainMenuCanvasAnimatorCloseTrigger = "Close";
    [SerializeField] private bool skipMainMenu;

    private void Start()
    {
        if(skipMainMenu)
        {
            StartFreePlay();
        }
    }

    public void StartFreePlay()
    {
        EventBus.MenuEvents.MenuItemSelectedLong();
        StartCoroutine(waitAndStart());
    }

    private IEnumerator waitAndStart()
    {
        while (!NetworkManager.Instance.IsReady)
        {
            yield return new WaitForSeconds(0.5f);
        }
        mainMenuCanvasAnimator.SetTrigger(mainMenuCanvasAnimatorCloseTrigger);
        yield return new WaitForSeconds(1f);
        mainMenuCanvas.SetActive(false);
        PhaseManager.Instance.StartFirstPrepPhase();
    }
}
