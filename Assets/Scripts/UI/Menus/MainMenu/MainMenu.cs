using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Gotchi.Events;

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
        mainMenuCanvasAnimator.SetTrigger(mainMenuCanvasAnimatorCloseTrigger);
        StartCoroutine(waitAndStart());
    }

    private IEnumerator waitAndStart()
    {
        yield return new WaitForSeconds(1.1f);
        mainMenuCanvas.SetActive(false);
        PhaseManager.Instance.StartNextPhase();
    }
}
